using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitStrategy : IBattleUnitStrategy
    {
        
        public BattleUnitStrategy(BattleHero unit)
            : base(unit)
        {
            Actions.Add(new BattleUnitAction_FindTarget(this));
            Actions.Add(new BattleUnitAction_DefenseShield(this));
            Actions.Add(new BattleUnitAction_AttackShield(this));
            Actions.Add(new BattleUnitAction_MeleeAttack(this));
            Actions.Add(new BattleUnitAction_Move(this));
        }
    }
}
