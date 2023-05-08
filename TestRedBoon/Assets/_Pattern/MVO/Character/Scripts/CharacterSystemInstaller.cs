using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pattern.MVO
{
    public class CharacterSystemInstaller : MonoInstaller
    {
        [SerializeField] private Character _character;

        public override void InstallBindings()
        {
            this.Container.Bind<Character>().FromInstance(_character).AsSingle();
        }
    }
}
