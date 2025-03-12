using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public static class Pathfinding
    {
        public static List<BattleCell> FindPath(BattleCell[,] grid, BattleCell start, List<BattleCell> targets)
        {
            if (targets.Count == 0)
            {
                return null;
            }

            List<BattleCell> openList = new List<BattleCell>();
            HashSet<BattleCell> closedList = new HashSet<BattleCell>();
            Dictionary<BattleCell, BattleCell> cameFrom = new Dictionary<BattleCell, BattleCell>();
            Dictionary<BattleCell, float> gCost = new Dictionary<BattleCell, float>();
            Dictionary<BattleCell, float> fCost = new Dictionary<BattleCell, float>();

            openList.Add(start);
            gCost[start] = 0;
            fCost[start] = GetBestTargetDistance(start, targets);

            BattleCell bestTarget = null;
            List<BattleCell> bestPath = null;
            float bestCost = float.MaxValue;

            while (openList.Count > 0)
            {
                BattleCell current = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (fCost[openList[i]] < fCost[current] || (fCost[openList[i]] == fCost[current] && gCost[openList[i]] < gCost[current]))
                    {
                        current = openList[i];
                    }
                }

                openList.Remove(current);
                closedList.Add(current);

                if (targets.Contains(current))
                {
                    float currentCost = gCost[current];
                    if (currentCost < bestCost)
                    {
                        bestTarget = current;
                        bestPath = RetracePath(cameFrom, start, current);
                        bestCost = currentCost;
                    }
                }

                foreach (BattleCell neighbor in GetNeighbors(grid, current))
                {
                    if (!neighbor.IsAvailableCell() || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    float tentativeGCost = gCost[current] + (IsDiagonal(current, neighbor) ? 1.5f : 1f);
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                    else if (tentativeGCost >= gCost[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + GetBestTargetDistance(neighbor, targets);
                }
            }

            if (bestPath != null)
            {
                return bestPath;
            }

            Debug.LogWarning($"Pathfinding: Path not found from {start.CellPos} to any of the targets.");
            return null; // Шлях не знайдено
        }

        private static List<BattleCell> RetracePath(Dictionary<BattleCell, BattleCell> cameFrom, BattleCell start, BattleCell end)
        {
            List<BattleCell> path = new List<BattleCell>();
            BattleCell current = end;

            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Reverse();
            return path;
        }

        private static List<BattleCell> GetNeighbors(BattleCell[,] grid, BattleCell cell)
        {
            List<BattleCell> neighbors = new List<BattleCell>();

            int x = cell.CellPos.x;
            int y = cell.CellPos.y;

            // Додаємо горизонтальні та вертикальні сусіди
            if (x > 0) neighbors.Add(grid[x - 1, y]);
            if (x < grid.GetLength(0) - 1) neighbors.Add(grid[x + 1, y]);
            if (y > 0) neighbors.Add(grid[x, y - 1]);
            if (y < grid.GetLength(1) - 1) neighbors.Add(grid[x, y + 1]);

            // Додаємо діагональні сусіди
            if (x > 0 && y > 0) neighbors.Add(grid[x - 1, y - 1]);
            if (x > 0 && y < grid.GetLength(1) - 1) neighbors.Add(grid[x - 1, y + 1]);
            if (x < grid.GetLength(0) - 1 && y > 0) neighbors.Add(grid[x + 1, y - 1]);
            if (x < grid.GetLength(0) - 1 && y < grid.GetLength(1) - 1) neighbors.Add(grid[x + 1, y + 1]);

            return neighbors;
        }

        public static float GetBestTargetDistance(BattleCell cell, List<BattleCell> targets)
        {
            float bestDistance = float.MaxValue;
            foreach (var target in targets)
            {
                float distance = GetManhattanDistance(cell, target);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                }
            }
            return bestDistance;
        }
        public static int GetManhattanDistance(BattleCell cellA, BattleCell cellB)
        {
            int dstX = Mathf.Abs(cellA.CellPos.x - cellB.CellPos.x);
            int dstY = Mathf.Abs(cellA.CellPos.y - cellB.CellPos.y);
            return dstX + dstY;
        }
        private static bool IsDiagonal(BattleCell cellA, BattleCell cellB)
        {
            return cellA.CellPos.x != cellB.CellPos.x && cellA.CellPos.y != cellB.CellPos.y;
        }
    }
}
