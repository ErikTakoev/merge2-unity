using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleArrowController : MonoBehaviour
	{
		[SerializeReference] GameObject arrowPrefab;

		Queue<BattleArrow> shoots;

		void Awake()
		{
			shoots = new Queue<BattleArrow>();
		}

		public Transform GetArrow(Transform parent)
		{
			var arrow = Instantiate(arrowPrefab, parent).transform;
			arrow.localPosition = Vector3.zero;
			arrow.localEulerAngles = Vector3.zero;
			arrow.localScale = Vector3.one;
			return arrow;
		}

		public void Shoot(Transform arrow, BattleUnit unit, BattleUnit target)
		{
			BattleArrow battleArrow = arrow.GetComponent<BattleArrow>();
			battleArrow.Shoot(unit, target);
			arrow.parent = transform;

			shoots.Enqueue(battleArrow);
		}

		void FixedUpdate()
		{
			List<Collider2D> results = new List<Collider2D>(5);
			foreach (var arrowData in shoots)
			{
				if (arrowData.Rigidbody == null)
				{
					continue;
				}
				if (arrowData.ArrowCollider.Overlap(results) != 0)
				{
					var targetColliders = arrowData.Target.Colliders;
					foreach (var collider in targetColliders)
					{
						if (!collider.gameObject.activeInHierarchy)
						{
							continue;
						}
						if (results.Contains(collider))
						{
							arrowData.HitInUnit(collider);
							break;
						}
					}
				}
			}
		}


	}
}
