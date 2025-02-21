using UnityEngine;

public class DraggableChip : MonoBehaviour
{
    [SerializeReference]
    Cell prevCell;

    [SerializeReference]
    Chip draggableChip;

    Transform draggableTransform;
    Transform prevCellTransform;

    public delegate bool MergeEvent(Cell prevCell, Cell newCell);
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
        draggableChip.SetDragging(false);
        if (OnMerge?.Invoke(prevCell, newCell) == false)
        {
            var tmpChip = newCell.Chip;
            newCell.Chip = prevCell.Chip;
            prevCell.Chip = tmpChip;
        }

        prevCell = null;
    }

    private void MoveToWorldPosition(Vector3 worldPosition)
    {
        Vector3 localMousePos = prevCellTransform.InverseTransformPoint(worldPosition);
        localMousePos.z = 0;
        draggableTransform.localPosition = localMousePos;
    }
}
