using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Wallet;
using GameEngine.Inventory;
using Sirenix.OdinInspector;
using Zenject;

namespace GameEngine.Character
{
    public class Player : ITrader
    {
        private InventoryItemList _inventoryItemList;
        private WalletStorage _walletStorage;

        public Player(InventoryItemList inventoryItemList, WalletStorage walletStorage)
        {
            _inventoryItemList = inventoryItemList;
            _walletStorage = walletStorage;
        }

        public InventoryItemList InventoryItemList => _inventoryItemList;

        public WalletStorage WalletStorage => _walletStorage;

    }
}
