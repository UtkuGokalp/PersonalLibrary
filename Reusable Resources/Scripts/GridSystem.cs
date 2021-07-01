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
        /// Once this code gets ported to C# 9.0 or above, this property will go away and GetValue() method will have "return default" instead. Don't rely on the fact that you can modify this property.
        /// </summary>
        //This is also the reason why we have a default! for initializing this property. Remove when updating the code to C# 9.0 or above.
        public T DefaultValue { get; set; } = default!;
        public event TypeSafeEventHandler<GridSystem<T>, OnGridElementChangedEventArgs>? OnGridElementChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a grid with the specified size. Initializes the elements if an initialization method is provided.
        /// </summary>
        public GridSystem(int xSize, int ySize, Func<T>? initializationMethod = null)
        {
            X_Size = xSize;
            Y_Size = ySize;
            grid = new T[X_Size, Y_Size];

            if (initializationMethod != null)
            {
                Foreach((index) =>
                {
                    grid[index.x, index.y] = initializationMethod.Invoke();
                });
            }
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
            return DefaultValue;
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
