using DG.Tweening;
using UnityEngine;

namespace Game.Gameplay.Player
{
    public class ItemPanel : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Text _itemTextObj;

        public void SetupMoney(string value)
        {
            _itemTextObj.text = value;
        }

        public void UpdateMoney(string newValue)
        {
            _itemTextObj.text = newValue;
            AnimatedMoney();
        }

        private void AnimatedMoney()
        {
            DOTween
                .Sequence()
                .Append(_itemTextObj.transform.DOScale(1.1f, 0.1f))
                .Append(_itemTextObj.transform.DOScale(1.0f, 0.3f));
        }
    } 
}
