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
        [SerializeField] private Canvas _traderTable;
        [SerializeField] private FieldSize _fieldSize;
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

            _playerInventory = new Inventory(this, _playerItemsView, _playerInventoryItemList.ListInventoryItemSO);
            _traderInventory = new Inventory(this, _traderItemsView, currentVendorInventoryItemList.ListInventoryItemSO);

            _playerInventory.LoadOnScreenInventory();
            _traderInventory.LoadOnScreenInventory();

            _traderTable.gameObject.SetActive(true);
        }

        private void InstantiateVisualInventories()
        {
            InstantiateInventoryItems instantiateInventory = new InstantiateInventoryItems(_fieldSize, _prefabItem);
            instantiateInventory.ClearFields(_playerTopLeftrect);
            instantiateInventory.ClearFields(_traderTopLeftrect);
            _playerItemsView = instantiateInventory.BuildField(_playerTopLeftrect);
            _traderItemsView = instantiateInventory.BuildField(_traderTopLeftrect);
        }

        public void Hide()
        {
            _traderTable.gameObject.SetActive(false);
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

            public Inventory(TraderTableView traderTableView, InventoryItemView[] _playerItemsView, List<InventoryItemSO> listItemSO)
            {
                this.traderTableView = traderTableView;
                this.listItemSO = listItemSO;
                this.itemsView = _playerItemsView;
            }

            /// <summary>
            /// Syncronize the Visual representation of Item on screen with inventory of player or current vendor
            /// </summary>
            /// <param name="itemsView"></param>
            /// <para
            public void LoadOnScreenInventory()
            {
                int i = 0;
                for (; i < listItemSO.Count; i++)
                {
                    //The cast ushort supported by limitation of Array size (see comment at it creation)
                    itemsView[i].LinkVisulItemWithSO(listItemSO[i], (ushort)i);
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
        }

        public struct InstantiateInventoryItems
        {
            private readonly float wSpace, hSpace;
            private readonly int columns, rows;
            private readonly int wSizeItem, hSizeItem;
            private readonly RectTransform prefabItem;
            private Transform parentTransform;

            public InstantiateInventoryItems(FieldSize _fieldSize, RectTransform _prefabItem)
            {
                _fieldSize.CalculateSpacing();
                (wSpace, hSpace) = _fieldSize.TupleSpacing;
                (columns, rows) = _fieldSize.NumItems;
                (wSizeItem, hSizeItem) = _fieldSize.SizeItems;
                prefabItem = _prefabItem;
                parentTransform = null;
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
