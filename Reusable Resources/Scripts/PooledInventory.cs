#nullable enable

using System.Collections;
using Utility.Development;
using System.Collections.Generic;

namespace Utility.Inventory
{
    public class PooledInventory<TItemBase> : IEnumerable<TItemBase>
    {
        #region Variables
        private readonly List<TItemBase>[] inventory;
        private readonly IEqualityComparer<TItemBase> itemEqualityComparer;
        public event TypeSafeEventHandler<PooledInventory<TItemBase>, OnItemAddedEventArgs>? OnItemAdded;
        public event TypeSafeEventHandler<PooledInventory<TItemBase>, OnItemRemovedEventArgs>? OnItemRemoved;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new inventory and initializes it with the given starting items where key is the item and value is the item count.
        /// </summary>
        /// <param name="startingItems">If length of startingItems array is bigger than inventorySize, only items that fit into the inventory are added to the inventory.</param>
        public PooledInventory(int inventorySize, IEqualityComparer<TItemBase> itemEqualityComparer, params KeyValuePair<TItemBase, int>[] startingItems)
        {
            this.itemEqualityComparer = itemEqualityComparer;
            inventory = new List<TItemBase>[inventorySize];
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = new List<TItemBase>();
            }

            if (startingItems.Length > inventory.Length)
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    AddItem(startingItems[i].Key, startingItems[i].Value);
                }
            }
            else
            {
                foreach (KeyValuePair<TItemBase, int> pair in startingItems)
                {
                    AddItem(pair.Key, pair.Value);
                }
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Returns the item at the given index.
        /// </summary>
        public TItemBase this[int index] => inventory[index][inventory[index].Count - 1];
        #endregion

        #region AddItem
        /// <summary>
        /// Adds item by given amount.
        /// </summary>
        public void AddItem(TItemBase item, int amount = 1)
        {
            if (amount < 1)
            {
                //Don't add the item if at least one item isn't being added.
                return;
            }
            int addedItemCount = 0;
            startAddingItems:
            if (ContainsItem(item, out int index, out int _))
            {
                for (int i = 0; i < amount; i++)
                {
                    inventory[index].Add(item);
                }
                addedItemCount += amount;
            }
            else
            {
                //Add one item, decrease amount by one and hand over
                //the addition logic to the piece of code which is
                //responsible for adding items that are already in the inventory.
                int? firstAvailableSlotIndex = GetFirstAvailableSlotIndex();
                if (firstAvailableSlotIndex.HasValue)
                {
                    inventory[firstAvailableSlotIndex.Value].Add(item);
                    addedItemCount++;
                    amount--;
                }
                else
                {
                    //If we don't have any slots left, don't bother trying to add any other items.
                    //Don't invoke OnItemAdded event because we didn't add any items, we didn't have any slots.
                    goto exitFunctionWithoutInvokingOnItemAddedEvent;
                }
                goto startAddingItems;
            }
            OnItemAdded?.Invoke(this, new OnItemAddedEventArgs(addedItemCount, index, item));
            exitFunctionWithoutInvokingOnItemAddedEvent:;
        }
        #endregion

        #region RemoveItem
        /// <summary>
        /// Removes item by given amount if exists. Returns false otherwise.
        /// </summary>
        public bool RemoveItem(TItemBase item, int amount = 1)
        {
            if (amount < 1)
            {
                //Don't remove the item if at least one item isn't being removed.
                return false;
            }
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
                OnItemRemoved?.Invoke(this, new OnItemRemovedEventArgs(removedAmount, index, item));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at the given index by amount if exists. Returns false otherwise.
        /// </summary>
        public bool RemoveItem(int index, int amount = 1)
        {
            if (!ContainsAnyItem())
            {
                return false;
            }
            if (index > inventory.Length - 1)
            {
                throw new System.IndexOutOfRangeException();
            }
            return RemoveItem(this[index], amount);
        }
        #endregion

        #region GetItem
        /// <summary>
        /// Returns the item at the given index.
        /// </summary>
        public TItemBase GetItem(int index) => this[index];
        #endregion

        #region GetItemIndex
        /// <summary>
        /// Returns the index of the given item if the item exists within inventory. Returns null otherwise.
        /// </summary>
        public int? GetItemIndex(TItemBase item)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (itemEqualityComparer.Equals(inventory[i][0], item))
                {
                    return i;
                }
            }
            return null;
        }
        #endregion

        #region GetItemCount
        /// <summary>
        /// Returns the count of the given item.
        /// </summary>
        public int GetItemCount(TItemBase item)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count != 0 && itemEqualityComparer.Equals(inventory[i][0], item))
                {
                    return inventory[i].Count;
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns the item count at the given index.
        /// </summary>
        public int GetItemCount(int index)
        {
            if (index <= inventory.Length - 1)
            {
                return inventory[index].Count;
            }
            return 0;
        }
        #endregion

        #region GetCountOfSlotsContainingItems
        /// <summary>
        /// Returns the number of total slots that has at least one item in it.
        /// </summary>
        public int GetCountOfSlotsContainingItems()
        {
            int totalItemCount = 0;
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count != 0)
                {
                    totalItemCount++;
                }
            }
            return totalItemCount;
        }
        #endregion

        #region ContainsItem
        /// <summary>
        /// Returns whether the given item exists or not and sets the index and the count varible if exists. Index is set to -1 and count to 0 if the item doesn't exist.
        /// </summary>
        public bool ContainsItem(TItemBase item, out int itemIndexInInventory, out int itemCount)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count != 0 && itemEqualityComparer.Equals(inventory[i][0], item))
                {
                    itemIndexInInventory = i;
                    itemCount = inventory[i].Count;
                    return true;
                }
            }
            itemIndexInInventory = -1;
            itemCount = 0;
            return false;
        }
        #endregion

        #region ContainsAnyItem
        /// <summary>
        /// Returns whether or not this inventory has any item.
        /// </summary>
        public bool ContainsAnyItem()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count != 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetInventorySize
        /// <summary>
        /// Returns the number of slots this inventory has.
        /// </summary>
        public int GetInventorySize() => inventory.Length;
        #endregion

        #region GetFirstAvailableSlotIndex
        /// <summary>
        /// Returns the first index of the slot that doesn't have any items in it.
        /// </summary>
        private int? GetFirstAvailableSlotIndex()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count == 0)
                {
                    return i;
                }
            }
            return null;
        }
        #endregion

        #region GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        public IEnumerator<TItemBase> GetEnumerator()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].Count > 0)
                {
                    //Returning first item will always work because when removing an item,
                    //it is being removed from the end of the slot. So if the count is more than 0,
                    //first item in the slot will always exist.
                    yield return inventory[i][0];
                }
                else
                {
                    continue;
                }
            }
        }
        #endregion

        #region OnItemAddedEventArgs
        public class OnItemAddedEventArgs : System.EventArgs
        {
            #region Variables
            public int AddedItemCount { get; }
            public int IndexOfAddedItem { get; }
            public TItemBase AddedItem { get; }
            #endregion

            #region Constructor
            public OnItemAddedEventArgs(int addedItemCount, int indexOfAddedItem, TItemBase addedItem)
            {
                IndexOfAddedItem = indexOfAddedItem;
                AddedItemCount = addedItemCount;
                AddedItem = addedItem;
            }
            #endregion
        }
        #endregion

        #region OnItemRemovedEventArgs
        public class OnItemRemovedEventArgs : System.EventArgs
        {
            #region Variables
            public int RemovedItemCount { get; }
            public int IndexOfRemovedItem { get; }
            public TItemBase RemovedItem { get; }
            #endregion

            #region Constructor
            public OnItemRemovedEventArgs(int removedItemCount, int indexOfRemovedItem, TItemBase removedItem)
            {
                IndexOfRemovedItem = indexOfRemovedItem;
                RemovedItemCount = removedItemCount;
                RemovedItem = removedItem;
            }
            #endregion
        }
        #endregion
    }
}
