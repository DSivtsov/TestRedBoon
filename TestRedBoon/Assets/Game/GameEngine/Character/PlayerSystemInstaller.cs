using UnityEngine;
using Zenject;
using GameEngine.Wallet;
using GameEngine.Inventory;
using Sirenix.OdinInspector;

namespace GameEngine.Character
{
    public class PlayerSystemInstaller : MonoInstaller
    {
        private Player _player;
        [SerializeField] private int _initialMoney;
        [SerializeField] private InventoryItemList _inventoryItemList;
        [SerializeField] private WalletStorage _walletStorage;
        [ShowInInspector] private VendorList _vendorList;
        public override void InstallBindings()
        {
            _walletStorage = new WalletStorage(_initialMoney);
            _player = new Player(_inventoryItemList, _walletStorage);
            Container.Bind<WalletStorage>().WithId("Player").FromInstance(_walletStorage);
            Container.Bind<InventoryItemList>().WithId("Player").FromInstance(_inventoryItemList);
            Container.Bind<Player>().FromInstance(_player).AsSingle();
            Container.Bind<VendorList>().AsSingle().OnInstantiated<VendorList>((_, it) => _vendorList = it).NonLazy();
        }
    }
}