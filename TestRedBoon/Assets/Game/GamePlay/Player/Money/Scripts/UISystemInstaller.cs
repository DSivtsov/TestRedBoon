using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{
    public sealed class UISystemInstaller : MonoBehaviour
    {
        [SerializeField] private ItemPanel _moneyPanel;
        [SerializeField] private ItemPanel _gemsPanel;

        private GemsStorage _gemsStorage;
        private MoneyStorage _moneyStorage;

        private GemsPanelAdapter _gemsPanelAdapter;
        private MoneyPanelAdapter _moneyPanelAdapter;

        [Inject]
        public void Contsruct(MoneyStorage moneyStorage, GemsStorage gemsStorage)
        {
            _gemsPanelAdapter = new GemsPanelAdapter(gemsStorage, _gemsPanel);
            _moneyPanelAdapter = new MoneyPanelAdapter(moneyStorage, _moneyPanel);
        }


        private void OnEnable()
        {
            _gemsPanelAdapter.Enable();
            _moneyPanelAdapter.Enable();
        }

        private void OnDisable()
        {
            _gemsPanelAdapter.Disable();
            _moneyPanelAdapter.Disable();
        }
    }
}