using UnityEngine;
using Zenject;
using Game.GameEngine.Inventory;

public class InventorySystemInstaller : MonoInstaller
{
    [SerializeField] private InventoryManager _inventoryManager;

    public override void InstallBindings()
    {
        this.Container.Bind<InventoryManager>().FromInstance(_inventoryManager).AsSingle();
    }
}

