using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitStrategy : IBattleUnitStrategy
    {
        bool isMoving;
        bool isPathfinding;
        BattleHero target;
        BattleCell movingToCell;
        
        public BattleUnitStrategy(BattleHero unit, List<EquipmentItem> items)
            : base(unit, items)
        {
        }

        protected override bool Attack()
        {
            bool result = false;

            if (Unit.IsMoving)
            {
                return false;
            }
            if (target == null)
            {
                return false;
            }
            var targetPos = target.Cell.CellPos;
            var unitPos = Unit.Cell.CellPos;
            var diff = targetPos - unitPos;
            if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
            {
                if (diff.x == 0 && Mathf.Abs(diff.y) == 1)
                {
                    result = false;
                }
                else if (Unit.IsAttackReady)
                {
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
            var field = BattleField.Instance;
            bool result = false;
            if (target == null)
            {
                target = field.FindTarget(Unit);
                result = true;
            }
            
            if (isPathfinding)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"Pathfinding in progress... for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y}");
                }
                return true;
            }

            if (Unit.IsMoving)
            {
                result = true;
            }

            BattleCell targetCell = target.NextCell == null ? target.Cell : target.NextCell;

            if (target != null && movingToCell != targetCell && !isPathfinding)
            {
                isPathfinding = true;
                movingToCell = targetCell;
                field.FindPathToTarget(Unit.Cell, movingToCell, OnPathfindingComplete);
                result = true;
            }
            

            return result;
        }

        void OnPathfindingComplete(List<BattleCell> path)
        {
            if (path == null)
            {
                Debug.LogError($"Pathfinding: is not found path to: Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y}");
                return;
            }
            Unit.MoveTo(path);
            isPathfinding = false;
            if (Unit.LogEnable)
            {
                Debug.Log($"Pathfinding complete for :{Unit.name}, moving to Cell x:{movingToCell.CellPos.x}, y:{movingToCell.CellPos.y}");
            }
        }

        protected override  bool SpecialAttack()
        {
            return false;
        }
    }
}
