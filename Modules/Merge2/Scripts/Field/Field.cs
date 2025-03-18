using System;
using UnityEngine;

namespace Merge2
{
	public class Field : MonoBehaviour
	{
		[SerializeReference]
		protected SpriteRenderer fieldSpriteRenderer;

		[SerializeReference]
		protected DraggableChipLogic draggableChip;

		protected Cell[,] cells;

		[SerializeField]
		protected ChipData[] chipData;

		protected MergeableChipLogic mergeableLogic;
		protected FillContainerLogic fillContainerLogic;


		protected virtual void Awake()
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

			mergeableLogic = new MergeableChipLogic();
			fillContainerLogic = new FillContainerLogic();

			draggableChip.OnMerge += mergeableLogic.MergeChip;
			draggableChip.OnMerge += fillContainerLogic.Fill;

			CreateCells();
			TestFillField();
		}

		private void CreateChip(int x, int y, ChipData chipData)
		{
			Chip chip = ChipFactory.CreateChip(cells[x, y], chipData);
			chip.SendTrigger(Chip.AnimatorTrigger.Spawn);
		}

		private void TestFillField()
		{
			(int, int)[] positions = new (int, int)[]
			{
				(2, 2),
				(2, 1),
				(2, 2),
				(1, 2),
				(3, 2),
				(1, 4),
				(0, 0),
				(0, 2)
			};
			for (int i = 0; i < chipData.Length; i++)
			{
				CreateChip(positions[i].Item1, positions[i].Item2, chipData[i]);
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
					cell.Init(new Vector2Int(i, j));
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
				draggableChip.OnDragEnd(null);
				return;
			}
			Cell cell = cells[cellPos.x, cellPos.y];
			cell.OnDragEnd(position);
			draggableChip.OnDragEnd(cell);
		}

		public void OnDrag(Vector2 position, Vector3 worldPosition)
		{
			Vector2Int cellPos = Vector2Int.FloorToInt(position);

			draggableChip.OnDrag(worldPosition);
			if (!IsValidCellPos(cellPos))
			{
				return;
			}
			Cell cell = cells[cellPos.x, cellPos.y];
			cell.OnDrag(position);
		}

		public Cell FindNearestFreeCell(Vector2Int cellPos)
		{
			int rows = cells.GetLength(0);
			int cols = cells.GetLength(1);

			// Напрямки руху: вправо, вниз, вліво, вгору
			int[] dx = { 0, 1, 0, -1 };
			int[] dy = { 1, 0, -1, 0 };

			int x = cellPos.x, y = cellPos.y;
			int step = 1; // Довжина кроку в поточному напрямку
			int direction = 0; // Індекс напрямку
			int lengthChange = 0; // Кожні два повороти довжина збільшується

			while (step < rows * cols) // Не більше ніж вся матриця
			{
				for (int i = 0; i < step; i++) // Рух у поточному напрямку
				{
					x += dx[direction];
					y += dy[direction];

					// Перевіряємо, чи не виходимо за межі
					if (x >= 0 && x < rows && y >= 0 && y < cols)
					{
						if (cells[x, y].Chip == null) // Якщо знайшли вільну клітинку
							return cells[x, y];
					}
				}

				// Зміна напрямку (право → вниз → ліво → вгору)
				direction = (direction + 1) % 4;
				lengthChange++;

				// Кожні два напрямки збільшуємо довжину руху
				if (lengthChange % 2 == 0)
					step++;
			}

			return null; // Якщо немає вільних клітинок
		}
	}
}
