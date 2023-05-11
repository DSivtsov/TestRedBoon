using GameEngine.Wallet;
using GameEngine.Inventory;

namespace GameEngine.Character
{
    public interface ITrader
    {
        InventoryItemList InventoryItemList { get; }
        WalletStorage WalletStorage { get; }
    }
}
