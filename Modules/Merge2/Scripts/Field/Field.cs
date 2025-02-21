using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeReference]
    SpriteRenderer fieldSpriteRenderer;

    [SerializeReference]
    DraggableChip draggableChip;

    Cell[,] cells;

    [SerializeField]
    ChipData[] chipData;

    MergeableChip mergeableLogic;
    FillContainerLogic fillContainerLogic;


    void Awake()
    {
        if (fieldSpriteRenderer == null)
        {
            Debug.LogError("FieldSpriteRenderer is empty");
            return;
        }

        if (!draggableChip)
        {
            Debug.Log("DraggableChip is empty");
            return;
        }

        mergeableLogic = new MergeableChip();
        fillContainerLogic = new FillContainerLogic();

        draggableChip.OnMerge += mergeableLogic.MergeChip;
        draggableChip.OnMerge += fillContainerLogic.ChipSuitableForContainer;

        CreateCells();
        TestFillField();
    }

    private void TestFillField()
    {
        int sizeX = cells.GetLength(0);
        int sizeY = cells.GetLength(1);
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                ChipData data = chipData[Random.Range(0, chipData.Length)];
                ChipFactory.CreateChip(cells[i, j], data);
            }
        }
    }

    private void CreateCells()
    {
        Vector2Int fieldSize = Vector2Int.FloorToInt(fieldSpriteRenderer.size);
        cells = new Cell[fieldSize.x, fieldSize.y];

        float halfX = fieldSize.x * 0.5f - 0.5f;
        float halfY = fieldSize.y * 0.5f + 0.5f;

        for (int j = 0; j < fieldSize.y; j++)
        {
            for (int i = 0; i < fieldSize.x; i++)
            {
                GameObject cellGO = new GameObject($"Cell[x:{i},y:{j}]", typeof(RectTransform), typeof(Cell));
                var rectTransform = cellGO.GetComponent<RectTransform>();
                rectTransform.SetParent(transform);
                rectTransform.sizeDelta = Vector2.one;
                rectTransform.localScale = Vector3.one;
                rectTransform.localPosition = new Vector3(i - halfX, fieldSize.y - j - halfY);

                var cell = cellGO.GetComponent<Cell>();
                cells[i, j] = cell;
            }
        }
    }

    bool IsValidCellPos(Vector2Int cellPos)
    {
        return cellPos.x >= 0 && cellPos.y >= 0 && cellPos.x < cells.GetLength(0) && cellPos.y < cells.GetLength(1);
    }

    public void OnTap(Vector2 position)
    {
        Vector2Int cellPos = Vector2Int.FloorToInt(position);

        if (!IsValidCellPos(cellPos))
        {
            return;
        }
        cells[cellPos.x, cellPos.y].OnTap(position);
    }

    public void OnDragStart(Vector2 position, Vector3 worldPosition)
    {
        Vector2Int cellPos = Vector2Int.FloorToInt(position);

        if (!IsValidCellPos(cellPos))
        {
            return;
        }

        Cell cell = cells[cellPos.x, cellPos.y];
        cell.OnDragStart(position);
        draggableChip.OnDragStart(cell, worldPosition);
    }

    public void OnDragEnd(Vector2 position, Vector3 worldPosition)
    {
        Vector2Int cellPos = Vector2Int.FloorToInt(position);

        if (!IsValidCellPos(cellPos))
        {
            return;
        }
        Cell cell = cells[cellPos.x, cellPos.y];
        cell.OnDragEnd(position);
        draggableChip.OnDragEnd(cell);
    }

    public void OnDrag(Vector2 position, Vector3 worldPosition)
    {
        Vector2Int cellPos = Vector2Int.FloorToInt(position);

        if (!IsValidCellPos(cellPos))
        {
            return;
        }
        Cell cell = cells[cellPos.x, cellPos.y];
        cell.OnDrag(position);
        draggableChip.OnDrag(worldPosition);
    }
}
