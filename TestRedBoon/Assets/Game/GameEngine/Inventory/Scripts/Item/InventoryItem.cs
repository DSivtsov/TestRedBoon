using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine.Inventory
{
    [Serializable]
    public sealed class InventoryItem
    {
        private const int INITMINPRICE = 1;
        private const int INITMAXPRICE = 100;
        private const int INITMINPROFIT = 1;
        private const int INITMAXPROFIT = 20;

        public string Name => this.name;

        public InventoryItemMetadata Metadata => this.metadata;

        public int SalesPrice => this.salePrice;

        public int PuchasePrice => this.puchasePrice;


        [PropertyOrder(-10)]
        [SerializeField]
        private string name;

        [PropertyOrder(-9)]
        [SerializeField]
        private int salePrice;

        [PropertyOrder(-8)]
        [SerializeField]
        private int puchasePrice;

        [PropertyOrder(-7)]
        [SerializeField]
        private InventoryItemMetadata metadata;

        public InventoryItem()
        {
            this.name = string.Empty;
            this.metadata = new InventoryItemMetadata();
        }

        public void SetRandomPrice()
        {
            System.Random random = new System.Random();
            this.salePrice = random.Next(INITMINPRICE, INITMAXPRICE + 1);
            this.puchasePrice = this.salePrice + UnityEngine.Random.Range(INITMINPROFIT, INITMAXPROFIT + 1);
        }

        public void UpdatePurchasePrice() => this.puchasePrice = this.salePrice + 1;
    }
}