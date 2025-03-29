using System;
using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace BattleField
{
	public class BattleField : MonoBehaviour
	{
		[SerializeReference]
		BattleHeroHealthBar healthBar;
		[SerializeReference]
		GameObject unitPrefab;
		[SerializeReference]
		GameObject teleportPrefab;

		[SerializeField]
		BattleGrid grid;

		List<BattleUnit> heroes;
		[SerializeField]
		BattleUnit heroBoss;
		[SerializeField]
		GameObject heroBossPrefab;

		List<BattleUnit> enemies;
		[SerializeField]
		BattleUnit enemyBoss;
		[SerializeField]
		GameObject[] enemyList;
		[SerializeField]
		TextAsset enemyDataSpawn;

		BattleFieldEnemySpawner enemySpawner;

		[SerializeReference]
		CinemachineTargetGroup cinemachineTargetGroup;

		void Start()
		{
			heroes = new List<BattleUnit>();
			enemies = new List<BattleUnit>();

			if (heroBossPrefab == null)
			{
				Debug.LogError("BattleField: heroBossPrefab or enemyBossPrefab is empty");
				return;
			}
			if (cinemachineTargetGroup == null)
			{
				Debug.LogError("BattleField: cinemachineTargetGroup is empty");
				return;
			}
			if (enemyDataSpawn == null)
			{
				Debug.LogError("BattleField: enemyDataSpawn is empty");
				return;
			}
			enemySpawner = new BattleFieldEnemySpawner(enemyDataSpawn.text.Split('\n'));
			enemySpawner.OnSpawn += OnSpawnEnemy;

			heroBoss = CreateUnit(heroBossPrefab, null, null, true, GetCell(2, 4));
			cinemachineTargetGroup.AddMember(heroBoss.transform, 1, 0.1f);
			heroBoss.Stats.OnChangeHealth += healthBar.OnChangeHealth;
			var health = heroBoss.Stats.GetHealth();
			healthBar.OnChangeHealth(health, health);
		}

		public int GetUnitCount(bool isHero)
		{
			return isHero ? heroes.Count : enemies.Count;
		}

		void OnSpawnEnemy(int posY, int enemyIndex)
		{
			var spawnPoint = grid.GetCell(grid.width - 1, posY);
			CreateUnit(enemyList[enemyIndex], null, null, false, spawnPoint);
		}

		public void OnDraggedHeroToBattleField(BattleHeroStyle style, List<EquipmentItem> items)
		{
			if (unitPrefab == null)
			{
				Debug.LogError("BattleField: heroPrefab is empty");
				return;
			}
			var spawnCell = grid.GetRandomSpawnPoint(heroBoss);

			var teleport = Instantiate(teleportPrefab, transform);
			teleport.transform.position = spawnCell.WorldPosition;
			Destroy(teleport, 5);

			DOVirtual.DelayedCall(0.7f, () =>
			{
				CreateUnit(unitPrefab, style, items, true, spawnCell);
			});

		}

		public List<(BattleUnit, int)> FindTargetsNear(BattleUnit unit)
		{
			// Unit, distance
			List<(BattleUnit, int)> targets = new List<(BattleUnit, int)>();
			var cellPos = unit.NextCell.CellPos;
			int searchDistance = 6;
			for (int x = -searchDistance; x < searchDistance; x++)
			{
				for (int y = -searchDistance; y < searchDistance; y++)
				{
					int posX = cellPos.x + x;
					int posY = cellPos.y + y;
					var cell = grid.GetCell(posX, posY);
					if (cell != null)
					{
						var cellUnit = cell.Unit;
						if (cellUnit != null && cellUnit.IsHero != unit.IsHero)
						{
							var distance = Pathfinding.GetManhattanDistance(unit.NextCell, cellUnit.NextCell);
							targets.Add((cellUnit, distance));
						}
					}
				}
			}

			return targets;
		}

		public BattleUnit FindTarget(BattleUnit unit, BattleUnit cashedTarget)
		{
			List<BattleUnit> targets = unit.IsHero ? enemies : heroes;

			BattleUnit target = null;
			float minDistance = float.MaxValue;

			foreach (BattleUnit t in targets)
			{
				var unitCell = unit.NextCell;
				var targetCell = t.NextCell;
				var diff = unitCell.CellPos - targetCell.CellPos;

				int distance = Pathfinding.GetManhattanDistance(unitCell, targetCell);
				if (diff.x == 0 && diff.y == 1)
				{
					distance += 2;
				}
				if (cashedTarget == t && distance == minDistance)
				{
					minDistance = distance;
					target = t;
				}
				else if (distance < minDistance)
				{
					minDistance = distance;
					target = t;
				}
			}
			if (!unit.IsHero)
			{
				// Debug.LogWarning($"Enemy target:{target?.name}");
			}
			return target;
		}


		public void FindPathToUnitAttackPosition(BattleCell unitCell, BattleCell targetCell, System.Action<List<BattleCell>> callback)
		{
			var targetCellPos = targetCell.CellPos;
			List<BattleCell> targetsCell = new List<BattleCell>();

			BattleCell tmpCell;
			for (int x = -1; x <= 1; x += 2)
			{
				for (int y = -1; y <= 1; y++)
				{
					tmpCell = grid.GetCell(targetCell.CellPos.x + x, targetCell.CellPos.y + y);
					if (tmpCell != null && tmpCell.IsAvailableCell())
					{
						targetsCell.Add(tmpCell);
					}
				}
			}
			grid.FindPathAsync(unitCell, targetsCell, callback).Forget();
		}

		public void FindPathToCell(BattleCell unitCell, BattleCell targetCell, System.Action<List<BattleCell>> callback)
		{
			grid.FindPathAsync(unitCell, new List<BattleCell>() { targetCell }, callback).Forget();
		}

		public BattleCell GetCell(int x, int y)
		{
			return grid.GetCell(x, y);
		}

		BattleUnit CreateUnit(GameObject prefab, BattleHeroStyle? style, List<EquipmentItem> items, bool isHero, BattleCell spawnPoint)
		{
			BattleUnit hero = Instantiate(prefab, transform).GetComponent<BattleUnit>();
			hero.name = (isHero ? "Hero" : "Enemy") + (heroes.Count + enemies.Count);
			var units = isHero ? heroes : enemies;
			units.Add(hero);

			hero.OnUnitDeadEvent += OnUnitDead;

			hero.Init(style, items, spawnPoint, isHero);

			return hero;
		}

		private void OnUnitDead(BattleUnit unit)
		{
			var units = unit.IsHero ? heroes : enemies;
			cinemachineTargetGroup.RemoveMember(unit.transform);
			units.Remove(unit);
		}

		void Update()
		{
			enemySpawner.Update();
		}
	}
}
