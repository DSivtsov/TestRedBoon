using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameEngine.Inventory
{
    public sealed class InventoryItemView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private Button button;
        
        public void LinkVisulItemWithSO(InventoryItemSO itemSO)
        {
            SetTitle(AmmountFormatter.GetAmmount(itemSO.PuchasePrice));
            SetIcon(itemSO.Metadata.icon);
        }

        public void SetTitle(string title)
        {
            this.titleText.text = title;
        }

        public void SetIcon(Sprite icon)
        {
            this.iconImage.sprite = icon;
        }

        //public void AddClickListener(UnityAction action)
        //{
        //    this.button.onClick.AddListener(action);
        //}

        //public void RemoveClickListener(UnityAction action)
        //{
        //    this.button.onClick.RemoveListener(action);
        //}
    }
}