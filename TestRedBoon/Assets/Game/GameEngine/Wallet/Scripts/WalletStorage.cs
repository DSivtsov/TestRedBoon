using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace GameEngine.Wallet
{
    [Serializable]
    public sealed class WalletStorage
    {
        public event Action<int> OnMoneyChanged;

        public event Action<int> OnMoneyEarned;

        public event Action<int> OnMoneySpent;

        public event Action<int> OnMoneySet;

        public int Money
        {
            get { return this._money; }
        }

        [ReadOnly]
        [ShowInInspector]
        private int _money;

        public WalletStorage(int money)
        {
            this._money = (money > 0) ? money : 0;
        }

        [Title("Methods")]
        [Button]
        [GUIColor(0, 1, 0)]
        public void EarnMoney(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                throw new Exception($"Can not earn negative money {amount}");
            }

            var previousValue = this._money;
            var newValue = previousValue + amount;

            this._money = newValue;
            this.OnMoneyChanged?.Invoke(newValue);
            this.OnMoneyEarned?.Invoke(amount);
        }

        [Button]
        [GUIColor(0, 1, 0)]
        public void SpendMoney(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            if (amount < 0)
            {
                throw new Exception($"Can not spend negative money {amount}");
            }

            var previousValue = this._money;
            var newValue = previousValue - amount;
            if (newValue < 0)
            {
                throw new Exception(
                    $"Negative money after spend. Money in bank: {previousValue}, spend amount {amount} ");
            }

            this._money = newValue;
            this.OnMoneyChanged?.Invoke(newValue);
            this.OnMoneySpent?.Invoke(amount);
        }

        [Button]
        [GUIColor(0, 1, 0)]
        public void SetupMoney(int amount)
        {
            if (amount < 0)
            {
                throw new Exception($"Can not set negative money amount {amount} to bank");
            }

            this._money = amount;
            this.OnMoneySet?.Invoke(amount);
        }

        public bool CanSpendMoney(int amount) => this._money >= amount;
    }
}
