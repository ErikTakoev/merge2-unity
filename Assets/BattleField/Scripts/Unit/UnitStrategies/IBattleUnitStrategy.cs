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
            Followers = new List<BattleHero>();
            Actions = new List<BattleUnitAction>();
            Mover = new BattleUnitMover(this);
        }
        
        public BattleHero Unit;
        BattleHero target;
        public BattleHero Target
        {
            get { return target; }
            set {
                if (target == value)
                {
                    return;
                }
                if (target != null)
                {
                    target.Strategy.Followers.Remove(Unit);
                }
                target = value;
                if (target != null)
                {
                    target.Strategy.Followers.Add(Unit);
                }
            }
        }
        public Dictionary<EquipmentPart, EquipmentItem> Items { get { return Unit.Items; } }
        public List<BattleHero> Attackers;
        public List<BattleHero> Followers;
        public BattleUnitMover Mover;

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
            var unit = Unit;

            if (unit.IsStunning)
            {
                if (unit.LogEnable)
                {
                    Debug.Log($"{unit.name} is stunned");
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
            Mover.Update();
        }

        public virtual void LateUpdate()
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].LateUpdate();
            }
        }
    }
}