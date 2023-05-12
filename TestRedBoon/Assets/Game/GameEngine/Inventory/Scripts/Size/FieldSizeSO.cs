using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine.Inventory
{
    [CreateAssetMenu(
        fileName = "FieldSize",
        menuName = "GameEngine/Inventory/New FieldSize"
    )]
    public class FieldSizeSO : ScriptableObject
    {
        //To exclude from stupid values and to more safety to use USHORT for store index in class GameEngine.Inventory.InventoryItemView
        private const int MAXINVENTORY = System.UInt16.MaxValue;

        [SerializeField] private int _wSizePanel;
        [SerializeField] private int _hSizePanel;
        [SerializeField] private int _wSizeItem;
        [SerializeField] private int _hSizeItem;
        [SerializeField] private int _rowItems;
        [SerializeField] private int _columnItems;


        [ReadOnly, SerializeField] private float _wightSpacing;
        [ReadOnly, SerializeField] private float _heightSpacing;

        private bool _isReadyInitialValues;

        public (float wight, float height) TupleSpacing => (_wightSpacing, _heightSpacing);
        public float WightSpacing => _wightSpacing;
        public float HeightSpacing => _heightSpacing;

        public (int columns, int rows) NumItems => (_columnItems, _rowItems);
        public int RowItems => _rowItems;
        public int ColumnItems => _columnItems;

        public (int wight, int height) SizeItems => (_wSizeItem, _hSizeItem);
        public int WidthSizeItem => _wSizeItem;
        public int HeightSizeItem => _hSizeItem;

        [Button]
        public void CalculateSpacing()
        {
            if (!IsNotReadyInitialValues)
            {
                _wightSpacing = (_wSizePanel - _columnItems * _wSizeItem) / (_columnItems + 1.0f);
                _heightSpacing = (_hSizePanel - _rowItems * _hSizeItem) / (_rowItems + 1.0f);
                if (IsNotHaveSpaceForItems)
                {
                    throw new System.NotSupportedException("Not Enought space on screen to place the items." +
                        " Execution stopped");
                }
                if (IsNotSupportHugeInventory)
                {
                    throw new System.NotSupportedException($"Not Support the Huge Inventory more then {MAXINVENTORY} items." +
                        $" Execution stopped");
                }
            }
            else
                throw new System.NotSupportedException("Initial Values not correctly. Execution stopped");
        }

        public bool IsNotReadyInitialValues => (IsWrongValue(_wSizePanel) || IsWrongValue(_hSizePanel) || IsWrongValue(_wSizeItem)
                || IsWrongValue(_hSizeItem) || IsWrongValue(_rowItems) || IsWrongValue(_columnItems));

        public bool IsNotHaveSpaceForItems => IsWrongValue((int)_wightSpacing) || IsWrongValue((int)_heightSpacing);
        public bool IsNotSupportHugeInventory => _rowItems * _columnItems > MAXINVENTORY;

        private bool IsWrongValue(int value) => value < 1;
    } 
}
