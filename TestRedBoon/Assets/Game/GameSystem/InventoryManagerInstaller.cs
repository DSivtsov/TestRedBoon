using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using GameEngine.Character;
using GameEngine.Inventory;


namespace GameSystem
{
    public class InventoryManagerInstaller : MonoInstaller
    {
        [SerializeField] private TraderTableView _inventoryView;
        [ShowInInspector] private InventoryManager _inventoryManager;

        [Inject]
        private VendorList _vendorList;
        public override void InstallBindings()
        {
            _inventoryManager = new InventoryManager(_inventoryView, _vendorList);
            Container.Bind<InventoryManager>().FromInstance(_inventoryManager).AsSingle();
        }
    } 
}

