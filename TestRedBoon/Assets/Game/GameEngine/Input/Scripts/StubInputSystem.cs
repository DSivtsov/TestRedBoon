using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GameEngine.Inventory;

public class StubInputSystem : MonoBehaviour
{
    [Inject]
    private InventorySystem _inventoryManager;

    private void Start()
    {
        _inventoryManager.OpenInventory();
    }

}
