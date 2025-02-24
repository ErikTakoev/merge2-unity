using UnityEngine;

namespace Merge2
{
    public class GameManager : MonoBehaviour
    {
        [SerializeReference]
        InputManager inputManager;

        [SerializeReference]
        Field field;

        void Start()
        {
            if (inputManager == null)
            {
                Debug.LogError("InputManager is empty");
                return;
            }

            if (field == null)
            {
                Debug.LogError("Field is empty");
                return;
            }

            inputManager.OnTap += OnTap;
            inputManager.OnDragStart += OnDragStart;
            inputManager.OnDrag += OnDrag;
            inputManager.OnDragEnd += OnDragEnd;
        }

        private void OnTap(Vector2 position, Vector3 worldPosition)
        {
            field.OnTap(position);
        }

        private void OnDragStart(Vector2 position, Vector3 worldPosition)
        {
            field.OnDragStart(position, worldPosition);
        }

        private void OnDragEnd(Vector2 position, Vector3 worldPosition)
        {
            field.OnDragEnd(position, worldPosition);
        }

        private void OnDrag(Vector2 position, Vector3 worldPosition)
        {
            field.OnDrag(position, worldPosition);
        }
    }
}