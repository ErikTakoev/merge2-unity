using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public abstract class BattleUnitAbstractStrategy : MonoBehaviour
    {
        public virtual void Init(BattleUnit unit)
        {
            Unit = unit;
            
            Attackers = new List<BattleUnit>();
            Followers = new List<BattleUnit>();
            Actions = new List<BattleUnitAction>();
            Mover = new BattleUnitMover(this);
        }
        
        public BattleUnit Unit { get; private set; }
        BattleUnit target;
        public BattleUnit Target
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
        public List<BattleUnit> Attackers { get; private set; }
        public List<BattleUnit> Followers { get; private set; }
        public BattleUnitMover Mover { get; private set; }

        protected List<BattleUnitAction> Actions { get; private set; }
        
        public virtual void AddAttacker(BattleUnit attacker)
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
            
            if (unit.IsDead) return;

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
            if (Unit.IsDead) return;
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].LateUpdate();
            }
        }
    }
}