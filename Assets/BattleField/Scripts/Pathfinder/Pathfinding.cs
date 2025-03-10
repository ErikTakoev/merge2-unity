using System.Collections.Generic;
using UnityEngine;

namespace BattleField
{
    public static class Pathfinding
    {
        public static List<BattleCell> FindPath(BattleCell[,] grid, BattleCell start, List<BattleCell> targets)
        {
            Queue<BattleCell> queue = new Queue<BattleCell>();
            Dictionary<BattleCell, BattleCell> cameFrom = new Dictionary<BattleCell, BattleCell>();
            HashSet<BattleCell> visited = new HashSet<BattleCell>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                BattleCell current = queue.Dequeue();

                if (targets.Contains(current) && !current.IsReserved)
                {
                    return RetracePath(cameFrom, start, current);
                }

                foreach (BattleCell neighbor in GetNeighbors(grid, current))
                {
                    if (!neighbor.IsAvailableCell() || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

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
            var neighbors = new List<BattleCell>();
            var x = cell.CellPos.x;
            var y = cell.CellPos.y;
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    if (x + i >= 0 && x + i < width && y + j >= 0 && y + j < height) neighbors.Add(grid[x + i, y + j]);
                }
            }

            return neighbors;
        }

        public static int GetManhattanDistance(BattleCell cellA, BattleCell cellB)
        {
            int dstX = Mathf.Abs(cellA.CellPos.x - cellB.CellPos.x);
            int dstY = Mathf.Abs(cellA.CellPos.y - cellB.CellPos.y);
            return dstX + dstY;
        }
    }
}
