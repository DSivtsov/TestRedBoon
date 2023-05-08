using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{
    public class GemsPanelAdapter : MonoBehaviour
    {
        private GemsStorage _gemsStorage;
        [SerializeField] ItemPanel _gemsPanel;

        [Inject]
        public void Construct(GemsStorage moneyStorage)
        {
            _gemsStorage = moneyStorage;
        }

        private void OnEnable()
        {
            _gemsStorage.OnMoneyChanged += OnMoneyChanged;
            _gemsStorage.OnMoneySet += OnMoneySet;
            _gemsPanel.SetupMoney(_gemsStorage.Gems.ToString());
        }
        private void OnDisable()
        {
            _gemsStorage.OnMoneyChanged -= OnMoneyChanged;
            _gemsStorage.OnMoneySet -= OnMoneySet;
        }

        private void OnMoneySet(int ammount) => _gemsPanel.SetupMoney(ammount.ToString());


        private void OnMoneyChanged(int newAmmount) => _gemsPanel.UpdateMoney(newAmmount.ToString());

    }
}
