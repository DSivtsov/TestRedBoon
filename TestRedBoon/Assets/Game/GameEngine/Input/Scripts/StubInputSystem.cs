using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Game.GameEngine.Inventory;

public class StubInputSystem : MonoBehaviour
{
    [Inject]
    private InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager.OpenInventory();
    }

}
