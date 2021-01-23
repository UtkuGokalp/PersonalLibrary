using System.Collections;
using System.Collections.Generic;

namespace Utility.Inventory
{
    public class Inventory<TItemBase> : IEnumerable<TItemBase>
    {
        #region Variables
        private readonly List<List<TItemBase>> inventory;
        private readonly IEqualityComparer<TItemBase> itemEqualityComparer;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new inventory and initializes it with the given starting items where key is the item and value is the item count.
        /// </summary>
        public Inventory(IEqualityComparer<TItemBase> itemEqualityComparer, params KeyValuePair<TItemBase, int>[] startingItems)
        {
            this.itemEqualityComparer = itemEqualityComparer;
            inventory = new List<List<TItemBase>>();
            foreach (KeyValuePair<TItemBase, int> pair in startingItems)
            {
                AddItem(pair.Key, pair.Value);
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Returns the item at the given index.
        /// </summary>
        public TItemBase this[int index]
        {
            get
            {
                return inventory[index][inventory[index].Count - 1];
            }
        }
        #endregion

        #region AddItem
        /// <summary>
        /// Adds item by given amount.
        /// </summary>
        public void AddItem(TItemBase item, int amount = 1)
        {
            if (ContainsItem(item, out int index, out int _))
            {
                for (int i = 0; i < amount; i++)
                {
                    inventory[index].Add(item);
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    inventory.Add(new List<TItemBase> { item });
                }
            }
        }
        #endregion

        #region RemoveItem
        /// <summary>
        /// Removes item by given amount if exists. Returns false otherwise.
        /// </summary>
        public bool RemoveItem(TItemBase item, int amount = 1)
        {
            if (ContainsItem(item, out int index, out int _))
            {
                //Reverse looping because removing items from the list also changes the index
                //which leads to IndexOutOfRangeException being thrown.
                int removedAmount = 0;
                for (int i = inventory[index].Count; inventory[index].Count > 0 && removedAmount < amount; i--)
                {
                    inventory[index].RemoveAt(inventory[index].Count - 1);
                    removedAmount++;
                }

                if (inventory[index].Count == 0)
                {
                    inventory.RemoveAt(index);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at the given index by amount if exists. Returns false otherwise.
        /// </summary>
        public bool RemoveItem(int index, int amount = 1)
        {
            if (inventory.Count == 0)
            {
                return false;
            }
            if (index > inventory.Count - 1)
            {
                throw new System.IndexOutOfRangeException();
            }
            return RemoveItem(this[index], amount);
        }
        #endregion

        #region GetItemCount
        /// <summary>
        /// Returns the given item count.
        /// </summary>
        public int GetItemCount(TItemBase item)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (itemEqualityComparer.Equals(inventory[i][0], item))
                {
                    return inventory[i].Count;
                }
            }
            return 0;
        }

        public int GetItemCount(int index)
        {
            if (index <= inventory.Count - 1)
            {
                return inventory[index].Count;
            }
            return 0;
        }
        #endregion

        #region ContainsItem
        /// <summary>
        /// Returns whether the given item exists or not and sets the index and the count varible if exists. Index is set to -1 and count to 0 if the item doesn't exist.
        /// </summary>
        public bool ContainsItem(TItemBase item, out int index, out int count)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (itemEqualityComparer.Equals(inventory[i][0], item))
                {
                    index = i;
                    count = inventory[i].Count;
                    return true;
                }
            }
            index = -1;
            count = 0;
            return false;
        }
        #endregion

        #region GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        public IEnumerator<TItemBase> GetEnumerator()
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                yield return inventory[i][0];
            }
        }
        #endregion
    }
}
