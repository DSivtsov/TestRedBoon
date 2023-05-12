using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using Zenject;

public class DebugInventoryManager : MonoBehaviour
{
    [Inject]
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _inventoryManager.EmulateSelectionOfVendorAndOpenTraderTable();
    }

}
