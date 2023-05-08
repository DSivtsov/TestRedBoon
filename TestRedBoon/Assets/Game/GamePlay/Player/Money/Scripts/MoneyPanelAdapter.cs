using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{
    public class MoneyPanelAdapter : MonoBehaviour
    {
        private MoneyStorage _moneyStorage;
        [SerializeField] ItemPanel _moneyPanel;

        [Inject]
        public void Construct(MoneyStorage moneyStorage)
        {
            _moneyStorage = moneyStorage;
        }

        private void OnEnable()
        {
            _moneyStorage.OnMoneyChanged += OnMoneyChanged;
            _moneyStorage.OnMoneySet += OnMoneySet;
            _moneyPanel.SetupMoney(_moneyStorage.Money.ToString());
        }
        private void OnDisable()
        {
            _moneyStorage.OnMoneyChanged -= OnMoneyChanged;
            _moneyStorage.OnMoneySet -= OnMoneySet;
        }

        private void OnMoneySet(int ammount) => _moneyPanel.SetupMoney(ammount.ToString());


        private void OnMoneyChanged(int newAmmount) => _moneyPanel.UpdateMoney(newAmmount.ToString());

    } 
}
