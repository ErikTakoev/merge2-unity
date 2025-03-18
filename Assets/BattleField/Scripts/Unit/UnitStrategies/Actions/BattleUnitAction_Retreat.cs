using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_Retreat : BattleUnitAction
    {
        bool movingToStartPosition;

        BattleUnitMover mover;

        
        public BattleUnitAction_Retreat(BattleUnitAbstractStrategy strategy)
            : base (strategy)
        {
            mover = strategy.Mover;
        }


        public override bool Action()
        {
            if (Target != null)
            {
                if (Target.NextCell.CellPos.x < 10)
                {
                    movingToStartPosition = false;
                    // Відступати нікуди
                    return false;
                }
            }

            BattleField field = BattleField.Instance;
            int unitCount = field.GetUnitCount(Unit.IsHero);

            if (unitCount > 1)
            {
                movingToStartPosition = false;
                return false;
            }

            bool result = false;
            var movingToCell = mover.GetMovingCell();
            
            if (movingToStartPosition)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding in progress... for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }
                return true;
            }

            BattleCell targetCell = field.GetCell(Random.Range(2, 5), Random.Range(4, 7));
            BattleCell unitCell = Unit.NextCell;

            mover.SetMovingCell(targetCell);
            movingToCell = targetCell;
            if (Unit.LogEnable)
            {
                Debug.Log($"Pathfinding: {Unit.name} start to find path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
            }

            BattleField.Instance.FindPathToTarget(unitCell, targetCell, OnPathfindingComplete);
            result = true;
            

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