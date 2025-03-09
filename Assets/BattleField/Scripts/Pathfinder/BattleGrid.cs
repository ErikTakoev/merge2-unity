using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public BattleCell GetRandomSpawnPoint(bool isHero)
        {
            // Вибір випадкової клітинки для спавну
            int x = isHero ? 0 : width - 1;
            int y = UnityEngine.Random.Range(0, height);
            return cells[x, y];
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
                    bool isWalkable = !IsColliderAtPosition(cellPosition);
                    cells[x, y] = new BattleCell(x, y, isWalkable, cellWidth, cellHeight, cellPosition); // Встановлення кожної клітинки з її розмірами та позицією
                }
            }
        }

        bool IsColliderAtPosition(Vector2 position)
        {
            // Перевірка наявності коллайдера в заданій позиції
            Collider2D hitColliders = Physics2D.OverlapBox(position, new Vector2(cellWidth / 2, cellHeight / 2), 0f);
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
                    Gizmos.color = cells[x, y].IsAvailableCell() ? Color.green : Color.red; // Колір залежить від властивості клітинки
                    Gizmos.DrawWireCube(cellPosition, new Vector3(cells[x, y].Width - 0.01f, cells[x, y].Height - 0.01f, 0));
                }
            }
        }

        public async void FindPathAsync(BattleCell startCell, BattleCell targetCell, Action<List<BattleCell>> callback)
        {
            // Виконання пошуку шляху в паралельному потоці
            List<BattleCell> path = await Task.Run(() => Pathfinding.FindPath(cells, startCell, targetCell));

            // Виклик колбеку з результатом
            callback?.Invoke(path);
        }
    }
}