using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Wallet;
using Zenject;
using System;

namespace GameEngine.Inventory
{
    /// <summary>
    /// Show two inventory
    ///     Left Player Inevntory(InventoryList)
    ///     Right Current Vendor Inevntory(InventoryList)
    /// </summary>
    public class TraderTableView : MonoBehaviour
    {
        [SerializeField] private Canvas _traderTableCanvas;
        [SerializeField] private FieldSizeSO _fieldSizeSO;
        [SerializeField] private RectTransform _playerTopLeftrect;
        [SerializeField] private RectTransform _traderTopLeftrect;
        [SerializeField] private RectTransform _prefabItem;


        [Inject(Id = "Player")]
        private WalletStorage _playerWalletStorage;
        [Inject(Id = "Player")]
        private InventoryItemList _playerInventoryItemList;

        private WalletStorage _traderWalletStorage;
        private InventoryItemList _traderInventoryItemList;

        private InventoryItemView[] _playerItemsView;
        private InventoryItemView[] _traderItemsView;

        private Inventory _playerInventory;
        private Inventory _traderInventory;

        public void Show(WalletStorage currentVendorWalletStorage, InventoryItemList currentVendorInventoryItemList)

        {
            InstantiateVisualInventories();

            _traderWalletStorage = currentVendorWalletStorage;

            _playerInventory = new Inventory(this, _playerItemsView, _playerInventoryItemList.ListInventoryItemSO, isPlayerInventory: true, _traderWalletStorage);
            _traderInventory = new Inventory(this, _traderItemsView, currentVendorInventoryItemList.ListInventoryItemSO, isPlayerInventory: false, _playerWalletStorage);

            _playerInventory.LinkWithVisualInventoryView(_playerTopLeftrect);
            _traderInventory.LinkWithVisualInventoryView(_traderTopLeftrect);

            _playerInventory.LoadOnScreenInventory();
            _traderInventory.LoadOnScreenInventory();

            _traderTableCanvas.gameObject.SetActive(true);
        }

        private void InstantiateVisualInventories()
        {
            InstantiateInventoryItems instantiateInventory = new InstantiateInventoryItems(_fieldSizeSO, _prefabItem, _traderTableCanvas);
            instantiateInventory.ClearFields(_playerTopLeftrect);
            instantiateInventory.ClearFields(_traderTopLeftrect);
            _playerItemsView = instantiateInventory.BuildField(_playerTopLeftrect);
            _traderItemsView = instantiateInventory.BuildField(_traderTopLeftrect);
        }

        public void Hide()
        {
            _traderTableCanvas.gameObject.SetActive(false);
        }

        public override string ToString()
        {
            return $"Player have {_playerInventoryItemList} items and {_playerWalletStorage.Money} money." +
                $" Trader have {_traderInventoryItemList} items and {_traderWalletStorage.Money} money";
        }

        public class Inventory
        {
            private TraderTableView traderTableView;
            private List<InventoryItemSO> listItemSO;
            private InventoryItemView[] itemsView;
            private bool isPlayerInventory;
            private WalletStorage purchaser;

            /// <param name="traderTableView"></param>
            /// <param name="itemsView">Visual representation Items in the Inventory</param>
            /// <param name="listItemSO">inventory of player or current vendor</param>
            /// <param name="isPlayerInventory"></param>
            /// <param name="purchaser">Wallet of contractor who will purchase items from this Inventory</param>
            public Inventory(TraderTableView traderTableView, InventoryItemView[] itemsView, List<InventoryItemSO> listItemSO,
                bool isPlayerInventory, WalletStorage purchaser)
            {
                this.traderTableView = traderTableView;
                this.listItemSO = listItemSO;
                this.itemsView = itemsView;
                this.isPlayerInventory = isPlayerInventory;
                this.purchaser = purchaser;
            }

            /// <summary>
            /// Syncronize the Visual representation of All Items in the Inventory on screen with inventory of player or current vendor
            /// </summary>
            public void LoadOnScreenInventory()
            {
                int i = 0;
                for (; i < listItemSO.Count; i++)
                {
                    InventoryItemSO inventoryItemSO = listItemSO[i];
                    //Debug.Log($"[{i}] [{inventoryItemSO.ItemName}]");
                    //The cast ushort supported by limitation of Array size (see comment at it creation)
                    itemsView[i].LinkVisulItemWithSO(inventoryItemSO, this, (ushort)i, GetItemSellingPrice(inventoryItemSO));
                }
                TunOffNotUsedVisualItem(i);
            }
            /// <summary>
            /// Visual not used position in inventories will be turn off
            /// </summary>
            /// <param name="itemsGO"></param>
            /// <param name="idxFirstEmpty"></param>
            private void TunOffNotUsedVisualItem(int idxFirstEmpty)
            {
                for (int i = idxFirstEmpty; i < itemsView.Length; i++)
                {
                    itemsView[i].gameObject.SetActive(false);
                }
            }

            /// <summary>
            /// Get Salling Price if current Inventory of Player and Purchasing Price if current Inventory of Vendor
            /// </summary>
            /// <param name="itemSO"></param>
            /// <returns></returns>
            private int GetItemSellingPrice(InventoryItemSO itemSO)
            {
                if (isPlayerInventory)
                    return itemSO.SalesPrice;
                else
                    return itemSO.PuchasePrice;
            }
            private int GetItemPurchasingPrice(InventoryItemSO itemSO)
            {
                if (isPlayerInventory)
                    return itemSO.SalesPrice;
                else
                    return itemSO.PuchasePrice;
            }



