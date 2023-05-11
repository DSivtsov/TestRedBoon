using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;
using GameEngine.Inventory;
using GameEngine.Character;

namespace GameSystem
{
    public class InventoryManager 
    {
        [ValueDropdown("_vendorList"), SerializeField]
        private Vendor _selectedVendor;

        private readonly InventoryView _inventoryView;
        private readonly List<Vendor> _vendorList;

        public InventoryManager(InventoryView inventoryView, VendorList vendorList)
        {
            _inventoryView = inventoryView;
            _vendorList = vendorList.GetListVendors();
        }

        [Button]
        public void OpenInventory()
        {
            if (_selectedVendor != null)
            {
                Debug.Log($"OpenInventory(): _selectedVendor[{_selectedVendor}]");

            }
            else
            {
                Debug.LogWarning($"OpenInventory(): Vendor must selected before open Inventory");
                return;
            }
            _inventoryView.Show();
        }

        [Button]
        public void CloseInventory()
        {
            _inventoryView.Hide();
        }

    } 
}
