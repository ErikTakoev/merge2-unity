using UnityEngine;

namespace BattleField
{
    public abstract class BattleUnitAction
    {
        protected BattleUnitAbstractStrategy strategy;
        public BattleUnitAction(BattleUnitAbstractStrategy strategy)
        {
            this.strategy = strategy;
        }

        protected BattleUnit Unit { get { return strategy.Unit; }}
        protected BattleUnit Target { get { return strategy.Target; }}

        public abstract bool Action();
        public virtual void Update() {}
        public virtual void LateUpdate() {}
    }
}