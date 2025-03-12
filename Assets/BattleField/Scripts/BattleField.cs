using System;
using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleField : MonoBehaviour
    {
        [SerializeReference]
        GameObject unitPrefab;
        [SerializeField]
        Transform[] heroesStartPositions;

        [SerializeField]
        BattleGrid grid;

        List<BattleHero> heroes;
        [SerializeField]
        BattleHero heroBoss;
        [SerializeField]
        GameObject heroBossPrefab;

        List<BattleHero> enemies;
        [SerializeField]
        BattleHero enemyBoss;
        [SerializeField]
        GameObject enemyBossPrefab;

        public static BattleField Instance;

        void Start()
        {
            Instance = this;
            heroes = new List<BattleHero>();
            enemies = new List<BattleHero>();

            if (heroBossPrefab == null || enemyBossPrefab == null)
            {
                Debug.LogError("BattleField: heroBossPrefab or enemyBossPrefab is empty");
                return;
            }

            heroBoss = CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            // CreateUnit(heroBossPrefab, null, null, true);
            enemyBoss = CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
            // CreateUnit(enemyBossPrefab, null, null, false);
        }

        public void OnDraggedHeroToBattleField(BattleHeroStyle style, List<EquipmentItem> items)
        {
            if (unitPrefab == null)
            {
                Debug.LogError("BattleField: heroPrefab is empty");
                return;
            }

            CreateUnit(style, items, true);
        }

        public BattleHero FindTarget(BattleHero unit, BattleHero cashedTarget)
        {
            List<BattleHero> targets = unit.IsHero ? enemies : heroes;

            BattleHero target = null;
            float minDistance = float.MaxValue;

            foreach (BattleHero t in targets)
            {
                var unitCell = unit.NextCell;
                var targetCell = t.NextCell;
                var diff = unitCell.CellPos - targetCell.CellPos;

                int distance = Pathfinding.GetManhattanDistance(unitCell, targetCell);
                if (diff.x == 0 && diff.y == 1)
                {
                    distance+=2;
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


        public void FindPathToTarget(BattleCell unitCell, BattleCell targetCell, System.Action<List<BattleCell>> callback)
        {
            var targetCellPos = targetCell.CellPos;
            List<BattleCell> targetsCell = new List<BattleCell>();

            BattleCell tmpCell;
            for (int x = -1; x <= 1; x+=2)
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
            grid.FindPathAsync(unitCell, targetsCell, callback);
        }

        public BattleCell GetCell(int x, int y)
        {
            return grid.GetCell(x, y);
        }


        BattleHero CreateUnit(GameObject prefab, BattleHeroStyle? style, List<EquipmentItem> items,  bool isHero)
        {
            BattleCell spawnPoint = grid.GetRandomSpawnPoint(isHero);

            BattleHero hero = Instantiate(prefab, transform).GetComponent<BattleHero>();
            hero.name = (isHero ? "Hero" : "Enemy") + (heroes.Count + enemies.Count);
            IBattleUnitStrategy strategy;
            if (isHero)
            {
                heroes.Add(hero);
                strategy = new BattleUnitShieldStrategy(hero);
            }
            else
            {
                enemies.Add(hero);
                strategy = new BattleUnitBowStrategy(hero);
            }

            hero.Init(style, items, strategy, spawnPoint, isHero);
            

            return hero;
        }

        private BattleHero CreateUnit(BattleHeroStyle style, List<EquipmentItem> items, bool isHero)
        {
            BattleHero hero = CreateUnit(unitPrefab, style, items, isHero);

            return hero;
        }
    }
}
