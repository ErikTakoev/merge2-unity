using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace BattleField
{

	public class BattleGrid : MonoBehaviour
	{
		public int width = 10; // Ширина сітки
		public int height = 10; // Висота сітки
		public float cellWidth = 1f; // Ширина клітинки
		public float cellHeight = 1f; // Висота клітинки

		[SerializeField]
		private BattleCell[,] cells; // Масив клітинок

		void Awake()
		{
			GenerateIsWalkableValues();
		}

		public BattleCell GetRandomSpawnPoint(BattleUnit heroBoss)
		{
			if (heroBoss == null)
			{
				return cells[0, height / 2];
			}
			var heroBossPosX = heroBoss.Cell.CellPos.x;
			(int, int) minMaxRandom;
			minMaxRandom.Item1 = heroBossPosX - 5;
			minMaxRandom.Item1 = Math.Max(minMaxRandom.Item1, 0);
			minMaxRandom.Item2 = minMaxRandom.Item1 + 2;

			int x, y;
			while (true)
			{
				x = UnityEngine.Random.Range(minMaxRandom.Item1, minMaxRandom.Item2);
				y = UnityEngine.Random.Range(0, height);

				BattleCell cell = cells[x, y];
				if (cell != null && cell.IsAvailableCell())
				{
					return cell;
				}
			}
		}

		public void GenerateIsWalkableValues()
		{
			// Ініціалізація масиву клітинок
			cells = new BattleCell[width, height];

			// Вирахування зміщення для центрування сітки
			Vector3 offset = new Vector3(-width * cellWidth / 2, -height * cellHeight / 2, 0);

			// Заповнення масиву клітинок (можна встановити різні значення або властивості)
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Vector3 cellPosition = transform.position + offset + new Vector3(x * cellWidth + cellWidth / 2, y * cellHeight + cellHeight / 2, 0);
					cellPosition.z = cellPosition.y * 0.01f;
					bool isWalkable = !IsColliderAtPosition(cellPosition);
					cells[x, y] = new BattleCell(x, y, isWalkable, cellWidth, cellHeight, cellPosition); // Встановлення кожної клітинки з її розмірами та позицією
				}
			}
		}

		bool IsColliderAtPosition(Vector2 position)
		{
			// Перевірка наявності коллайдера в заданій позиції
			Collider2D hitColliders = Physics2D.OverlapBox(position, new Vector2(cellWidth / 2, cellHeight / 2), 0f);
			if (hitColliders != null && gameObject.layer != hitColliders.gameObject.layer)
				return false;
			return hitColliders != null;
		}

		void OnDrawGizmos()
		{
			if (cells == null)
			{
				GenerateIsWalkableValues();
			}
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Vector3 cellPosition = cells[x, y].WorldPosition;
					var cell = cells[x, y];
					if (!cell.IsAvailableCell())
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.green;
					}
					Gizmos.DrawWireCube(cellPosition, new Vector3(cells[x, y].Width - 0.01f, cells[x, y].Height - 0.01f, 0));

					// Додаємо відображення координат (x, y)
					UnityEditor.Handles.Label(cellPosition + new Vector3(-cellWidth / 4, -cellHeight / 4, 0), $"({x}, {y})");
				}
			}
		}

		public async void FindPathAsync(BattleCell startCell, List<BattleCell> targets, Action<List<BattleCell>> callback)
		{
			// Виконання пошуку шляху в паралельному потоці
			List<BattleCell> path = await Task.Run(() => Pathfinding.FindPath(cells, startCell, targets));

			// Виклик колбеку з результатом
			callback?.Invoke(path);
		}

		public BattleCell GetCell(int x, int y)
		{
			if (x < 0 || x >= width || y < 0 || y >= height)
			{
				return null;
			}

			return cells[x, y];
		}
	}
}