            /// <summary>
            /// Item from current Inventory trying to Sell
            /// </summary>
            /// <param name="positionInIventorySellingItem">position selling Item in current Inventory</param>
            /// <param name="inventoryToSell"></param>
            /// <returns>false if can't sell</returns>
            public bool TrySell(ushort positionInIventorySellingItem, Inventory inventoryToSell)
            {
                InventoryItemSO sellingnventoryItemSO = listItemSO[positionInIventorySellingItem];
                Debug.Log($"TrySell Item[{sellingnventoryItemSO.ItemName}] to Inventory[{inventoryToSell}]" +
                    $" by [{GetItemSellingPrice(sellingnventoryItemSO)}] price");

                if (inventoryToSell.TryToPuchaseItem(sellingnventoryItemSO))
                {
                    RemoveItemFromInventory(positionInIventorySellingItem);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="itemPurchased"></param>
            /// <returns>true item was purchased</returns>
            public bool TryToPuchaseItem(InventoryItemSO itemPurchased)
            {
                int purchasePrice = GetItemPurchasingPrice(itemPurchased);
                if (purchaser.CanSpendMoney(purchasePrice))
                {
                    purchaser.SpendMoney(purchasePrice);
                    AddItemToInventory(itemPurchased);
                    return true;
                }
                else
                    return false;
            }

            public void AddItemToInventory(InventoryItemSO itemPurchased)
            {
                listItemSO.Add(itemPurchased);
                Debug.Log("RedrawInventory. From AddItemToInventory()");
                RedrawInventory();
            }

            private void RemoveItemFromInventory(ushort positionInInventory)
            {
                listItemSO.RemoveAt(positionInInventory);
                Debug.Log("RedrawInventory. From AddItemToInventory()");
                RedrawInventory();
            }

            private void RedrawInventory()
            {
                LoadOnScreenInventory();
            }

            public void LinkWithVisualInventoryView(RectTransform playerTopLeftrect)
            {
                if (playerTopLeftrect != null)
                {
                    InventoryView inventoryView = playerTopLeftrect.GetComponent<InventoryView>();
                    if (inventoryView != null)
                    {
                        inventoryView.SetCorespondenInventory(this);
                    }
                    else
                        Debug.LogError($"{this}: In {playerTopLeftrect} absent the component {typeof(InventoryView)}");
                }
                else
                    Debug.LogError("{this}: Not correctly Initialized");
            }

            public override string ToString()
            {
                if (listItemSO.Count == 0)
                    return $"I'm EMPTY";
                else
                    return $"I have {listItemSO.Count} items, the first Item[{listItemSO[0].ItemName}]";
            }
        }

        public struct InstantiateInventoryItems
        {
            private readonly float wSpace, hSpace;
            private readonly int columns, rows;
            private readonly int wSizeItem, hSizeItem;
            private readonly RectTransform prefabItem;
            private Transform parentTransform;
            private Canvas traderTableCanvas;

            public InstantiateInventoryItems(FieldSizeSO _fieldSize, RectTransform _prefabItem, Canvas _traderTableCanvas)
            {
                _fieldSize.CalculateSpacing();
                (wSpace, hSpace) = _fieldSize.TupleSpacing;
                (columns, rows) = _fieldSize.NumItems;
                (wSizeItem, hSizeItem) = _fieldSize.SizeItems;
                prefabItem = _prefabItem;
                parentTransform = null;
                traderTableCanvas = _traderTableCanvas;
            }

            /// <summary>
            /// Filling from Left to Right, from Up to Down
            /// </summary>
            /// <param name="_begTopLeftrect"></param>
            public InventoryItemView[] BuildField(RectTransform _begTopLeftrect)
            {
                //The Array size is limmited by possible value of [rows * columns] in class GameEngine.Inventory.FieldSize 
                InventoryItemView[] itemsView = new InventoryItemView[rows * columns];
                int idx = 0;
                parentTransform = _begTopLeftrect.transform;

                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        itemsView[idx++] = InstantiateItems(GetAnchorVector2ForElement(row, column), getName(row, column));
                    }
                }
                return itemsView;
            }

            private string getName(int row, int column)
            {
                return $"Item[{row},{column}]";
            }

            private InventoryItemView InstantiateItems(Vector2 vector2, string name)
            {
                RectTransform rectTransform = Instantiate<RectTransform>(prefabItem, parentTransform);
                rectTransform.anchoredPosition = vector2;
                rectTransform.name = name;
                //return rectTransform.gameObject.GetComponent<InventoryItemView>();
                InventoryItemView inventoryItemView = rectTransform.GetComponent<InventoryItemView>();
                inventoryItemView.SetCanvas(traderTableCanvas.scaleFactor);
                return rectTransform.GetComponent<InventoryItemView>();
            }

            //For Testing purpose only
            private string GetInfoTopLeftAnchorPosForElement(int row, int column)
            {
                return $"[{row},{column}][{GetAnchorVector2ForElement(row, column):f1}]";
            }

            private Vector2 GetAnchorVector2ForElement(int row, int column)
            {
                float posX = wSpace * (column + 1) + wSizeItem * column;
                float posY = hSpace * (row + 1) + hSizeItem * row;
                return new Vector2(posX, -posY);
            }

            public void ClearFields(RectTransform _playerTopLeftrect)
            {
                foreach (Transform item in _playerTopLeftrect.transform)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }
        }
    }
}
