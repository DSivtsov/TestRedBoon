using System.Collections.Generic;
using Game.GameEngine;
//using GameSystem;
//using Windows;
using UnityEngine;

namespace Game.GameEngine.Inventory
{
    public sealed class InventoryItemListPresenter
    {
        [SerializeField]
        private InventoryItemView prefab;

        [SerializeField]
        private Transform container;

        //private InventoryService inventoryService;

        //private PopupManager popupManager;

        //private InventoryItemConsumer consumeManager;

        //private readonly Dictionary<InventoryItem, ViewHolder> items = new();
        private readonly Dictionary<InventoryItem, ViewHolder> items = new Dictionary<InventoryItem, ViewHolder>();

        public void OnShow(object args)
        {
            //var playerInventory = this.inventoryService.GetInventory();
            //playerInventory.OnItemAdded += this.AddItem;
            //playerInventory.OnItemRemoved += this.RemoveItem;

            //var inventoryItems = playerInventory.GetAllItems();
            //for (int i = 0, count = inventoryItems.Length; i < count; i++)
            //{
            //    var inventoryItem = inventoryItems[i];
            //    this.AddItem(inventoryItem);
            //}
        }

        public void OnHide()
        {
            //var playerInventory = this.inventoryService.GetInventory();
            //var playerInventory = new 
        //    playerInventory.OnItemAdded -= this.AddItem;
        //    playerInventory.OnItemRemoved -= this.RemoveItem;

        //    var inventoryItems = playerInventory.GetAllItems();
        //    for (int i = 0, count = inventoryItems.Length; i < count; i++)
        //    {
        //        var inventoryItem = inventoryItems[i];
        //        this.RemoveItem(inventoryItem);
        //    }
        }

        public void AddItem(InventoryItem item)
        {
            if (this.items.ContainsKey(item))
            {
                return;
            }

            //var view = Instantiate(this.prefab, this.container);
            //var presenter = new InventoryItemViewPresenter(view, item);
            //presenter.Construct(this.popupManager, this.consumeManager);

            //var viewHolder = new ViewHolder(view, presenter);
            //this.items.Add(item, viewHolder);

            //presenter.Start();
        }

        private void RemoveItem(InventoryItem item)
        {
            if (!this.items.ContainsKey(item))
            {
                return;
            }

            var viewHolder = this.items[item];
            //viewHolder.presenter.Stop();
            //Destroy(viewHolder.view.gameObject);
            this.items.Remove(item);
        }

        //void IGameConstructElement.ConstructGame(GameContext context)
        //{
        //    this.inventoryService = context.GetService<InventoryService>();
        //    this.popupManager = context.GetService<PopupManager>();
        //    this.consumeManager = context.GetService<InventoryItemConsumer>();
        //}

        private sealed class ViewHolder
        {
            public readonly InventoryItemView view;
            //public readonly InventoryItemViewPresenter presenter;

            //public ViewHolder(InventoryItemView view, InventoryItemViewPresenter presenter)
            //{
            //    this.view = view;
            //    this.presenter = presenter;
            //}
        }
    }
}