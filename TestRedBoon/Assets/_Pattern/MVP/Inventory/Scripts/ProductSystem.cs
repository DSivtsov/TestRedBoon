using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.GameEngine.Inventory;
using Pattern.MVO;
using Sirenix.OdinInspector;

namespace Pattern.MVP
{
    [Serializable]
    public class ProductSystem
    {
        private readonly MoneyStorage _moneyStorage;

        public ProductSystem(MoneyStorage moneyStorage)
        {
            Debug.Log($"ProductSystem({moneyStorage.Money})");
            _moneyStorage = moneyStorage;
        }

        [Button]
        public bool CanBuy(InventoryItemSO inventoryItemSO)
        {
            return inventoryItemSO.PuchasePrice < _moneyStorage.Money;
        }

        [Button]
        public bool CanSell(InventoryItemSO inventoryItemSO)
        {
            //return inventoryItemSO.SalesPrice < _moneyStorage.Money;
            throw new NotImplementedException("CanSell(InventoryItemSO inventoryItemSO)");
        }

        [Button]
        public void Buy(InventoryItemSO inventoryItemSO)
        {
            if (CanBuy(inventoryItemSO))
            {
                _moneyStorage.SpendMoney(inventoryItemSO.PuchasePrice);
                Debug.Log($"Buy: {inventoryItemSO.ItemName} is bought. Money left[{_moneyStorage.Money}]");
            }
            else
            {
                Debug.Log($"Buy: Not enough money to buy {inventoryItemSO.ItemName}");
            }
        }

        [Button]
        public void Sell(InventoryItemSO inventoryItemSO)
        {
            if (CanSell(inventoryItemSO))
            {
                throw new NotImplementedException("Sell(InventoryItemSO inventoryItemSO)");
                Debug.Log($"Sell: {inventoryItemSO.ItemName} is sold");
            }
            else
            {
                Debug.Log($"Sell: Not enough money to sell {inventoryItemSO.ItemName}");
            }
        }
    } 
}
