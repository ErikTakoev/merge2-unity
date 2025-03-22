using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
	public class BattleUnitMover
	{
		public bool IsMoving { get { return Path != null; } }
		private BattleUnitAbstractStrategy strategy;
		bool moveUpdateEnable = false;
		BattleCell movingToCell;
		BattleUnit unit;
		List<BattleCell> Path;

		public BattleUnitMover(BattleUnitAbstractStrategy strategy)
		{
			this.strategy = strategy;
			unit = strategy.Unit;
		}

		public void Update()
		{
			if (unit.IsDead) return;

			if (moveUpdateEnable)
			{
				unit.transform.position = Vector3.MoveTowards(unit.transform.position, unit.NextCell.WorldPosition, 0.6f * Time.deltaTime);
				if (Vector3.Distance(unit.transform.position, unit.NextCell.WorldPosition) < 0.02f)
				{
					unit.Cell = unit.NextCell;

					if (!MoveToNextCell())
					{
						moveUpdateEnable = false;
						unit.Character.SetState(CharacterState.Idle);
						MoveStop();
						unit.NextCell = unit.Cell;
					}
				}
			}
		}

		public void SetMovingCell(BattleCell cell)
		{
			movingToCell = cell;
		}

		public BattleCell GetMovingCell()
		{
			return movingToCell;
		}

		public void MoveTo(List<BattleCell> newPath)
		{
			if (unit.IsDead)
			{
				return;
			}

			Path = newPath;
			if (unit.LogEnable)
			{
				Debug.Log($"{unit.name} MoveTo for:{unit.name}, path:{Path.Count}");
			}

			if (Path.Count > 0)
			{
				if (unit.NextCell == unit.Cell)
				{
					moveUpdateEnable = true;
					unit.Character.SetState(CharacterState.Walk);
					MoveToNextCell();
				}
			}
		}

		public void MoveStop()
		{
			if (unit.LogEnable)
			{
				Debug.Log($"{unit.name} MoveStop");
			}
			Path = null;
			movingToCell = null;
		}
		public bool MoveToNextCell()
		{
			if (unit.LogEnable)
			{
				Debug.Log($"{unit.name} MoveToNextCell");
			}
			if (Path == null || Path.Count == 0)
			{
				return false;
			}
			var cell = Path[0];
			Path.RemoveAt(0);

			if (cell == unit.Cell)
			{
				if (Path.Count == 0)
				{
					return false;
				}
				cell = Path[0];
				Path.RemoveAt(0);
			}
			if (!cell.IsAvailableCell())
			{
				return false;
			}
			unit.NextCell = cell;
			unit.Turn(unit.NextCell.WorldPosition);
			return true;
		}
	}
}
