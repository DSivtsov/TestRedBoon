using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public class MoneySystemInstaller : MonoInstaller
    {
        [SerializeField] private MoneyStorage _moneyStorage;

        public override void InstallBindings()
        {
            //_moneyStorage = new MoneyStorage(1999);
            this.Container.Bind<MoneyStorage>().FromInstance(_moneyStorage).AsSingle();
            //Container.Bind<MoneyStorage>().FromNew().AsSingle()
            //    .WithArguments(400);
        }
    } 
}
