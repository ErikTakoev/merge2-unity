using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
	public class BattleArrowController : MonoBehaviour
	{
		[SerializeReference] GameObject arrowPrefab;

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
		}


	}
}
