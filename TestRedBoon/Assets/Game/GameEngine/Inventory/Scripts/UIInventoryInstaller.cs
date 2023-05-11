using System;
using UnityEngine;
using Zenject;
using GameEngine.Wallet;


namespace GameEngine.Inventory
{
    public sealed class UIInventoryInstaller : MonoBehaviour
    {
        [SerializeField] private ItemPanel _moneyPanel;

        private MoneyPanelAdapter _moneyPanelAdapter;

        [Inject(Id = "PlayerWallet")]
        private WalletStorage _moneyStorage;

        private void Awake()
        {
            _moneyPanelAdapter = new MoneyPanelAdapter(_moneyStorage, _moneyPanel);
        }


        private void OnEnable()
        {
            _moneyPanelAdapter.Enable();
        }

        private void OnDisable()
        {
            _moneyPanelAdapter.Disable();
        }
    } 
}