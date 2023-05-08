using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{
    public class MoneyPanelAdapter
    {
        private MoneyStorage _moneyStorage;
        private ItemPanel _moneyPanel;

        public MoneyPanelAdapter(MoneyStorage itemStorage, ItemPanel itemPanel)
        {
            _moneyStorage = itemStorage;
            _moneyPanel = itemPanel;
        }

        [Inject]
        public void Construct(MoneyStorage moneyStorage)
        {
            _moneyStorage = moneyStorage;
        }

        public void Enable()
        {
            _moneyStorage.OnMoneyChanged += OnMoneyChanged;
            _moneyStorage.OnMoneySet += OnMoneySet;
            _moneyPanel.SetupMoney(_moneyStorage.Money.ToString());
        }
        public void Disable()
        {
            _moneyStorage.OnMoneyChanged -= OnMoneyChanged;
            _moneyStorage.OnMoneySet -= OnMoneySet;
        }

        private void OnMoneySet(int ammount) => _moneyPanel.SetupMoney(ammount.ToString());


        private void OnMoneyChanged(int newAmmount) => _moneyPanel.UpdateMoney(newAmmount.ToString());

    } 
}
