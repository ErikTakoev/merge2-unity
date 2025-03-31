using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace BattleField
{
	public class BattleUnitAction_Retreat : BattleUnitAction
	{
		bool movingToStartPosition;

		BattleUnitMover mover;
		[Inject] BattleField field;
		[Inject] CloudMessage[] cloudMessagePrefabs;


		public BattleUnitAction_Retreat(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
			mover = strategy.Mover;
		}


		public override bool Action()
		{
			if (Target != null)
			{
				if (Target.NextCell.CellPos.x < 15)
				{
					movingToStartPosition = false;
					// Відступати нікуди
					return false;
				}
			}

			int unitCount = field.GetUnitCount(Unit.IsHero);

			if (unitCount > 1)
			{
				movingToStartPosition = false;
				return false;
			}


			if (movingToStartPosition)
			{
				return true;
			}
			bool result = true;

			BattleCell targetCell = field.GetCell(5, 3);
			BattleCell unitCell = Unit.NextCell;

			mover.SetMovingCell(targetCell);
			if (Unit.LogEnable)
			{
				Debug.Log($"Pathfinding: {Unit.name} start to find path to: Cell x:{targetCell.CellPos.x}, y:{targetCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
			}
			movingToStartPosition = true;
			field.FindPathToCell(unitCell, targetCell, OnPathfindingComplete);

			return result;
		}

		void OnPathfindingComplete(List<BattleCell> path)
		{
			var movingToCell = mover.GetMovingCell();
			if (path == null || path.Count == 0)
			{
				if (Unit.LogEnable)
				{
					Debug.Log($"Pathfinding: {Unit.name} is not found path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
				}
				mover.SetMovingCell(null);
				return;
			}
			mover.MoveTo(path);
			if (Unit.LogEnable)
			{
				Debug.Log($"Pathfinding complete for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
			}
		}
	}
}
