using UnityEngine;
using Zenject;
using Game.UI;
using GameEngine.Wallet;

namespace GameEngine.Inventory
{
    public class MoneyPanelAdapter
    {
        private WalletStorage _moneyStorage;
        private ItemPanel _moneyPanel;

        public MoneyPanelAdapter(WalletStorage itemStorage, ItemPanel itemPanel)
        {
            _moneyStorage = itemStorage;
            _moneyPanel = itemPanel;
        }

        [Inject]
        public void Construct(WalletStorage moneyStorage)
        {
            _moneyStorage = moneyStorage;
        }

        private void OnMoneySet(int ammount) => _moneyPanel.SetupMoney(FormatAmmount(ammount));

        private void OnMoneyChanged(int newAmmount) => _moneyPanel.UpdateMoney(FormatAmmount(newAmmount));

        public void Enable()
        {
            _moneyStorage.OnMoneyChanged += OnMoneyChanged;
            _moneyStorage.OnMoneySet += OnMoneySet;
            OnMoneySet(_moneyStorage.Money);
        }

        public void Disable()
        {
            _moneyStorage.OnMoneyChanged -= OnMoneyChanged;
            _moneyStorage.OnMoneySet -= OnMoneySet;
        }

        private string FormatAmmount(int ammount) => AmmountFormatter.GetAmmount(ammount);
    }
}
