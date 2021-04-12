using UnityEngine;
using System.Collections.Generic;

namespace Utility.Development
{
    public class RandomDungeonGenerator : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;
        [SerializeField]
        private int totalRoomCount;
        #endregion

        #region Constants
        public const int NO_ROOM = 0;
        public const int HAS_ROOM = 1;
        #endregion

        #region GetRoomMatrix
        public int[,] GetRoomMatrix()
        {
            int[,] roomMatrix = new int[width, height];

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

                    for (int i = 0; i < oldOutestRoomIndices.Length; i++)
                    {
                        Vector2Int[] neighbours = GetAvailableNeighbours
                                                 (oldOutestRoomIndices[i].x,
                                                  oldOutestRoomIndices[i].y,
                                                  roomMatrix);

                        foreach (Vector2Int neighbour in neighbours)
                        {
                            if (generatedRoomCount == totalRoomCount)
                            {
                                break;
                            }
                            //Guarrantee that at least one outest room index will be stored.
                            if (outestRoomIndices.Count == 0)
                            {
                                SetCurrentRoomInRoomMatrix();
                            }
                            else if (Random.Range(0, 2) == 1)
                            {
                                SetCurrentRoomInRoomMatrix();
                            }

                            void SetCurrentRoomInRoomMatrix()
                            {
                                roomMatrix[neighbour.x, neighbour.y] = HAS_ROOM;
                                generatedRoomCount++;
                                outestRoomIndices.Add(neighbour);
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

        #region OnValidate
        private void OnValidate()
        {
            if (width < 1)
            {
                width = 1;
            }
            if (height < 1)
            {
                height = 1;
            }
            if (totalRoomCount < 1)
            {
                totalRoomCount = 1;
            }
            if (totalRoomCount > width * height)
            {
                totalRoomCount = width * height;
                Debug.LogWarning($"Maximum total room count is width x height ({width * height}).");
            }
        }
        #endregion
    }
}
