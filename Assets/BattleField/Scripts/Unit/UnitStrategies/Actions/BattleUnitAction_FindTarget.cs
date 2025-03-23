using VContainer;

namespace BattleField
{
	public class BattleUnitAction_FindTarget : BattleUnitAction
	{
		[Inject] BattleField field;

		public BattleUnitAction_FindTarget(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
		}


		public override bool Action()
		{
			strategy.Target = field.FindTarget(Unit, Target);
			return false;
		}
		//public override void Update() {}
	}
}
