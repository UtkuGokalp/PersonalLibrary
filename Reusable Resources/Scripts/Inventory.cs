#nullable enable

using System.Collections;
using Utility.Development;
using System.Collections.Generic;

namespace Utility.Inventory
{
    public class Inventory<TItemBase> : IEnumerable<TItemBase>
    {
        #region Variables
        private InventoryItem?[] inventory;
        private IEqualityComparer<TItemBase> equalityComparer;
        public event TypeSafeEventHandler<Inventory<TItemBase>, OnItemAddedEventArgs>? OnItemAdded;
        public event TypeSafeEventHandler<Inventory<TItemBase>, OnItemRemovedEventArgs>? OnItemRemoved;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new inventory and initializes it with the given starting items where key is the item and value is the item count.
        /// </summary>
        /// <param name="startingItems">If length of startingItems array is bigger than inventorySize, only items that fit into the inventory are added to the inventory.</param>
        public Inventory(int inventorySize, IEqualityComparer<TItemBase> itemEqualityComparer, params KeyValuePair<TItemBase, int>[] startingItems)
        {
            inventory = new InventoryItem[inventorySize];
            equalityComparer = itemEqualityComparer;

            if (startingItems.Length <= inventory.Length)
            {
                for (int i = 0; i < startingItems.Length; i++)
                {
                    AddItem(startingItems[i].Key, startingItems[i].Value);
                }
            }
            else
            {
                //There are more starting items than the inventory slots.
                //Add only the ones the inventory can hold.
                for (int i = 0; i < inventory.Length; i++)
                {
                    AddItem(startingItems[i].Key, startingItems[i].Value);
                }
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
                if (index > inventory.Length - 1)
                {
                    throw new System.IndexOutOfRangeException();
                }
                if (inventory[index] == null)
                {
                    //TODO: When Unity supports C# 9.0 or higher,
                    //remove SlowWasEmptyException and just return null.
                    throw new SlotWasEmptyException();
                }
                return inventory[index]!.Item;
            }
        }
        #endregion

        #region AddItem
        /// <summary>
        /// Adds item by given amount.
        /// </summary>
        public void AddItem(TItemBase item, int amount = 1)
        {
            if (amount < 1)
            {
                //Don't add if it isn't at least one item to add.
                return;
            }
            if (ContainsItem(item, out int index, out _))
            {
                inventory[index]!.ItemCount += amount;
                OnItemAdded?.Invoke(this, new OnItemAddedEventArgs(inventory[index]!.ItemCount, index, item));
            }
            else
            {
                int? availableSlotIndex = GetFirstAvailableSlotIndex();
                if (availableSlotIndex != null)
                {
                    inventory[(int)availableSlotIndex] = new InventoryItem(item, amount);
                    OnItemAdded?.Invoke(this, new OnItemAddedEventArgs(amount, (int)availableSlotIndex, item));
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
            if (amount < 1)
            {
                //Don't remove if it isn't at least one item to remove.
                return false;
            }
            if (ContainsItem(item, out int itemIndex, out int itemCount))
            {
                if (inventory[itemIndex]!.ItemCount <= amount)
                {
                    inventory[itemIndex] = null;
                    OnItemRemoved?.Invoke(this, new OnItemRemovedEventArgs(amount, itemIndex, item));
                }
                else
                {
                    inventory[itemIndex]!.ItemCount = amount;
                    OnItemRemoved?.Invoke(this, new OnItemRemovedEventArgs(amount, itemIndex, item));
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
            if (index > inventory.Length - 1)
            {
                return false;
            }
            if (inventory[index] == null)
            {
                return false;
            }
            return RemoveItem(inventory[index]!.Item, amount);
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
                if (inventory[i] == null)
                {
                    continue;
                }
                if (equalityComparer.Equals(inventory[i]!.Item, item))
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
            int? index = GetItemIndex(item);
            if (index == null)
            {
                return 0;
            }
            return inventory[(int)index]!.ItemCount;
        }

        /// <summary>
        /// Returns the item count at the given index.
        /// </summary>
        public int GetItemCount(int index)
        {
            if (index > inventory.Length - 1)
            {
                return 0;
            }
            if (inventory[index] == null)
            {
                return 0;
            }
            return inventory[index]!.ItemCount;
        }
        #endregion

        #region GetCountOfSlotsContainingItems
        /// <summary>
        /// Returns the number of total slots that has at least one item in it.
        /// </summary>
        public int GetCountOfSlotsContainingItems()
        {
            int count = 0;
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null)
                {
                    count++;
                }
            }
            return count;
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
                if (inventory[i] == null)
                {
                    continue;
                }
                if (equalityComparer.Equals(inventory[i]!.Item, item))
                {
                    itemCount = inventory[i]!.ItemCount;
                    itemIndexInInventory = i;
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
                if (inventory[i] != null)
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
                if (inventory[i] == null)
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
                if (inventory[i] == null)
                {
                    continue;
                }
                yield return inventory[i]!.Item;
            }
        }
        #endregion

        #region InventoryItem
        public class InventoryItem
        {
            #region Variables
            public int ItemCount { get; set; }
            public TItemBase Item { get; set; }
            #endregion

            #region Constructor
            public InventoryItem(TItemBase item, int itemCount)
            {
                Item = item;
                ItemCount = itemCount;
            }
            #endregion
        }
        #endregion

        #region SlotWasEmptyException
        public class SlotWasEmptyException : System.Exception
        {
            public SlotWasEmptyException() { }
            public SlotWasEmptyException(string message) : base(message) { }
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
