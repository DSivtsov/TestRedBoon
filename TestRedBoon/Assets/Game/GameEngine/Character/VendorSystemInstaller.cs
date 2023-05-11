using UnityEngine;
using Zenject;
using GameEngine.Wallet;
using GameEngine.Inventory;
using Sirenix.OdinInspector;

namespace GameEngine.Character
{
    public class VendorSystemInstaller : MonoInstaller
    {
        [SerializeField] private Vendor _vendor;
        [SerializeField] private string _name;
        [SerializeField] private int _initialMoney;
        [SerializeField] private bool _isMoneyInfinity = false;
        [SerializeField] private InventoryItemList _inventoryItemList;
        [SerializeField] private WalletStorage _walletStorage;

        [Inject]
        private VendorList _vendorList;

        public override void InstallBindings()
        {
            _walletStorage = new WalletStorage(_initialMoney);
            _vendor = new Vendor(_name, _inventoryItemList, _isMoneyInfinity, _walletStorage);
            _vendorList.AddVendor(_vendor);
        }
    }
}