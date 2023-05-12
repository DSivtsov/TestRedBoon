using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameEngine.Inventory
{
    public class InventoryView : MonoBehaviour, IDropHandler
    {
        private TraderTableView.Inventory _inventory;

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                InventoryItemView inventoryItemView = eventData.pointerDrag.GetComponent<InventoryItemView>();
                if (inventoryItemView != null)
                {
                    inventoryItemView.WasDroppedOn(_inventory);
                }
            }
            else
                Debug.LogWarning("{this}: Item was dropped in a very strange place");
        }

        public void SetCorespondenInventory(TraderTableView.Inventory inventory)
        {
            _inventory = inventory;
        }
    } 
}
