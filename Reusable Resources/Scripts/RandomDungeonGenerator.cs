using System;
using UnityEngine;
using System.Collections.Generic;

namespace Utility.Development
{
    public class RandomDungeonGenerator
    {
        #region Variables
        private int width;
        private int height;
        private int totalRoomCount;
        #endregion

        #region Constants
        public const int NO_ROOM = 0;
        public const int HAS_ROOM = 1;
        #endregion

        #region Constructor
        public RandomDungeonGenerator(int width, int height, int totalRoomCount)
        {
            if (width < 1)
            {
                throw new ArgumentException("Width cannot be less than 1.");
            }
            if (height < 1)
            {
                throw new ArgumentException("Height cannot be less than 1.");
            }
            if (totalRoomCount < 1)
            {
                throw new ArgumentException("Total room count cannot be less than 1.");
            }
            if (totalRoomCount > width * height)
            {
                throw new ArgumentException($"Maximum total room count is width x height ({width * height}).");
            }

            this.width = width;
            this.height = height;
            this.totalRoomCount = totalRoomCount;
        }
        #endregion

        #region GetRoomMatrix
        public int[,] GetRoomMatrix()
        {
            int[,] roomMatrix = new int[width, height];
            //C# native number generator is used here because this method might need to be multithreaded
            //which UnityEngine.Random.Range() does not support.
            System.Random randomNumberGenerator = new System.Random();

            if (totalRoomCount == width * height) //If we want all of the rooms to be filled
            {
                for (int x = 0; x < roomMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < roomMatrix.GetLength(1); y++)
                    {
                        roomMatrix[x, y] = HAS_ROOM;
                    }
                }
            }
            else
            {
                List<Vector2Int> outestRoomIndices = new List<Vector2Int>();
                int generatedRoomCount = 0;

                Vector2Int origin = new Vector2Int(width / 2, height / 2);
                roomMatrix[origin.x, origin.y] = HAS_ROOM;
                generatedRoomCount++;
                outestRoomIndices.Add(origin);

                Vector2Int[] oldOutestRoomIndices;

                while (generatedRoomCount < totalRoomCount)
                {
                    oldOutestRoomIndices = outestRoomIndices.ToArray();
                    outestRoomIndices.Clear();

                    //At this point we are stuck forever in this loop. This can happen because the algorithm
                    //depends on luck (which is not ideal). Calling the method again here and returning the value
                    //is going to cost memory but it is going to make this method functional provided that we have enough memory.
                    //Note: this method doesn't get stuck often, just from time to time so this solution can be seen as valid.
                    //Just from time to time: The algorithm showed that this line of code is executed rarely during the tests,
                    //even when this is most likely to be executed (which is totalRoomCount == width * height - (a small number))
                    if (oldOutestRoomIndices.Length == 0)
                    {
                        return GetRoomMatrix();
                    }

                    for (int i = 0; i < oldOutestRoomIndices.Length; i++)
                    {
                        if (generatedRoomCount == totalRoomCount)
                        {
                            break;
                        }

                        Vector2Int[] neighbours = GetAvailableNeighbours
                                                 (oldOutestRoomIndices[i].x,
                                                  oldOutestRoomIndices[i].y,
                                                  roomMatrix);
                        if (neighbours.Length == 0)
                        {
                            //If there are no neighbours we cannot create a new room anyways,
                            //just check out the next rooms.
                            continue;
                        }

                        neighbours.Shuffle();
                        int selectedNeighbourCount = randomNumberGenerator.Next(0, neighbours.Length) + 1;

                        for (int j = 0; j < selectedNeighbourCount; j++)
                        {
                            if (generatedRoomCount == totalRoomCount)
                            {
                                break;
                            }
                            else
                            {
                                if (neighbours.Length != 0)
                                {
                                    roomMatrix[neighbours[j].x, neighbours[j].y] = HAS_ROOM;
                                    generatedRoomCount++;
                                    outestRoomIndices.Add(neighbours[j]);
                                }
                            }
                        }
                    }
                }
            }

            return roomMatrix;
        }
        #endregion

        #region GetAvailableNeighbours
        private Vector2Int[] GetAvailableNeighbours(int x, int y, int[,] roomMatrix)
        {
            if (x < 0 || x >= GetRoomsRowCount() ||
                y < 0 || y >= GetRoomsColumnCount())
            {
                return null;
            }

            List<Vector2Int> neighbours = new List<Vector2Int>();

            if (x > 0)
            {
                if (roomMatrix[x - 1, y] == NO_ROOM)
                {
                    neighbours.Add(new Vector2Int(x - 1, y));
                }
            }
            if (x + 1 < GetRoomsRowCount())
            {
                if (roomMatrix[x + 1, y] == NO_ROOM)
                {
                    neighbours.Add(new Vector2Int(x + 1, y));
                }
            }
            if (y > 0)
            {
                if (roomMatrix[x, y - 1] == NO_ROOM)
                {
                    neighbours.Add(new Vector2Int(x, y - 1));
                }
            }
            if (y + 1 < GetRoomsColumnCount())
            {
                if (roomMatrix[x, y + 1] == NO_ROOM)
                {
                    neighbours.Add(new Vector2Int(x, y + 1));
                }
            }

            return neighbours.ToArray();

            int GetRoomsRowCount() => roomMatrix.GetLength(0);
            int GetRoomsColumnCount() => roomMatrix.GetLength(1);
        }
        #endregion
    }
}
