using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameEngine.Inventory
{
    [Serializable]
    public class InventoryManager
    {
        [SerializeField] private Canvas _canvasInventory;
        public void OpenInventory()
        {
            ShowInventory();
        }

        private void ShowInventory()
        {
            _canvasInventory.gameObject.SetActive(true);
        }
    } 
}
