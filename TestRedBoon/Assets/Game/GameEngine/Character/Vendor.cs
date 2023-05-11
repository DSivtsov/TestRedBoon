using UnityEngine;
using GameEngine.Wallet;
using GameEngine.Inventory;
using Sirenix.OdinInspector;

namespace GameEngine.Character
{
    [System.Serializable]
    public class Vendor : ITrader
    {
        [ReadOnly, ShowInInspector] private string _name;
        private InventoryItemList _inventoryItemList;
        private int _initialMoney;
        private bool _isMoneyInfinity =  false;
        private WalletStorage _walletStorage;

        public Vendor(string name, InventoryItemList inventoryItemList, bool isMoneyInfinity, WalletStorage walletStorage)
        {
            _name = name;
            _inventoryItemList = inventoryItemList;
            _isMoneyInfinity = isMoneyInfinity;
            _walletStorage = walletStorage;
        }

        public InventoryItemList InventoryItemList => _inventoryItemList;

        public WalletStorage WalletStorage => _walletStorage;

        public string Name => _name;

        public override string ToString() => _name;
    }
}
