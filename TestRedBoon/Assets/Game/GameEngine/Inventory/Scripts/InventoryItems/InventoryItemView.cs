using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace GameEngine.Inventory
{
    /*
		[common] Inventory class
			MyInventory
			OtherInventory (linked Wallet)

	Add class DropInventoryItemOnInventory
		WasDropped(OnthisInventory)
			HideItem()
			If (OnthisInventory == OtherInventory) TrySell()
				else DragItemEnd()
		TrySell()
			If (Wallet(other Inventory).CanSpend(ItemCurrentPrice)) ItemPurchased()
				else ItemNotPurchased()
		ItemPurchased()
			Wallet(other Inventory).SpendMoney()
			Inventory(other Inventory).ItemPurchased(positionInInventory)
			Inventory(my Inventory).ItemSelled(positionInInventory)
		ItemNotPurchased()
			PlaceHolderOfDragedItem.Hide()
			ItemUnHideAtInitPos()
     */
    public sealed class InventoryItemView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField]
        private TextMeshProUGUI _price;

        [SerializeField]
        private Image _iconImage;

        private ushort _positionInIventory;
        // _myInventory - Inventory which Selling item
        private TraderTableView.Inventory _myInventory;

        private float _canvasScaleFactor;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private Vector2 _initialPositionItem;

        //Curently used only for debug
        private string _itemName;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// Syncronize the Visual representation of Item on screen with inventory of player or current vendor
        /// </summary>
        /// <param name="itemSO"></param>
        /// <param name="inventory"></param>
        /// <param name="positionInIventory"></param>
        /// <param name="priceForThisInventory">price based on whome the owner of this Inventory</param>
        public void LinkVisulItemWithSO(InventoryItemSO itemSO, TraderTableView.Inventory inventory, ushort positionInIventory, int priceForThisInventory)
        {
            SetPrice(AmmountFormatter.GetAmmount(priceForThisInventory));
            SetIcon(itemSO.Metadata.icon);
            SetItemName(itemSO.ItemName);
            _positionInIventory = positionInIventory;
            _myInventory = inventory;
            UnHideItem();
        }

        public void SetItemName(string itemName)
        {
            _itemName = itemName;
        }

        public void SetPrice(string price)
        {
            _price.text = price;
        }

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
        }

        public void SetCanvas(float canvasScaleFactor)
        {
            _canvasScaleFactor = canvasScaleFactor;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _initialPositionItem = _rectTransform.anchoredPosition;
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            RestoreInitialItemPositionAndSettings();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvasScaleFactor;
        }

        public void WasDroppedOnInventory(TraderTableView.Inventory inventory)
        {
            //Debug.Log($"Item[{_itemName}] dropped on Inventory[{inventory}]");
            if (_myInventory != inventory)
            {
                HideItem();
                RestoreInitialItemPositionAndSettings();
                if (!_myInventory.TrySell(_positionInIventory, inventory))
                {
                    Debug.LogWarning("TrySell[false]");
                    UnHideItem();
                }
            }
        }

        private void RestoreInitialItemPositionAndSettings()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.anchoredPosition = _initialPositionItem;
        }

        private void HideItem() => gameObject.SetActive(false);

        private void UnHideItem() => gameObject.SetActive(true);
    }
}