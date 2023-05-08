using System;

namespace Pattern.MVO
{
    public sealed class HitPointsPanelAdapter
    {
        private IHitPointComponent _characterComponent;
        private CommonPanel _characterParameterPanel;
        
        public HitPointsPanelAdapter(IHitPointComponent character, CommonPanel parameterPanel)
        {
            _characterComponent = character;
            _characterParameterPanel = parameterPanel;
        }

        public void Enable()
        {
            _characterComponent.onHitPointChanged += OnValueChanged;
            _characterParameterPanel.SetupValue(_characterComponent.GetHitPoints().ToString());
        }
        public void Disable()
        {
            _characterComponent.onHitPointChanged -= OnValueChanged;
        }

        private void OnValueChanged(float newValue) => _characterParameterPanel.UpdateValue(newValue.ToString());

    }
}

