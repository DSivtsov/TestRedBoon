using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public class GemsSystemInstaller : MonoInstaller
    {
        [SerializeField] private GemsStorage _gemsStorage;

        public override void InstallBindings()
        {
            this.Container.Bind<GemsStorage>().FromInstance(_gemsStorage).AsSingle();
        }
    }
}
