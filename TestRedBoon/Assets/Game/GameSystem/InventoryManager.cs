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

        private readonly TraderTableView _traderTableView;
        private readonly List<Vendor> _vendorList;

        public InventoryManager(TraderTableView inventoryView, VendorList vendorList)
        {
            _traderTableView = inventoryView;
            _vendorList = vendorList.GetListVendors();
        }

        [Button]
        public void OpenTraderTable()
        {
            if (_selectedVendor != null)
            {
                Debug.Log($"OpenTraderTable(): _selectedVendor[{_selectedVendor}]");

            }
            else
            {
                Debug.LogWarning($"OpenTraderTable(): Vendor must selected before open Inventory");
                return;
            }
            _traderTableView.Show(_selectedVendor.WalletStorage,_selectedVendor.InventoryItemList);
        }

        [Button]
        public void CloseTraderTable()
        {
            _traderTableView.Hide();
        }

    } 
}
