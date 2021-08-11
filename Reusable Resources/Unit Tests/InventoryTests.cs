#nullable enable

using NUnit.Framework;
using Utility.Inventory;
using System.Collections.Generic;

namespace InventoryUpdate.Tests.InventoryTests
{
    public class InventoryTests
    {
        #region ItemBase
        private class ItemBase
        {
            public string name = string.Empty;
            public ItemBase() { }
            public ItemBase(string name) { this.name = name; }
        }
        #endregion

        #region ItemComparer
        private class ItemComparer : IEqualityComparer<ItemBase>
        {
            public bool Equals(ItemBase x, ItemBase y)
            {
                return x.name == y.name;
            }

            public int GetHashCode(ItemBase obj) => throw new System.NotImplementedException();
        }
        #endregion

        #region GetNewArrayOfStartingItems
        private KeyValuePair<ItemBase, int>[] GetNewArrayOfStartingItems()
        {
            return new KeyValuePair<ItemBase, int>[]
            {
                //Item counts are random numbers.
                new KeyValuePair<ItemBase, int>(new ItemBase("TestItem1"), 154),
                new KeyValuePair<ItemBase, int>(new ItemBase("TestItem2"), 22),
                new KeyValuePair<ItemBase, int>(new ItemBase("TestItem3"), 74),
                new KeyValuePair<ItemBase, int>(new ItemBase("TestItem4"), 864)
            };
        }
        #endregion

        #region Inventory_CreatesCorrectAmountOfSlots
        [Test]
        public void Inventory_CreatesCorrectAmountOfSlots()
        {
            int intendedSize = 1578; //Just a random number to act as the inventory size.
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(intendedSize, new ItemComparer());
            int inventorySize = inventory.GetInventorySize();

            Assert.AreEqual(intendedSize, inventorySize);
        }
        #endregion

        #region Inventory_DoesntAddStartingItems_StartingItemCountIsZero
        [Test]
        public void Inventory_DoesntAddStartingItems_StartingItemCountIsZero()
        {
            ItemBase item = new ItemBase("Item1");
            KeyValuePair<ItemBase, int> startingItem = new KeyValuePair<ItemBase, int>(item, 0);

            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer(), startingItem);

            Assert.IsFalse(inventory.ContainsItem(item, out _, out _));
        }
        #endregion

        #region Inventory_DoesntAddStartingItems_StartingItemCountIsLessThanZero
        [Test]
        public void Inventory_DoesntAddStartingItems_StartingItemCountIsLessThanZero()
        {
            ItemBase item = new ItemBase("Item1");
            KeyValuePair<ItemBase, int> startingItem = new KeyValuePair<ItemBase, int>(item, -1);

            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer(), startingItem);

