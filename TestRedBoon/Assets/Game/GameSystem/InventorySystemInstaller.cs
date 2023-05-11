using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using GameEngine.Inventory;

namespace GameSystem
{
    public class InventorySystemInstaller : MonoInstaller
    {
        [SerializeField] private Canvas _canvasInventory;
        [ShowInInspector] private InventorySystem _inventoryManager;

        public override void InstallBindings()
        {
            _inventoryManager = new InventorySystem(_canvasInventory);
            Container.Bind<InventorySystem>().FromInstance(_inventoryManager).AsSingle();
        }
    } 
}

