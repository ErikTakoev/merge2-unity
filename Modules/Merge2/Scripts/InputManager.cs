using UnityEngine;

namespace Merge2
{
	public class InputManager : MonoBehaviour
	{
		public delegate void InputEvent(Vector2 position, Vector3 worldPosition);
		public event InputEvent OnTap;
		public event InputEvent OnDragStart;
		public event InputEvent OnDrag;
		public event InputEvent OnDragEnd;

		[SerializeField]
		bool logEnable = false;

		[SerializeReference]
		SpriteRenderer spriteRenderer;
		Transform spriteTransform;

		Camera mainCamera;

		[SerializeField]
		float timeForOnTap = 0.4f;
		float timeLeftOnTap;

		[SerializeField]
		float offsetPosForStartDragging = 0.4f;
		Vector2 startPos;

		bool press = false;
		bool dragging = false;


		void Start()
		{
			spriteTransform = spriteRenderer.transform;
			mainCamera = Camera.main;
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				press = true;
				timeLeftOnTap = timeForOnTap;
			}

			if (!press)
			{
				return;
			}

			Vector2 pos; /// TopLeft - 0, 0 coord; BottomRight - 5,5; coord (5 with spriteRenderer.size)
			Vector3 worldMousePos;
			{
				worldMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				Vector3 localMousePos = spriteTransform.InverseTransformPoint(worldMousePos);

				Vector2 spriteSize = spriteRenderer.size;

				pos.x = (localMousePos.x + spriteSize.x * 0.5f);
				pos.y = spriteSize.y - (localMousePos.y + spriteSize.y * 0.5f);
			}

			if (Input.GetMouseButtonDown(0))
			{
				startPos = pos;
			}
			else if (Input.GetMouseButton(0))
			{
				timeLeftOnTap -= Time.deltaTime;

				if (timeLeftOnTap < 0
					|| Vector2.Distance(startPos, pos) > offsetPosForStartDragging)
				{
					if (!dragging)
					{
						dragging = true;
						OnDragStartImpl(startPos, worldMousePos);
					}
					else
					{
						OnDragImpl(pos, worldMousePos);
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (dragging)
				{
					OnDragEndImpl(pos, worldMousePos);
					dragging = false;
				}
				else
				{
					OnTapImpl(pos, worldMousePos);
				}
				press = false;
			}
		}

		void OnTapImpl(Vector2 position, Vector3 worldPosition)
		{
			if (logEnable)
			{
				Debug.Log($"OnTap: X: {position.x}, Y: {position.y}, Dragging: {dragging}");
			}

			OnTap?.Invoke(position, worldPosition);
		}
		void OnDragStartImpl(Vector2 position, Vector3 worldPosition)
		{
			if (logEnable)
			{
				Debug.Log($"OnDragStart: X: {position.x}, Y: {position.y}, Dragging: {dragging}");
			}

			OnDragStart?.Invoke(position, worldPosition);
		}
		void OnDragImpl(Vector2 position, Vector3 worldPosition)
		{
			if (logEnable)
			{
				Debug.Log($"OnDrag: X: {position.x}, Y: {position.y}, Dragging: {dragging}");
			}
			OnDrag?.Invoke(position, worldPosition);
		}
		void OnDragEndImpl(Vector2 position, Vector3 worldPosition)
		{
			if (logEnable)
			{
				Debug.Log($"OnDragEnd: X: {position.x}, Y: {position.y}, Dragging: {dragging}");
			}
			OnDragEnd?.Invoke(position, worldPosition);
		}
	}
}
