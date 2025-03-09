using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public static class Pathfinding
    {
        public static List<BattleCell> FindPath(BattleCell[,] grid, BattleCell start, BattleCell target)
        {
            List<BattleCell> openList = new List<BattleCell>();
            HashSet<BattleCell> closedList = new HashSet<BattleCell>();
            Dictionary<BattleCell, BattleCell> cameFrom = new Dictionary<BattleCell, BattleCell>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                BattleCell currentCell = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (GetFCost(openList[i], target) < GetFCost(currentCell, target))
                    {
                        currentCell = openList[i];
                    }
                }

                openList.Remove(currentCell);
                closedList.Add(currentCell);

                if (currentCell == target)
                {
                    return RetracePath(cameFrom, start, target);
                }

                foreach (BattleCell neighbor in GetNeighbors(grid, currentCell))
                {
                    if ((!neighbor.IsAvailableCell() && neighbor != target) || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = GetDistance(currentCell, neighbor);
                    if (newMovementCostToNeighbor < GetDistance(currentCell, neighbor) || !openList.Contains(neighbor))
                    {
                        cameFrom[neighbor] = currentCell;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private static List<BattleCell> RetracePath(Dictionary<BattleCell, BattleCell> cameFrom, BattleCell start, BattleCell end)
        {
            List<BattleCell> path = new List<BattleCell>();
            BattleCell currentCell = end;

            while (currentCell != start)
            {
                path.Add(currentCell);
                currentCell = cameFrom[currentCell];
            }
            path.RemoveAt(0);
            path.Reverse();
            return path;
        }

        private static List<BattleCell> GetNeighbors(BattleCell[,] grid, BattleCell cell)
        {
            List<BattleCell> neighbors = new List<BattleCell>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = cell.CellPos.x + x;
                    int checkY = cell.CellPos.y + y;

                    if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1))
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        public static int GetDistance(BattleCell cellA, BattleCell cellB)
        {
            // Обчислюємо різницю по осі X між двома клітинками
            int dstX = Mathf.Abs(cellA.CellPos.x - cellB.CellPos.x);
            
            // Обчислюємо різницю по осі Y між двома клітинками
            int dstY = Mathf.Abs(cellA.CellPos.y - cellB.CellPos.y);

            // Враховуємо різні розміри клітинок по осях X та Y
            float cellWidth = cellA.Width;
            float cellHeight = cellA.Height;

            // Якщо різниця по осі X більша за різницю по осі Y
            if (dstX > dstY)
            {
                // Повертаємо відстань, яка враховує діагональні та прямі кроки
                // 14 - це вартість діагонального кроку (приблизно √2 * 10), 10 - вартість прямого кроку
                return Mathf.RoundToInt(14 * dstY * cellHeight + 10 * (dstX - dstY) * cellWidth);
            }
            // Якщо різниця по осі Y більша або дорівнює різниці по осі X
            // Повертаємо відстань, яка враховує діагональні та прямі кроки
            return Mathf.RoundToInt(14 * dstX * cellWidth + 10 * (dstY - dstX) * cellHeight);
        }

        private static int GetFCost(BattleCell cell, BattleCell target)
        {
            return GetDistance(cell, target);
        }
    }
}