using UnityEngine;

namespace Merge2
{
	public class Cell : MonoBehaviour
	{
		public Chip Chip
		{
			get { return chip; }
			set
			{
				chip = value;
				if (chip != null)
				{
					Transform tr = chip.gameObject.transform;
					tr.SetParent(transform);
					if (chip.IsDragging())
					{
						isMoveingChip = true;
						movingTimeLeft = 0;
						chipTransform = tr;
						movingStartPosition = chipTransform.localPosition;
					}
					else
					{
						tr.localPosition = Vector3.zero;
						chip.SetDragging(false);
					}
				}
			}
		}
		[SerializeField]
		bool logEnable = false;

		Vector2Int cellPosition;
		public Vector2Int CellPosition
		{
			get { return cellPosition; }
		}

		[SerializeField]
		Chip chip;

		[SerializeField]
		float movingTime = 0.2f;
		bool isMoveingChip = false;
		Transform chipTransform;
		float movingTimeLeft = 0f;
		Vector3 movingStartPosition;

		public void Init(Vector2Int cellPos)
		{
			cellPosition = cellPos;
		}

		public void OnTap(Vector2 position)
		{
			if (logEnable)
			{
				Debug.Log($"OnTap in node:{gameObject.name}");
			}
			if (chip == null)
			{
				return;
			}

			chip.OnTap(position);
		}

		public void OnDragStart(Vector2 position)
		{
			if (logEnable)
			{
				Debug.Log($"OnDragStart in node:{gameObject.name}");
			}
			if (chip == null)
			{
				return;
			}

			chip.OnDragStart(position);
		}

		public void OnDragEnd(Vector2 position)
		{
			if (logEnable)
			{
				Debug.Log($"OnDragEnd in node:{gameObject.name}");
			}
			if (chip == null)
			{
				return;
			}

			chip.OnDragEnd(position);
		}

		public void OnDrag(Vector2 position)
		{
			if (logEnable)
			{
				Debug.Log($"OnDrag in node:{gameObject.name}");
			}
			if (chip == null)
			{
				return;
			}

			chip.OnDrag(position);
		}

		private void Update()
		{
			if (!isMoveingChip)
			{
				return;
			}
			if (!chip)
			{
				return;
			}
			movingTimeLeft += Time.deltaTime;

			chipTransform.localPosition = Vector3.Lerp(movingStartPosition, Vector3.zero, movingTimeLeft / movingTime);
			if (movingTimeLeft >= movingTime)
			{
				movingTimeLeft = 0;
				isMoveingChip = false;
				chip.SetDragging(false);
			}
		}
	}
}
