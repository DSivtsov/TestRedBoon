using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{
    public sealed class GemsPanelAdapter
    {
        private GemsStorage _gemsStorage;
        ItemPanel _gemsPanel;

        public GemsPanelAdapter(GemsStorage itemStorage, ItemPanel itemPanel)
        {
            _gemsStorage = itemStorage;
            _gemsPanel = itemPanel;
        }

        public void Enable()
        {
            _gemsStorage.OnMoneyChanged += OnMoneyChanged;
            _gemsStorage.OnMoneySet += OnMoneySet;
            _gemsPanel.SetupMoney(_gemsStorage.Gems.ToString());
        }
        public void Disable()
        {
            _gemsStorage.OnMoneyChanged -= OnMoneyChanged;
            _gemsStorage.OnMoneySet -= OnMoneySet;
        }

        private void OnMoneySet(int ammount) => _gemsPanel.SetupMoney(ammount.ToString());


        private void OnMoneyChanged(int newAmmount) => _gemsPanel.UpdateMoney(newAmmount.ToString());

    }
}
