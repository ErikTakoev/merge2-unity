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
                    tr.localPosition = Vector3.zero;
                }
            }
        }
        [SerializeField]
        bool logEnable = false;

        Chip chip;

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
    }
}