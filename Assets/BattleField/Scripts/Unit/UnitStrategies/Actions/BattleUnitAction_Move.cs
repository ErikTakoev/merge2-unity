using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_Move : BattleUnitAction
    {
        
        bool isPathfinding;
        BattleCell movingToCell;

        BattleUnitMover mover;

        
        public BattleUnitAction_Move(IBattleUnitStrategy strategy)
            : base (strategy)
        {
            mover = strategy.Mover;
        }


        public override bool Action()
        {
            if (Target == null) return false;
            bool result = false;
            
            if (isPathfinding)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding in progress... for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }
                return true;
            }

            if (mover.IsMoving)
            {
                result = true;
            }
            else
            {
                movingToCell = null;
            }

            BattleCell targetCell = Target.NextCell;

            BattleCell unitCell = Unit.NextCell;
            if (Target != null && movingToCell != targetCell && !isPathfinding && unitCell != targetCell)
            {
                isPathfinding = true;
                movingToCell = targetCell;
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding: {Unit.name} start to find path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }

                BattleField.Instance.FindPathToTarget(unitCell, movingToCell, OnPathfindingComplete);
                result = true;
            }
            

            return result;
        }
        
        void OnPathfindingComplete(List<BattleCell> path)
        {
            isPathfinding = false;
            if (path == null || path.Count == 0)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding: {Unit.name} is not found path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }
                movingToCell = null;
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