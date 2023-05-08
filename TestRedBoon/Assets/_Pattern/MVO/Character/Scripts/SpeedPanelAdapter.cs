namespace Pattern.MVO
{
    public sealed class SpeedPanelAdapter
    {
        private ISpeedComponent _characterComponent;
        private CommonPanel _characterParameterPanel;

        public SpeedPanelAdapter(ISpeedComponent character, CommonPanel parameterPanel)
        {
            _characterComponent = character;
            _characterParameterPanel = parameterPanel;
        }

        public void Enable()
        {
            _characterComponent.onSpeedChanged += OnValueChanged;
            _characterParameterPanel.SetupValue(_characterComponent.GetSpeed().ToString());
        }
        public void Disable()
        {
            _characterComponent.onSpeedChanged -= OnValueChanged;
        }

        private void OnValueChanged(float newValue) => _characterParameterPanel.UpdateValue(newValue.ToString());

    }
}
