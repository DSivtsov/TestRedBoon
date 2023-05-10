using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Pattern.MVO;
using Game.GameEngine.Inventory;
using System;

namespace Pattern.MVP
{
    public class ProductSystemInstaller : MonoInstaller
    {
        [SerializeField] private InventoryItemList _inventoryItemList;
        // To exclude possibility the circular reference [INJECT] can't be use in Bindings file, use OnInstantiated
        [SerializeField] private ProductSystem _productSystem;

        public override void InstallBindings()
        {

            Container.Bind<InventoryItemList>().FromInstance(_inventoryItemList).AsSingle();
            //In case of cretion new object by Zenject, it fills ctor by known binding automatically
            Container.Bind<ProductSystem>().FromNew().AsSingle().OnInstantiated<ProductSystem>((_, newInstance) => _productSystem = newInstance).NonLazy();

        }
    }
}
