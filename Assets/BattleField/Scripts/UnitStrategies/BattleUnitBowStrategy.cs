using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitBowStrategy : IBattleUnitStrategy
    {
        
        public BattleUnitBowStrategy(BattleHero unit)
            : base(unit)
        {
            Actions.Add(new BattleUnitAction_FindTarget(this));
            Actions.Add(new BattleUnitAction_BowAttack(this));
            Actions.Add(new BattleUnitAction_MoveKeepDistance(this));
            Actions.Add(new BattleUnitAction_Move(this));
        }
    }
}
