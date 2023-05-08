using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public class MoneyPanelAdapter : IEnableComponent, IDisableComponent
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

        private void OnMoneySet(int ammount) => _moneyPanel.SetupMoney(ammount.ToString());

        private void OnMoneyChanged(int newAmmount) => _moneyPanel.UpdateMoney(newAmmount.ToString());

        void IEnableComponent.OnEnable()
        {
            _moneyStorage.OnMoneyChanged += OnMoneyChanged;
            _moneyStorage.OnMoneySet += OnMoneySet;
            _moneyPanel.SetupMoney(_moneyStorage.Money.ToString());
        }

        void IDisableComponent.OnDisable()
        {
            _moneyStorage.OnMoneyChanged -= OnMoneyChanged;
            _moneyStorage.OnMoneySet -= OnMoneySet;
        }
    } 
}
