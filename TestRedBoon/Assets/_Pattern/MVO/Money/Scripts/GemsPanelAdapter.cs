using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public sealed class GemsPanelAdapter : IEnableComponent, IDisableComponent
    {
        private GemsStorage _gemsStorage;
        ItemPanel _gemsPanel;

        public GemsPanelAdapter(GemsStorage itemStorage, ItemPanel itemPanel)
        {
            _gemsStorage = itemStorage;
            _gemsPanel = itemPanel;
        }

        private void OnMoneySet(int ammount) => _gemsPanel.SetupMoney(ammount.ToString());

        private void OnMoneyChanged(int newAmmount) => _gemsPanel.UpdateMoney(newAmmount.ToString());

        void IEnableComponent.OnEnable()
        {
            _gemsStorage.OnMoneyChanged += OnMoneyChanged;
            _gemsStorage.OnMoneySet += OnMoneySet;
            _gemsPanel.SetupMoney(_gemsStorage.Gems.ToString());
        }

        void IDisableComponent.OnDisable()
        {
            _gemsStorage.OnMoneyChanged -= OnMoneyChanged;
            _gemsStorage.OnMoneySet -= OnMoneySet;
        }
    }
}
