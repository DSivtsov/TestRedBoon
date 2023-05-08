using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Pattern.MVO
{
    public class MoneyWidget : MonoBehaviour
    {
        private MoneyStorage _moneyStorage;
        [SerializeField] UnityEngine.UI.Text _moneyTextObj;

        [Inject]
        public void Construct(MoneyStorage moneyStorage)
        {
            _moneyStorage = moneyStorage;
        }

        private void OnEnable()
        {
            _moneyStorage.OnMoneyChanged += OnMoneyChanged;
            UpdateText(_moneyStorage.Money);
        }

        private void OnDisable()
        {
            _moneyStorage.OnMoneyChanged -= OnMoneyChanged;
        }

        private void OnMoneyChanged(int newAmmount) => UpdateText(newAmmount);

        private void UpdateText(int ammount)
        {
            _moneyTextObj.text = ammount.ToString();
            AnimatedMoney();
        }

        private void AnimatedMoney()
        {
            DOTween
                .Sequence()
                .Append(this._moneyTextObj.transform.DOScale(1.1f, 0.1f))
                .Append(this._moneyTextObj.transform.DOScale(1.0f, 0.3f));
        }
    } 
}
