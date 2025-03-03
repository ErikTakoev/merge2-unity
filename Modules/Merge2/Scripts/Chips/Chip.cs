using UnityEngine;
using UnityEngine.Rendering;

namespace Merge2
{

    public partial class Chip : MonoBehaviour
    {
        protected ChipData data;
        [SerializeField]
        bool logEnable = false;

        public bool LogEnable
        {
            get { return logEnable; }
        }

        SortingGroup sorting;
        Animator animator;

        public ChipData Data
        {
            get { return data; }
        }

        public virtual void Init(ChipData data)
        {
            this.data = data;
            sorting = GetComponent<SortingGroup>();
            animator = GetComponent<Animator>();
            if (sorting == null)
            {
                Debug.LogError("Chip: SortingGroup is empty");
            }
            if (animator == null)
            {
                Debug.LogError("Chip: Animator is empty");
            }
        }

        public void SetDragging(bool value)
        {
            sorting.sortingOrder = value ? 2 : 1;
        }

        public bool IsDragging()
        {
            return sorting.sortingOrder == 2;
        }

        public virtual void OnTap(Vector2 position)
        {
            if (logEnable)
            {
                Debug.Log($"Chip: OnTap in node:{gameObject.name}");
            }
        }

        public virtual void OnDragStart(Vector2 position)
        {
            if (logEnable)
            {
                Debug.Log($"Chip: OnDragStart in node:{gameObject.name}");
            }
        }

        public virtual void OnDragEnd(Vector2 position)
        {
            if (logEnable)
            {
                Debug.Log($"Chip: OnDragEnd in node:{gameObject.name}");
            }
        }

        public virtual void OnDrag(Vector2 position)
        {
            if (logEnable)
            {
                Debug.Log($"Chip: OnDrag in node:{gameObject.name}");
            }
        }

		public void SendTrigger(AnimatorTrigger trigger)
		{
			SendTrigger(trigger.ToString());
		}

		public virtual void SendTrigger(string trigger)
		{
			if (animator != null)
			{
				animator.SetTrigger(trigger);
			}
		}

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}