using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
//using GameEngine.
using Zenject;

namespace GameEngine.Inventory
{
    public class InventorySystem
    {
        private Canvas _canvasInventory;

        public InventorySystem(Canvas canvasInventory)
        {
            _canvasInventory = canvasInventory;
        }

        [Button]
        public void OpenInventory()
        {
            ShowInventory();
        }

        [Button]
        public void CloseInventory()
        {
            HideInventory();
        }

        private void ShowInventory()
        {
            _canvasInventory.gameObject.SetActive(true);
        }



        private void HideInventory()
        {
            _canvasInventory.gameObject.SetActive(false);
        }
    } 
}
