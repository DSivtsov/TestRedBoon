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
        private bool _isItemInProcessSelling;

        //Curently used only for debug
        private string _itemName;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        public void LinkVisulItemWithSO(InventoryItemSO itemSO, TraderTableView.Inventory inventory, ushort positionInIventory)
        {
            SetPrice(AmmountFormatter.GetAmmount(itemSO.PuchasePrice));
            SetIcon(itemSO.Metadata.icon);
            SetItemName(itemSO.ItemName);
            _positionInIventory = positionInIventory;
            _myInventory = inventory;
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
            //Debug.Log("OnBeginDrag");
            _initialPositionItem = _rectTransform.anchoredPosition;
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log("OnEndDrag");
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            if (!_isItemInProcessSelling)
                RestoreInitialItemPosition();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            //Debug.Log("OnDrag");
            _rectTransform.anchoredPosition += eventData.delta / _canvasScaleFactor;
        }

        public void WasDroppedOn(TraderTableView.Inventory inventory)
        {
            //Debug.Log($"Item[{_itemName}] dropped on Inventory[{inventory}]");
            HideItem();
            if (_myInventory != inventory)
            {
                _isItemInProcessSelling = true;
                if (!inventory.TrySell(_positionInIventory, inventory))
                {
                    Debug.Log("TrySell[false]");
                    RestoreInitialItemPosition(); 
                }
                _isItemInProcessSelling = false;
            }
            else
            {
                RestoreInitialItemPosition();
            }
        }

        private void RestoreInitialItemPosition()
        {
            _rectTransform.anchoredPosition = _initialPositionItem;
            UnHideItem();
        }

        private void HideItem() => gameObject.SetActive(false);

        private void UnHideItem() => gameObject.SetActive(true);
    }
}