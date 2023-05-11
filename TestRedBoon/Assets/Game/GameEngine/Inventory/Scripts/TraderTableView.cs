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

        private List<InventoryItemSO> _playerListItemSO;
        private List<InventoryItemSO> _traderListItemSO;
        private InventoryItemView[] _playerItemsView;
        private InventoryItemView[] _traderItemsView;

        public void Show(WalletStorage currentVendorWalletStorage, InventoryItemList currentVendorInventoryItemList)

        {
            InitTraderTableView(currentVendorWalletStorage, currentVendorInventoryItemList);

            _traderTable.gameObject.SetActive(true);

            InstantiateVisualInventories();

            LoadOnScreenInventory(_playerItemsView,_playerListItemSO);
            LoadOnScreenInventory(_traderItemsView,_traderListItemSO);
        }

        private void InitTraderTableView(WalletStorage currentVendorWalletStorage, InventoryItemList currentVendorInventoryItemList)
        {
            _traderWalletStorage = currentVendorWalletStorage;
            _traderListItemSO = currentVendorInventoryItemList.ListInventoryItemSO;

            _playerListItemSO = _playerInventoryItemList.ListInventoryItemSO;
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

        /// <summary>
        /// Syncronize the Visual representation of Item on screen with inventory of player or current vendor
        /// </summary>
        /// <param name="itemsView"></param>
        /// <param name="listItemSO"></param>
        private void LoadOnScreenInventory(InventoryItemView[] itemsView, List<InventoryItemSO> listItemSO)
        {
            int i = 0;
            for (; i < listItemSO.Count; i++)
            {
                itemsView[i].LinkVisulItemWithSO(listItemSO[i]);
            }
            TunOffNotUsedVisualItem(itemsView, i);

        }
        /// <summary>
        /// Visual not used position in inventories will be turn off
        /// </summary>
        /// <param name="itemsGO"></param>
        /// <param name="idxFirstEmpty"></param>
        private void TunOffNotUsedVisualItem(InventoryItemView[] itemsGO, int idxFirstEmpty)
        {
            for (int i = idxFirstEmpty; i < itemsGO.Length; i++)
            {
                itemsGO[i].gameObject.SetActive(false);
            }
        }

        public override string ToString()
        {
            return $"Player have {_playerInventoryItemList} items and {_playerWalletStorage.Money} money." +
                $" Trader have {_traderInventoryItemList} items and {_traderWalletStorage.Money} money";
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
