using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Pattern.MVO;

namespace Pattern.MVP
{
    public class ProductSystemInstaller : MonoInstaller
    {
        [SerializeField] private MoneyStorage _moneyStorage;
        [SerializeField] private ProductSystem _productSystem;

        public override void InstallBindings()
        {

            this.Container.Bind<ProductSystem>().FromNew().AsSingle();

        }
    }
}
