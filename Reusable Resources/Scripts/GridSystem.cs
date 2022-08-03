#nullable enable

using System;

namespace Utility.Development
{
    public class GridSystem<T>
    {
        #region Variables
        private readonly T?[,] grid;
        public int X_Size { get; }
        public int Y_Size { get; }
        /// <summary>
        /// The default value for every grid element.
        /// </summary>
        public T? DefaultElementValue { get; set; }
        public event TypeSafeEventHandler<GridSystem<T>, OnGridElementChangedEventArgs>? OnGridElementChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a grid with the specified size. Initializes the elements if an initialization method is provided.
        /// </summary>
        public GridSystem(int xSize, int ySize, T? defaultElementValue, Func<(int x, int y), T>? initializationMethod = null)
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
        public T? this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }
        /// <summary>
        /// Calls this[int, int] indexer. Removes the need to manually convert the tuple to two integers.
        /// </summary>
        public T? this[(int x, int y) index]
        {
            get => this[index.x, index.y];
            set => this[index.x, index.y] = value;
        }
        #endregion

        #region SetValue
        /// <summary>
        /// Sets the given index to the provided value. Returns true on success. Returns false otherwise.
        /// </summary>
        public bool SetValue(int x, int y, T? value)
        {
            if (IndexIsValid(x, y))
            {
                T? oldValue = GetValue(x, y);
                grid[x, y] = value;
                OnGridElementChanged?.Invoke(this, new OnGridElementChangedEventArgs(oldValue, GetValue(x, y)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calls SetValue(int, int, T) method. Removes the need to manually convert the tuple to two integers.
        /// </summary>
        public bool SetValue((int x, int y) index, T value) => SetValue(index.x, index.y, value);
        #endregion

        #region GetValue
        /// <summary>
        /// Returns the value in the given address. Returns the default value if address is invalid.
        /// </summary>
        public T? GetValue(int x, int y)
        {
            if (IndexIsValid(x, y))
            {
                return grid[x, y];
            }
            return DefaultElementValue;
        }

        /// <summary>
        /// Calls GetValue(int, int) method. Removes the need to manually convert the tuple to two integers.
        /// </summary>
        public T? GetValue((int x, int y) index) => GetValue(index.x, index.y);
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

        /// <summary>
        /// Calls IndexIsValid(int, int) method. Removes the need to manually convert the tuple to two integers.
        /// </summary>
        public bool IndexIsValid((int x, int y) index) => IndexIsValid(index.x, index.y);
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
            public T? OldElement { get; }
            public T? NewElement { get; }
            #endregion

            #region Constructor
            public OnGridElementChangedEventArgs(T? oldElement, T? newElement)
            {
                OldElement = oldElement;
                NewElement = newElement;
            }
            #endregion
        }
        #endregion
    }
}
