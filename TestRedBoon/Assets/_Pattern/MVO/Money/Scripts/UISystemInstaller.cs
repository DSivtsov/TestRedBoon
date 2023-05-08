using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public sealed class UISystemInstaller : MonoContext
    {
        [SerializeField] private ItemPanel _moneyPanel;
        [SerializeField] private ItemPanel _gemsPanel;

        private GemsPanelAdapter _gemsPanelAdapter;
        private MoneyPanelAdapter _moneyPanelAdapter;

        [Inject]
        public void Contsruct(MoneyStorage moneyStorage, GemsStorage gemsStorage)
        {
            _gemsPanelAdapter = new GemsPanelAdapter(gemsStorage, _gemsPanel);
            _moneyPanelAdapter = new MoneyPanelAdapter(moneyStorage, _moneyPanel);
        }

        protected override IEnumerable<object> ProvidedComponents()
        {
            yield return _gemsPanelAdapter;
            yield return _moneyPanelAdapter;
        }
    }
}