using UnityEngine;

namespace Merge2
{
    public class DraggableChipLogic : MonoBehaviour
    {
        [SerializeReference]
        Cell prevCell;

        [SerializeReference]
        Chip draggableChip;

        Transform draggableTransform;
        Transform prevCellTransform;

        public delegate bool MergeEvent(Cell prevCell, Cell overCell);
        public event MergeEvent OnMerge;


        public void OnDragStart(Cell prevCell, Vector3 worldPosition)
        {
            if (!prevCell || !prevCell.Chip)
            {
                return;
            }
            this.prevCell = prevCell;
            draggableChip = prevCell.Chip;
            draggableChip.SetDragging(true);

            draggableTransform = draggableChip.transform;
            prevCellTransform = prevCell.transform;

            MoveToWorldPosition(worldPosition);
        }

        public void OnDrag(Vector3 worldPosition)
        {
            if (!prevCell)
            {
                return;
            }

            MoveToWorldPosition(worldPosition);
        }

        public void OnDragEnd(Cell newCell)
        {
            if (!prevCell)
            {
                return;
            }
            bool swap = true;
            if (OnMerge != null)
            {
                foreach(var handler in OnMerge.GetInvocationList())
                {
                    if ((bool)handler.DynamicInvoke(prevCell, newCell) == true)
                    {
                        swap = false;
                    }
                }
            }
            if (swap)
            {
                if (newCell)
                {
                    var tmpChip = newCell.Chip;
                    if (tmpChip != null)
                    {
                        tmpChip.SetDragging(true);
                    }
                    newCell.Chip = prevCell.Chip;
                    prevCell.Chip = tmpChip;
                }
                else
                {
                    prevCell.Chip = prevCell.Chip; // Для анімації повернення на попереднє місце
                }
                
            }

            prevCell = null;
            draggableChip = null;
        }

        private void MoveToWorldPosition(Vector3 worldPosition)
        {
            Vector3 localMousePos = prevCellTransform.InverseTransformPoint(worldPosition);
            localMousePos.z = 0;
            draggableTransform.localPosition = localMousePos;
        }
    }
}