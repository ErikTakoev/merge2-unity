using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_Move : BattleUnitAction
    {
        
        protected bool isPathfinding;
        protected BattleCell movingToCell;
        
        public BattleUnitAction_Move(IBattleUnitStrategy strategy)
            : base (strategy)
        {
        }


        public override bool Action()
        {
            bool result = false;
            
            if (isPathfinding)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding in progress... for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }
                return true;
            }

            if (Unit.IsMoving)
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
            if (path.Count > 0 && path[path.Count - 1].IsReserved)
            {
                if (Unit.LogEnable)
                {
                    Debug.LogWarning($"Pathfinding: {Unit.name} last cell is reserved: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                }
                movingToCell = null;
                return;
            }
            Unit.MoveTo(path);
            if (Unit.LogEnable)
            {
                Debug.Log($"Pathfinding complete for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
            }
        }
        
        public override void Update()
        {
            var unit = Unit;
            if (unit.NextCell != unit.Cell)
            {
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, unit.NextCell.WorldPosition, 0.3f * Time.deltaTime);
                if (Vector3.Distance( unit.transform.position, unit.NextCell.WorldPosition) < 0.02f)
                {
                    unit.Cell = unit.NextCell;

                    if (!unit.MoveToNextCell())
                    {
                        unit.Character.SetState(CharacterState.Idle);
                        unit.MoveStop();
                        unit.NextCell = unit.Cell;
                    }
                }
            }
        }
    }
}