            Assert.IsFalse(inventory.ContainsItem(item, out _, out _));
        }
        #endregion

        #region Inventory_AddItemDoesntAddItem_ItemAmountIsZero
        [Test]
        public void Inventory_AddItemDoesntAddItem_ItemAmountIsZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, 0);

            Assert.IsFalse(inventory.ContainsItem(item, out _, out _));
        }
        #endregion

        #region Inventory_AddItemDoesntAddItem_ItemAmountIsLessThanZero
        [Test]
        public void Inventory_AddItemDoesntAddItem_ItemAmountIsLessThanZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, -1);

            Assert.IsFalse(inventory.ContainsItem(item, out _, out _));
        }
        #endregion

        #region Inventory_AddsAllStartingItems_MoreSlotsThanStartingItemsCount
        [Test]
        public void Inventory_AddsAllStartingItems_MoreSlotsThanStartingItemsCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            //Set the items based on the startingItems.Length in case the array is ever changed in the future.
            int size = startingItems.Length + 1;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool hasAllItems = true;
            foreach (var startingItem in startingItems)
            {
                if (!inventory.ContainsItem(startingItem.Key, out _, out _))
                {
                    hasAllItems = false;
                    break;
                }
            }

            Assert.IsTrue(hasAllItems);
        }
        #endregion

        #region Inventory_AddsAllStartingItems_SameSlotCountAsStartingItemsCount
        [Test]
        public void Inventory_AddsAllStartingItems_SameSlotCountAsStartingItemsCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            int size = startingItems.Length;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool hasAllItems = true;
            foreach (var startingItem in startingItems)
            {
                if (!inventory.ContainsItem(startingItem.Key, out _, out _))
                {
                    hasAllItems = false;
                    break;
                }
            }

            Assert.IsTrue(hasAllItems);
        }
        #endregion

        #region Inventory_AddsStartingItemsThatInventoryHasSpaceFor_LessSlotCountThanStartingItemsCount
        [Test]
        public void Inventory_AddsStartingItemsThatInventoryHasSpaceFor_LessSlotCountThanStartingItemsCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            int size = startingItems.Length - 1;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool hasItemsThatShouldntBeInInventory = false;

            for (int i = 0; i < startingItems.Length; i++)
            {
                if (i > inventory.GetInventorySize() - 1)
                {
                    if (inventory.ContainsItem(startingItems[i].Key, out _, out _))
                    {
                        hasItemsThatShouldntBeInInventory = true;
                        break;
                    }
                }
            }

            Assert.IsFalse(hasItemsThatShouldntBeInInventory);
        }
        #endregion

        #region Inventory_AddsCorrectCountOfStartingItems_MoreSlotsThanStartingItemCount
        [Test]
        public void Inventory_AddsCorrectCountOfStartingItems_MoreSlotsThanStartingItemCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            int size = startingItems.Length + 1;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool addedCorrectAmountOfStartingItems = true;
            for (int i = 0; i < startingItems.Length; i++)
            {
                if (inventory.GetItemCount(i) != startingItems[i].Value)
                {
                    addedCorrectAmountOfStartingItems = false;
                }
            }

            Assert.IsTrue(addedCorrectAmountOfStartingItems);
        }
        #endregion

        #region Inventory_AddsCorrectCountOfStartingItems_SameAmountOfSlotsAsStartingItemCount
        [Test]
        public void Inventory_AddsCorrectCountOfStartingItems_SameAmountOfSlotsAsStartingItemCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            int size = startingItems.Length;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool addedCorrectAmountOfStartingItems = true;
            for (int i = 0; i < startingItems.Length; i++)
            {
                if (inventory.GetItemCount(i) != startingItems[i].Value)
                {
                    addedCorrectAmountOfStartingItems = false;
                }
            }

            Assert.IsTrue(addedCorrectAmountOfStartingItems);
        }
        #endregion

        #region Inventory_AddsCorrectCountOfStartingItems_LessSlotsThanStartingItemCount
        [Test]
        public void Inventory_AddsCorrectCountOfStartingItems_LessSlotsThanStartingItemCount()
        {
            var startingItems = GetNewArrayOfStartingItems();
            int size = startingItems.Length - 1;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer(), startingItems);

            bool addedCorrectAmountOfStartingItems = true;
            for (int i = 0; i < inventory.GetInventorySize(); i++)
            {
                if (inventory.GetItemCount(i) != startingItems[i].Value)
                {
                    addedCorrectAmountOfStartingItems = false;
                }
            }

            Assert.IsTrue(addedCorrectAmountOfStartingItems);
        }
        #endregion

        #region Inventory_IndexerReturnsCorrectItem
        [Test]
        public void Inventory_IndexerReturnsCorrectItem()
        {
            ItemComparer comparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, comparer);
            ItemBase firstItem = new ItemBase("Item 1");
            ItemBase secondItem = new ItemBase("Item 2");
            ItemBase thirdItem = new ItemBase("Item 3");

            inventory.AddItem(firstItem);
            inventory.AddItem(secondItem);
            inventory.AddItem(thirdItem);

            Assert.IsTrue(comparer.Equals(inventory[0], firstItem));
            Assert.IsTrue(comparer.Equals(inventory[1], secondItem));
            Assert.IsTrue(comparer.Equals(inventory[2], thirdItem));
        }
        #endregion

        #region Inventory_IndexerThrowsIndexOutOfRangeException_OutOfRangeIndexProvided
        [Test]
        public void Inventory_IndexerThrowsIndexOutOfRangeException_OutOfRangeIndexProvided()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(2, new ItemComparer());

            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                _ = inventory[2].name; //Access the indexer with an invalid index.
            });
        }
        #endregion

        #region Inventory_AddItemAddsItemInCorrectAmount_InventoryDidntContainItem_InventoryHasEnoughSlots_AmountWasDefault
        [Test]
        public void Inventory_AddItemAddsItemInCorrectAmount_InventoryDidntContainItem_InventoryHasEnoughSlots_AmountWasDefault()
        {
            ItemComparer comparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase itemToAdd = new ItemBase("Item1");

            int itemAmount = 1;
            inventory.AddItem(itemToAdd, itemAmount);

            Assert.IsTrue(comparer.Equals(inventory[0], itemToAdd));
            Assert.IsTrue(inventory.GetItemCount(itemToAdd) == itemAmount);
        }
        #endregion

        #region Inventory_AddItemAddsItemInCorrectAmount_InventoryContainedItem_InventoryHasEnoughSlots_AmountWasDefault
        [Test]
        public void Inventory_AddItemAddsItemInCorrectAmount_InventoryContainedItem_InventoryHasEnoughSlots_AmountWasDefault()
        {
            ItemComparer comparer = new ItemComparer();
            ItemBase itemToAdd = new ItemBase("Item1");
            KeyValuePair<ItemBase, int> startingItem = new KeyValuePair<ItemBase, int>(itemToAdd, 1);
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer(), startingItem);

            inventory.AddItem(itemToAdd);

            Assert.IsTrue(comparer.Equals(inventory[0], itemToAdd));
            Assert.IsTrue(inventory.GetItemCount(itemToAdd) == startingItem.Value + 1);
        }
        #endregion

        #region Inventory_AddItemDoesntAddItem_InventoryDidntHaveEnoughSlots_AmountWasDefault
        [Test]
        public void Inventory_AddItemDoesntAddItem_InventoryDidntHaveEnoughSlots_AmountWasDefault()
        {
            KeyValuePair<ItemBase, int> startingItem = new KeyValuePair<ItemBase, int>(new ItemBase("Item1"), 1);
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer(), startingItem);
            ItemBase itemToAdd = new ItemBase("Item2");

            inventory.AddItem(itemToAdd);

            Assert.IsFalse(inventory.ContainsItem(itemToAdd, out _, out _));
        }
        #endregion

        #region Inventory_RemoveItemDoesntRemoveItem_ItemAmountIsZero
        [Test]
        public void Inventory_RemoveItemDoesntRemoveItem_ItemAmountIsZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, 1);
            inventory.RemoveItem(item, 0);

            Assert.AreEqual(1, inventory.GetItemCount(item));
        }
        #endregion

        #region Inventory_RemoveItemDoesntRemoveItem_ItemAmountIsLessThanZero
        [Test]
        public void Inventory_RemoveItemDoesntRemoveItem_ItemAmountIsLessThanZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, 1);
            inventory.RemoveItem(item, -1);

            Assert.AreEqual(1, inventory.GetItemCount(item));
        }
        #endregion

        #region Inventory_RemoveItemReturnsFalse_ItemAmountIsZero
        [Test]
        public void Inventory_RemoveItemReturnsFalse_ItemAmountIsZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, 1);
            bool returnValue = inventory.RemoveItem(item, 0);

            Assert.IsFalse(returnValue);
        }
        #endregion

        #region Inventory_RemoveItemReturnsFalse_ItemAmountIsLessThanZero
        [Test]
        public void Inventory_RemoveItemReturnsFalse_ItemAmountIsLessThanZero()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item = new ItemBase("Item1");

            inventory.AddItem(item, 1);
            bool returnValue = inventory.RemoveItem(item, -1);

            Assert.IsFalse(returnValue);
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasMoreItemThanRemoveAmount_ItemVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasMoreItemThanRemoveAmount_ItemVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 100, removeAmount = 50;
            inventory.AddItem(item1, addAmount);
            inventory.RemoveItem(item1, removeAmount);

            Assert.AreEqual(addAmount - removeAmount, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasAsMuchAsRemoveAmount_ItemVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasAsMuchAsRemoveAmount_ItemVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int itemAmount = 50;
            inventory.AddItem(item1, itemAmount);
            inventory.RemoveItem(item1, itemAmount);

            Assert.AreEqual(0, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasLessItemThanRemoveAmount_ItemVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasLessItemThanRemoveAmount_ItemVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 50, removeAmount = 100;
            inventory.AddItem(item1, addAmount);
            inventory.RemoveItem(item1, removeAmount);

            Assert.AreEqual(0, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemDoesntChangeAnything_InventoryDoesntHaveTheItem_ItemVersion
        [Test]
        public void Inventory_RemoveItemDoesntChangeAnything_InventoryDoesntHaveTheItem_ItemVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 100, removeAmount = 50;
            inventory.AddItem(item1, addAmount);
            List<ItemBase> inventoryItemsBeforeRemoval = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                inventoryItemsBeforeRemoval.Add(item);
            }

            inventory.RemoveItem(item2, removeAmount);
            List<ItemBase> inventoryItemsAfterRemoval = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                inventoryItemsAfterRemoval.Add(item);
            }

            bool beforeAndAfterListsAreEqual = true;
            for (int i = 0; i < inventoryItemsBeforeRemoval.Count; i++)
            {
                if (inventoryItemsBeforeRemoval[i] != inventoryItemsAfterRemoval[i])
                {
                    beforeAndAfterListsAreEqual = false;
                    break;
                }
            }

            Assert.IsTrue(beforeAndAfterListsAreEqual);
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasMoreItemThanRemoveAmount_IndexVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasMoreItemThanRemoveAmount_IndexVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 100, removeAmount = 50;
            inventory.AddItem(item1, addAmount);
            inventory.RemoveItem(0, removeAmount);

            Assert.AreEqual(addAmount - removeAmount, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasAsMuchAsRemoveAmount_IndexVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasAsMuchAsRemoveAmount_IndexVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int itemAmount = 50;
            inventory.AddItem(item1, itemAmount);
            inventory.RemoveItem(0, itemAmount);

            Assert.AreEqual(0, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasLessItemThanRemoveAmount_IndexVersion
        [Test]
        public void Inventory_RemoveItemRemovesTheNecessaryAmount_InventoryHasLessItemThanRemoveAmount_IndexVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 50, removeAmount = 100;
            inventory.AddItem(item1, addAmount);
            inventory.RemoveItem(0, removeAmount);

            Assert.AreEqual(0, inventory.GetItemCount(item1));
        }
        #endregion

        #region Inventory_RemoveItemDoesntChangeAnything_InventoryDoesntHaveTheItem_IndexVersion
        [Test]
        public void Inventory_RemoveItemDoesntChangeAnything_InventoryDoesntHaveTheItem_IndexVersion()
        {
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int addAmount = 100, removeAmount = 50;
            inventory.AddItem(item1, addAmount);
            List<ItemBase> inventoryItemsBeforeRemoval = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                inventoryItemsBeforeRemoval.Add(item);
            }

            inventory.RemoveItem(0, removeAmount);
            List<ItemBase> inventoryItemsAfterRemoval = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                inventoryItemsAfterRemoval.Add(item);
            }

            bool beforeAndAfterListsAreEqual = true;
            for (int i = 0; i < inventoryItemsBeforeRemoval.Count; i++)
            {
                if (inventoryItemsBeforeRemoval[i] != inventoryItemsAfterRemoval[i])
                {
                    beforeAndAfterListsAreEqual = false;
                    break;
                }
            }

            Assert.IsTrue(beforeAndAfterListsAreEqual);
        }
        #endregion

        #region Inventory_GetItemReturnValueMatchesIndexerReturnValue
        [Test]
        public void Inventory_GetItemReturnValueMatchesIndexerReturnValue()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(10, new ItemComparer());

            for (int i = 0; i < inventory.GetInventorySize(); i++)
            {
                inventory.AddItem(new ItemBase($"Item{i + 1}"));
            }

            bool allValuesWereEqual = true;
            for (int i = 0; i < inventory.GetInventorySize(); i++)
            {
                if (inventory[i] != inventory.GetItem(i))
                {
                    allValuesWereEqual = false;
                    break;
                }
            }

            Assert.IsTrue(allValuesWereEqual);
        }
        #endregion

        #region Inventory_GetItemIndexReturnsCorrectIndex_ItemExists
        [Test]
        public void Inventory_GetItemIndexReturnsCorrectIndex_ItemExists()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, new ItemComparer());
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");
            ItemBase item3 = new ItemBase("Item3");
            inventory.AddItem(item1, 10);
            inventory.AddItem(item2, 10);
            inventory.AddItem(item3, 10);

            if (inventory.GetItemIndex(item1) == null || inventory.GetItemIndex(item2) == null || inventory.GetItemIndex(item3) == null)
            {
                Assert.Fail();
                return;
            }
            Assert.AreEqual(0, (int)inventory.GetItemIndex(item1)!);
            Assert.AreEqual(1, (int)inventory.GetItemIndex(item2)!);
            Assert.AreEqual(2, (int)inventory.GetItemIndex(item3)!);
        }
        #endregion

        #region Inventory_GetItemIndexReturnsNull_ItemDoesntExist
        [Test]
        public void Inventory_GetItemIndexReturnsNull_ItemDoesntExist()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");

            inventory.AddItem(item1);

            Assert.IsNull(inventory.GetItemIndex(item2));
        }
        #endregion

        #region Inventory_GetItemCountReturnsCorrectItemCount_ItemVersion
        [Test]
        public void Inventory_GetItemCountReturnsCorrectItemCount_ItemVersion()
        {
            int itemCount = 10;
            ItemBase item = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(item, itemCount);

            Assert.AreEqual(itemCount, inventory.GetItemCount(item));
        }
        #endregion

        #region Inventory_GetItemCountReturnsCorrectItemCount_IndexVersion
        [Test]
        public void Inventory_GetItemCountReturnsCorrectItemCount_IndexVersion()
        {
            int itemCount = 10;
            ItemBase item = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(item, itemCount);

            Assert.AreEqual(itemCount, inventory.GetItemCount(0));
        }
        #endregion

        #region Inventory_GetItemCountReturnsZero_ItemIsNotInInventory
        [Test]
        public void Inventory_GetItemCountReturnsZero_ItemIsNotInInventory()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");

            inventory.AddItem(item1);

            Assert.AreEqual(0, inventory.GetItemCount(item2));
        }
        #endregion

        #region Inventory_GetItemCountReturnsZero_IndexIsInvalid
        [Test]
        public void Inventory_GetItemCountReturnsZero_IndexIsInvalid()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());
            ItemBase item1 = new ItemBase("Item1");

            inventory.AddItem(item1);

            Assert.AreEqual(0, inventory.GetItemCount(1));
        }
        #endregion

        #region Inventory_GetCountOfSlotsContainingItemsReturnsCorrectNumberOfSlots
        [Test]
        public void Inventory_GetCountOfSlotsContainingItemsReturnsCorrectNumberOfSlots()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(10, new ItemComparer());

            int countOfAddedItems = 0;
            for (int i = 0; i < 3; i++)
            {
                inventory.AddItem(new ItemBase($"Item{i + 1}"));
                countOfAddedItems++;
            }

            Assert.AreEqual(countOfAddedItems, inventory.GetCountOfSlotsContainingItems());
        }
        #endregion

        #region Inventory_ContainsItemReturnsTrue_ItemIsContained
        [Test]
        public void Inventory_ContainsItemReturnsTrue_ItemIsContained()
        {
            ItemBase item = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(item);

            Assert.IsTrue(inventory.ContainsItem(item, out _, out _));
        }
        #endregion

        #region Inventory_ContainsItemReturnsFalse_ItemIsNotContained
        [Test]
        public void Inventory_ContainsItemReturnsFalse_ItemIsNotContained()
        {
            ItemBase item1 = new ItemBase("Item1");
            ItemBase item2 = new ItemBase("Item2");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(item1);

            Assert.IsFalse(inventory.ContainsItem(item2, out _, out _));
        }
        #endregion

        #region Inventory_ContainsItemOutsCorrectIndex
        [Test]
        public void Inventory_ContainsItemOutsCorrectIndex()
        {
            ItemBase item = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(item);

            _ = inventory.ContainsItem(item, out int index, out _);
            Assert.AreEqual(0, index);
        }
        #endregion

        #region Inventory_ContainsItemOutsCorrectItemCount
        [Test]
        public void Inventory_ContainsItemOutsCorrectItemCount()
        {
            ItemBase item = new ItemBase("Item1");
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            int itemAmount = 10;
            inventory.AddItem(item, itemAmount);

            _ = inventory.ContainsItem(item, out _, out int itemCount);
            Assert.AreEqual(itemAmount, itemCount);
        }
        #endregion

        #region Inventory_ContainsAnyItemReturnsTrue_InventoryIsNotEmpty
        [Test]
        public void Inventory_ContainsAnyItemReturnsTrue_InventoryIsNotEmpty()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            inventory.AddItem(new ItemBase("Item1"));

            Assert.IsTrue(inventory.ContainsAnyItem());
        }
        #endregion

        #region Inventory_ContainsAnyItemReturnsFalse_InventoryIsEmpty
        [Test]
        public void Inventory_ContainsAnyItemReturnsFalse_InventoryIsEmpty()
        {
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(1, new ItemComparer());

            Assert.IsFalse(inventory.ContainsAnyItem());
        }
        #endregion

        #region Inventory_GetInventorySizeReturnsCorrectInventorySize
        [Test]
        public void Inventory_GetInventorySizeReturnsCorrectInventorySize()
        {
            int size = 10;
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(size, new ItemComparer());

            Assert.AreEqual(size, inventory.GetInventorySize());
        }
        #endregion

        #region Inventory_ForeachLoopsThroughTheSlots_NoEmptySlots
        [Test]
        public void Inventory_ForeachLoopsThroughTheSlots_NoEmptySlots()
        {
            ItemComparer itemComparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, itemComparer);

            ItemBase[] itemsToAdd = new ItemBase[inventory.GetInventorySize()];

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                itemsToAdd[i] = new ItemBase($"Item{i + 1}");
            }

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                inventory.AddItem(itemsToAdd[i]);
            }

            List<ItemBase> loopedItems = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                loopedItems.Add(item);
            }

            if (loopedItems.Count != inventory.GetInventorySize())
            {
                Assert.Fail();
            }
            else
            {
                bool allItemsAreTheSameAndInOrder = true;

                for (int i = 0; i < loopedItems.Count; i++)
                {
                    if (loopedItems[i] != inventory[i])
                    {
                        allItemsAreTheSameAndInOrder = false;
                        break;
                    }
                }

                Assert.IsTrue(allItemsAreTheSameAndInOrder);
            }
        }
        #endregion

        #region Inventory_ForeachLoopsThroughTheSlots_EmptySlotsAtStart
        [Test]
        public void Inventory_ForeachLoopsThroughTheSlots_EmptySlotsAtStart()
        {
            ItemComparer itemComparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, itemComparer);

            ItemBase[] itemsToAdd = new ItemBase[inventory.GetInventorySize()];

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                itemsToAdd[i] = new ItemBase($"Item{i + 1}");
            }

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                inventory.AddItem(itemsToAdd[i]);
            }

            inventory.RemoveItem(0);
            inventory.RemoveItem(1);

            List<ItemBase> loopedItems = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                loopedItems.Add(item);
            }

            bool hasRemovedItems = loopedItems.Contains(itemsToAdd[0]) || loopedItems.Contains(itemsToAdd[1]);
            Assert.IsFalse(hasRemovedItems);
        }
        #endregion

        #region Inventory_ForeachLoopsThroughTheSlots_EmptySlotsAtEnd
        [Test]
        public void Inventory_ForeachLoopsThroughTheSlots_EmptySlotsAtEnd()
        {
            ItemComparer itemComparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, itemComparer);

            ItemBase[] itemsToAdd = new ItemBase[inventory.GetInventorySize()];

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                itemsToAdd[i] = new ItemBase($"Item{i + 1}");
            }

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                inventory.AddItem(itemsToAdd[i]);
            }

            inventory.RemoveItem(1);
            inventory.RemoveItem(2);

            List<ItemBase> loopedItems = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                loopedItems.Add(item);
            }

            bool hasRemovedItems = loopedItems.Contains(itemsToAdd[1]) || loopedItems.Contains(itemsToAdd[2]);
            Assert.IsFalse(hasRemovedItems);
        }
        #endregion

        #region Inventory_ForeachLoopsThroughTheSlots_EmptySlotsBetweenFilledSlots
        [Test]
        public void Inventory_ForeachLoopsThroughTheSlots_EmptySlotsBetweenFilledSlots()
        {
            ItemComparer itemComparer = new ItemComparer();
            Inventory<ItemBase> inventory = new Inventory<ItemBase>(3, itemComparer);

            ItemBase[] itemsToAdd = new ItemBase[inventory.GetInventorySize()];

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                itemsToAdd[i] = new ItemBase($"Item{i + 1}");
            }

            for (int i = 0; i < itemsToAdd.Length; i++)
            {
                inventory.AddItem(itemsToAdd[i]);
            }

            inventory.RemoveItem(1);

            List<ItemBase> loopedItems = new List<ItemBase>();
            foreach (ItemBase item in inventory)
            {
                loopedItems.Add(item);
            }

            bool hasRemovedItems = loopedItems.Contains(itemsToAdd[1]);
            Assert.IsFalse(hasRemovedItems);
        }
        #endregion
    }
}
