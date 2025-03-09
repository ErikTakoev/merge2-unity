using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
            if (target != null)
            {
                RaycastHit2D[] raycastResult = new RaycastHit2D[2];
                int count = Unit.Collider.Raycast(target.transform.position - Unit.transform.position, raycastResult, 0.3f);
                if(count == 1 && raycastResult[0].collider.gameObject == target.gameObject)
                {
                    result = true;
                }
            }
            
            if(target != null && Unit.IsMoving)
            {
                if(result)
                {
                    movingToCell = null;
                    Unit.MoveStop();
                    Unit.Attack(target);
                }
            }
            if (target != null && Unit.IsAttackReady)
            {
                if(result)
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
            if (!Unit.IsAttacking && !Unit.IsDodgeRolling && Random.value > 0.5f)
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
                field.FindPath(Unit.Cell, movingToCell, OnPathfindingComplete);
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
