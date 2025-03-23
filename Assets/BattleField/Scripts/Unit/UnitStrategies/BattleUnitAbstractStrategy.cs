using System.Collections.Generic;
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
			actions = new List<BattleUnitAction>();
			Mover = new BattleUnitMover(this);
		}

		public BattleUnit Unit { get; private set; }

		private BattleUnit target;
		public BattleUnit Target
		{
			get => target;
			set
			{
				if (target == value)
				{
					return;
				}
				if (target != null)
				{
					_ = target.Strategy.Followers.Remove(Unit);
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

		private List<BattleUnitAction> actions;

		protected virtual void AddAction(BattleUnitAction action)
		{
			BattleContainer.Resolver.Inject(action);
			actions.Add(action);
		}

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
			Vector2Int targetPos = Target.NextCell.CellPos;
			Vector2Int unitPos = Unit.NextCell.CellPos;
			Vector2Int diff = targetPos - unitPos;
			if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
			{
				return diff.x == 0 && Mathf.Abs(diff.y) == 1 ? null : diff;
			}
			return null;
		}

		public virtual void Update()
		{
			BattleUnit unit = Unit;

			if (unit.IsDead)
			{
				return;
			}

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
				for (int i = 0; i < actions.Count; i++)
				{
					if (actions[i].Action())
					{
						break;
					}
				}
			}

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Update();
			}
			Mover.Update();
		}

		public virtual void LateUpdate()
		{
			if (Unit.IsDead)
			{
				return;
			}

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].LateUpdate();
			}
		}
	}
}
