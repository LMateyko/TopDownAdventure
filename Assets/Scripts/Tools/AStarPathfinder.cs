using System;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure.Tools
{
    public class AStarPathfinder
    {
        // A structure to hold the necessary parameters
        private struct Cell
        {
            // Row and Column index of its parent
            // Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1
            public int parentX, parentY;
            // f = g + h
            public double finalCost, gridCost, huristic;
        }

        static public List<Vector3Int> AStarSearch(int[,] m_searchGrid, Vector3Int startTile, Vector3Int targetTile, out Dictionary<Vector2Int, double> pathReport)
        {
            pathReport = new Dictionary<Vector2Int, double>();

            // Start and End tile are the same
            if (startTile == targetTile)
            {
                return null;
            }

            if (!IsValidSquareInGrid(m_searchGrid, startTile.x, startTile.y) || !IsValidSquareInGrid(m_searchGrid, targetTile.x, targetTile.y))
            {
                return null;
            }

            // Target is within a wall or hazard
            if (m_searchGrid[startTile.x, startTile.y] != 0
                || m_searchGrid[targetTile.x, targetTile.y] != 0)
            {
                return null;
            }


            pathReport.Clear();
            int columns = m_searchGrid.GetLength(0);
            int rows = m_searchGrid.GetLength(1);

            bool[,] closedList = new bool[columns, rows];

            Cell[,] cellDetails = new Cell[columns, rows];
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    cellDetails[x, y].finalCost = double.MaxValue;
                    cellDetails[x, y].gridCost = double.MaxValue;
                    cellDetails[x, y].huristic = double.MaxValue;
                    cellDetails[x, y].parentX = -1;
                    cellDetails[x, y].parentY = -1;
                }
            }

            int currentX = startTile.x;
            int currentY = startTile.y;

            cellDetails[currentX, currentY].finalCost = 0.0;
            cellDetails[currentX, currentY].gridCost = 0.0;
            cellDetails[currentX, currentY].huristic = 0.0;
            cellDetails[currentX, currentY].parentX = currentX;
            cellDetails[currentX, currentY].parentY = currentY;

            // Sorted set sorted by a double (finalCost) with the X,Y coordinates as a Vector2Int
            SortedSet<(double, Vector2Int)> openList = new SortedSet<(double, Vector2Int)>(
                Comparer<(double, Vector2Int)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));

            openList.Add((0.0, new Vector2Int(currentX, currentY)));

            while (openList.Count > 0)
            {
                // Removed the lowest value in the set for processing
                (double finalCost, Vector2Int position) pair = openList.Min;
                openList.Remove(pair);

                currentX = pair.position.x;
                currentY = pair.position.y;

                closedList[currentX, currentY] = true;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if ((i != 0 && j != 0) || (i == 0 && j == 0))
                            continue;

                        //if (i == 0 && j == 0)
                        //    continue;

                        int newX = currentX + i;
                        int newY = currentY + j;

                        // Check if we are still within the grid
                        if (IsValidSquareInGrid(m_searchGrid, newX, newY))
                        {
                            // Check if we have reached our target tile
                            if (targetTile.x == newX && targetTile.y == newY)
                            {
                                cellDetails[newX, newY].parentX = currentX;
                                cellDetails[newX, newY].parentY = currentY;
                                return TracePath(columns, rows, cellDetails, targetTile);
                            }

                            if (!closedList[newX, newY] && IsSquareOpenInGrid(m_searchGrid, newX, newY))
                            {
                                // Calculate the new values for this path
                                double newGridCost = cellDetails[currentX, currentY].gridCost + 1.0;
                                double newHeuristicValue = CalculateManhatanDistance(newX, newY, targetTile);
                                double newFinalValue = newGridCost + newHeuristicValue;

                                // If the final value for this position is lower than what we have already found, display result
                                if (cellDetails[newX, newY].finalCost == double.MaxValue || cellDetails[newX, newY].finalCost > newFinalValue)
                                {
                                    var newPos = new Vector2Int(newX, newY);

                                    if (pathReport.ContainsKey(newPos))
                                        pathReport[newPos] = Math.Round(newFinalValue);
                                    else
                                        pathReport.Add(newPos, Math.Round(newFinalValue));

                                    cellDetails[newX, newY].finalCost = newFinalValue;
                                    cellDetails[newX, newY].gridCost = newGridCost;
                                    cellDetails[newX, newY].huristic = newHeuristicValue;
                                    cellDetails[newX, newY].parentX = currentX;
                                    cellDetails[newX, newY].parentY = currentY;

                                    openList.Add((newFinalValue, newPos));
                                }
                            }
                        }
                    }
                }
            }

            // Invalid Path
            return null;
        }

        static private bool IsValidSquareInGrid(int[,] m_searchGrid, int x, int y)
        {
            return (x >= 0) && (y >= 0) && (x < m_searchGrid.GetLength(0)) && (y < m_searchGrid.GetLength(1));
        }

        static private bool IsSquareOpenInGrid(int[,] m_searchGrid, int x, int y)
        {
            return m_searchGrid[x, y] == 0;
        }

        static private double CalculateManhatanDistance(int currentX, int currentY, Vector3Int goalPos)
        {
            return Math.Abs(currentX - goalPos.x) + Math.Abs(currentY - goalPos.y);
        }

        static private List<Vector3Int> TracePath(int columns, int rows, Cell[,] cellDetails, Vector3Int targetPosition)
        {
            List<Vector3Int> results = new List<Vector3Int>();

            int currentX = targetPosition.x;
            int currentY = targetPosition.y;

            var currentPosition = Vector3Int.zero;

            while (cellDetails[currentX, currentY].parentX != currentX
                || cellDetails[currentX, currentY].parentY != currentY)
            {
                
                currentPosition.x = currentX;
                currentPosition.y = currentY;

                results.Insert(0, currentPosition);

                int newX = cellDetails[currentX, currentY].parentX;
                int newY = cellDetails[currentX, currentY].parentY;

                currentX = newX;
                currentY = newY;
            }

            return results;
        }
    }

}

