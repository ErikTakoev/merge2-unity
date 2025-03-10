using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitStrategy : IBattleUnitStrategy
    {
        
        public BattleUnitStrategy(BattleHero unit, List<EquipmentItem> items)
            : base(unit, items)
        {
        }

        protected override bool FindTarget()
        {
            target = BattleField.Instance.FindTarget(Unit);

            return false;
        }


        protected override bool Attack()
        {
            bool result = false;

            if (target == null)
            {
                return false;
            }
            var targetPos = target.NextCell.CellPos;
            var unitPos = Unit.Cell.CellPos;
            var diff = targetPos - unitPos;
            if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
            {
                result = true;
                if (diff.x == 0 && Mathf.Abs(diff.y) == 1)
                {
                    result = false;
                }
                else if (Unit.IsAttackReady)
                {
                    Unit.MoveStop();
                    Unit.Attack(target);
                }
            }
            return result;
        }

        protected override  bool DodgeRoll()
        {
            if (Attackers.Count == 0)
            {
                return false;
            }
            if (!Unit.IsAttacking && !Unit.IsDodgeRolling && UnityEngine.Random.value > 0.5f)
            {
                Unit.DodgeRoll(Attackers[0]);
                return true;
            }
            Attackers.Clear();
            return false;
        }

        protected override  bool Move()
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

            BattleCell targetCell = target.NextCell;

            BattleCell unitCell = Unit.NextCell;
            if (target != null && movingToCell != targetCell && !isPathfinding && unitCell != targetCell)
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
                Debug.LogError($"Pathfinding: {Unit.name} is not found path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                movingToCell = null;
                return;
            }
            if (path.Count > 0 && path[path.Count - 1].IsReserved)
            {
                Debug.LogWarning($"Pathfinding: {Unit.name} last cell is reserved: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
                movingToCell = null;
                return;
            }
            Unit.MoveTo(path);
            if (Unit.LogEnable)
            {
                Debug.Log($"Pathfinding complete for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y} from x: {Unit.Cell.CellPos.x}, y: {Unit.Cell.CellPos.y}");
            }
        }

        protected override  bool SpecialAttack()
        {
            return false;
        }
    }
}
