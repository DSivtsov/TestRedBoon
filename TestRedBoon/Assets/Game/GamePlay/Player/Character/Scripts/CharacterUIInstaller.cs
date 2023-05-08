using System;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Player
{

    public interface IHitPointComponent
    {
        public int GetHitPoints();

        public event Action<float> onHitPointChanged;

    }

    public interface IDamageComponent
    {
        public int GetDamange();

        public event Action<float> onDamageChanged;

    }

    public interface ISpeedComponent
    {
        public float GetSpeed();

        public event Action<float> onSpeedChanged;

    }

    public sealed class CharacterUIInstaller : MonoBehaviour
    {
        [SerializeField] private CommonPanel _hitPointPanel;
        [SerializeField] private CommonPanel _damangePanel;
        [SerializeField] private CommonPanel _speedPanel;

        private HitPointsPanelAdapter _hitPointsPanelAdapter;
        private DamagePanelAdapter _damangePanelAdapter;
        private SpeedPanelAdapter _speedPanelAdapter;

        private Character _character;
        
        [Inject]
        public void Contsruct(Character character)
        {
            _character = character;
        }

        private void Awake()
        {
            _hitPointsPanelAdapter = new HitPointsPanelAdapter(_character, _hitPointPanel);
            _damangePanelAdapter = new DamagePanelAdapter(_character, _damangePanel);
            _speedPanelAdapter = new SpeedPanelAdapter(_character, _speedPanel);
        }


        private void OnEnable()
        {
            _hitPointsPanelAdapter.Enable();
            _damangePanelAdapter.Enable();
            _speedPanelAdapter.Enable();
        }

        private void OnDisable()
        {
            _hitPointsPanelAdapter.Disable();
            _damangePanelAdapter.Disable();
            _speedPanelAdapter.Disable();
        }
    }
}
