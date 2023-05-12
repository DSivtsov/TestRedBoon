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

        public List<InventoryItemSO> ListInventoryItemSO => _itemsList;

        public override string ToString() => _itemsList.Count.ToString();
    }
}