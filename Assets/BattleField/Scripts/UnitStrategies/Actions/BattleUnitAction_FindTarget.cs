using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_FindTarget : BattleUnitAction
    {
        public BattleUnitAction_FindTarget(IBattleUnitStrategy strategy)
            : base (strategy)
        {
        }


        public override bool Action()
        {
            strategy.Target = BattleField.Instance.FindTarget(Unit, Target);
            return false;
        }
        //public override void Update() {}
    }
}