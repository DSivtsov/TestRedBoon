using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Inventory
{
    [CreateAssetMenu(
        fileName = "InventoryItemList",
        menuName = "GameEngine/Inventory/New InventoryItemList"
    )]
    public sealed class InventoryItemList : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItemSO> _itemsList;

        //public InventoryItemConfig FindItem(string name)
        //{
        //    for (int i = 0, count = this.items.Length; i < count; i++)
        //    {
        //        var item = this.items[i];
        //        if (item.ItemName == name)
        //        {
        //            return item;
        //        }
        //    }

        //    throw new Exception($"Item {name} is not found!");
        //}

        public List<InventoryItemSO> ListInventoryItemSO => _itemsList;

        public override string ToString() => _itemsList.Count.ToString();
    }
}