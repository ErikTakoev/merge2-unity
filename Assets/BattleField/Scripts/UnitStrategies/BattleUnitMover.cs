using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitMover
    {
        public bool IsMoving { get { return Path != null; } }
        private IBattleUnitStrategy strategy;
        bool moveUpdateEnable = true;
        BattleHero unit;
        List<BattleCell> Path;

        public BattleUnitMover(IBattleUnitStrategy strategy)
        {
            this.strategy = strategy;
            unit = strategy.Unit;
        }

        public void Update()
        {
            if (moveUpdateEnable)
            {
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, unit.NextCell.WorldPosition, 0.6f * Time.deltaTime);
                if (Vector3.Distance( unit.transform.position, unit.NextCell.WorldPosition) < 0.02f)
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

        public void MoveTo(List<BattleCell> newPath)
        {
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
        }
        public bool MoveToNextCell()
        {
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
                    MoveStop();
                    return false;
                }
                cell = Path[0];
                Path.RemoveAt(0);
            }
            if (!cell.IsAvailableCell())
            {
                MoveStop();
                return false;
            }
            unit.NextCell = cell;
            unit.Turn(unit.NextCell.WorldPosition);
            return true;
        }
    }
}