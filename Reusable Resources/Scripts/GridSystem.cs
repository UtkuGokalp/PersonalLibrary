#nullable enable

using System;

namespace Utility.Development
{
    public class GridSystem<T>
    {
        #region Variables
        private readonly T[,] grid;
        public int X_Size { get; }
        public int Y_Size { get; }
        /// <summary>
        /// The default value for every grid element.
        /// </summary>
        public T DefaultElementValue { get; set; }
        public event TypeSafeEventHandler<GridSystem<T>, OnGridElementChangedEventArgs>? OnGridElementChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a grid with the specified size. Initializes the elements if an initialization method is provided.
        /// </summary>
        public GridSystem(int xSize, int ySize, /*TODO: Change this to T? instead of default! once Unity supports C# 9.0 or above.*/T defaultElementValue = default!, Func<(int x, int y), T>? initializationMethod = null)
        {
            X_Size = xSize;
            Y_Size = ySize;
            DefaultElementValue = defaultElementValue;
            grid = new T[X_Size, Y_Size];

            if (initializationMethod != null)
            {
                Foreach((index) =>
                {
                    grid[index.x, index.y] = initializationMethod.Invoke((index.x, index.y));
                });
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Getter calls GetValue() method. Setter calls SetValue().
        /// </summary>
        public T this[int xIndex, int yIndex]
        {
            get => GetValue(xIndex, yIndex);
            set => SetValue(xIndex, yIndex, value);
        }
        public T this[(int xIndex, int yIndex) index]
        {
            get => this[index.xIndex, index.yIndex];
            set => this[index.xIndex, index.yIndex] = value;
        }
        #endregion

        #region SetValue
        /// <summary>
        /// Sets the given index to the provided value. Returns true on success. Returns false otherwise.
        /// </summary>
        public bool SetValue(int x, int y, T value)
        {
            if (IndexIsValid(x, y))
            {
                T oldValue = GetValue(x, y);
                grid[x, y] = value;
                OnGridElementChanged?.Invoke(this, new OnGridElementChangedEventArgs(oldValue, GetValue(x, y)));
                return true;
            }
            return false;
        }
        #endregion

        #region GetValue
        /// <summary>
        /// Returns the value in the given address. Returns the default value if address is invalid.
        /// </summary>
        public T GetValue(int x, int y)
        {
            if (IndexIsValid(x, y))
            {
                return grid[x, y];
            }
            return DefaultElementValue;
        }
        #endregion

        #region IndexIsValid
        /// <summary>
        /// Returns true if given indexes are within the grid boundries. Returns false otherwise.
        /// </summary>
        public bool IndexIsValid(int x, int y)
        {
            if (x < 0 || x > X_Size - 1 ||
                y < 0 || y > Y_Size - 1)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Foreach
        /// <summary>
        /// Executes the given function on all members of the grid.
        /// </summary>
        public void Foreach(Action<(int x, int y)> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Given action was null.");
            }

            for (int y = 0; y < Y_Size; y++)
            {
                for (int x = 0; x < X_Size; x++)
                {
                    action.Invoke((x, y));
                }
            }
        }
        #endregion

        #region SetAllElementsToDefault
        /// <summary>
        /// Sets every grid element to the current default value.
        /// </summary>
        public void SetAllElementsToDefault()
        {
            Foreach((index) =>
            {
                grid[index.x, index.y] = DefaultElementValue;
            });
        }
        #endregion

        #region OnGridElementChangedEventArgs
        public class OnGridElementChangedEventArgs : EventArgs
        {
            #region Variables
            public T OldElement { get; }
            public T NewElement { get; }
            #endregion

            #region Constructor
            public OnGridElementChangedEventArgs(T oldElement, T newElement)
            {
                OldElement = oldElement;
                NewElement = newElement;
            }
            #endregion
        }
        #endregion
    }
}
