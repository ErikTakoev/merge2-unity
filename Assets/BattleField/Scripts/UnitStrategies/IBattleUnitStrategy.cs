using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public abstract class IBattleUnitStrategy
    {
        public IBattleUnitStrategy(BattleHero unit)
        {
            Unit = unit;
            
            Attackers = new List<BattleHero>();
            Actions = new List<BattleUnitAction>();
        }
        
        public BattleHero Unit;
        public BattleHero Target;
        public Dictionary<EquipmentPart, EquipmentItem> Items { get { return Unit.Items; } }
        public List<BattleHero> Attackers;
        public List<BattleCell> Path;

        protected List<BattleUnitAction> Actions;
        

        public virtual void AddAttacker(BattleHero attacker)
        {
            Attackers.Add(attacker);
        }

        public Vector2Int? IsMeleeBattle()
        {
            if (Target == null)
            {
                return null;
            }
            var targetPos = Target.NextCell.CellPos;
            var unitPos = Unit.NextCell.CellPos;
            var diff = targetPos - unitPos;
            if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
            {
                if (diff.x == 0 && Mathf.Abs(diff.y) == 1)
                {
                    return null;
                }
                return diff;
            }
            return null;
        }

        public virtual void Update()
        {
            
            if (Unit.IsStunning)
            {
                if (Unit.LogEnable)
                {
                    Debug.Log($"{Unit.name} is stunned");
                }
                
                return;
            }
            else
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    if (Actions[i].Action())
                        break;
                }
            }
            
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Update();
            }
        }

        public void MoveTo(List<BattleCell> newPath)
        {
            Path = newPath;
            var unit = Unit;
            if (unit.LogEnable)
            {
                Debug.Log($"{unit.name} MoveTo for:{unit.name}, path:{Path.Count}");
            }
            
            if (Path.Count > 0)
            {
                if (unit.NextCell == unit.Cell)
                {
                    unit.Character.SetState(CharacterState.Run);
                    MoveToNextCell();
                }
            }
        }

        public void MoveStop()
        {
            var unit = Unit;
            if (unit.LogEnable)
            {
                Debug.Log($"{unit.name} MoveStop");
            }
            Path = null;
        }
        public bool MoveToNextCell()
        {
            var unit = Unit;
            
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