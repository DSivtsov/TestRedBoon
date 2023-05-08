namespace Pattern.MVO
{
    public sealed class DamagePanelAdapter
    {
        private IDamageComponent _characterComponent;
        private CommonPanel _characterParameterPanel;

        public DamagePanelAdapter(IDamageComponent character, CommonPanel parameterPanel)
        {
            _characterComponent = character;
            _characterParameterPanel = parameterPanel;
        }

        public void Enable()
        {
            _characterComponent.onDamageChanged+= OnValueChanged;
            _characterParameterPanel.SetupValue(_characterComponent.GetDamange().ToString());
        }
        public void Disable()
        {
            _characterComponent.onDamageChanged -= OnValueChanged;
        }

        private void OnValueChanged(float newValue) => _characterParameterPanel.UpdateValue(newValue.ToString());

    }
}
