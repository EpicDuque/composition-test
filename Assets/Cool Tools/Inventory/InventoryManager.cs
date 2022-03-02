using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static List<ItemBase> Collection { get; set; }
        
        public static UnityEvent InventoryUpdated { get; private set; }

        public static IEnumerable<ItemBase> GetItemsWithName(string itemName)
        {
            return Collection.Where(x => x.ItemName == itemName);
        }

        private void Start()
        {
            Collection = new List<ItemBase>();
            InventoryUpdated = new UnityEvent();
            
            UpdateInventory();
        }

        public static void UpdateInventory()
        {
            
            InventoryUpdated?.Invoke();
        }
    }
}