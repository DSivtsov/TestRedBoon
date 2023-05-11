using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameEngine.Inventory
{
    [CreateAssetMenu(
        fileName = "InventoryItemSO",
        menuName = "GameEngine/Inventory/New InventoryItemSO"
    )]
    public sealed class InventoryItemSO : SerializedScriptableObject
    {
        public string ItemName => this.origin.Name;

        public InventoryItemMetadata Metadata => this.origin.Metadata;

        public InventoryItem Prototype => this.origin;

        public int SalesPrice => this.origin.SalesPrice;

        public int PuchasePrice => this.origin.PuchasePrice;

        [OdinSerialize]
        private InventoryItem origin = new InventoryItem();

        private void OnValidate()
        {
            if (this.origin.SalesPrice <= 0)
                this.origin.SetRandomPrice();

            if (this.origin.PuchasePrice < this.origin.SalesPrice)
                this.origin.UpdatePurchasePrice();
        }
    }
}