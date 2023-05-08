using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattern.MVO
{
    public  interface IEnableComponent
    {
        void OnEnable();
    }
    public interface IDisableComponent
    {
        void OnDisable();
    }
    public class MonoContext : MonoBehaviour
    {
        private List<IEnableComponent> _enableComponents = new List<IEnableComponent>();
        private List<IDisableComponent> _disableComponents = new List<IDisableComponent>();

        private void Awake() => AddComponents(ProvidedComponents());

        private void AddComponents(IEnumerable<object> providedComponents)
        {
            foreach (var component in providedComponents)
            {
                if (component is IEnableComponent enableComponent)
                {
                    _enableComponents.Add(enableComponent);
                }
                if (component is IDisableComponent disableComponent)
                {
                    _disableComponents.Add(disableComponent);
                }
            }
        }

        protected virtual IEnumerable<object> ProvidedComponents()
        {
            yield break;
        }

        private void OnEnable()
        {
            foreach (IEnableComponent component in _enableComponents)
                component.OnEnable();
        }

        private void OnDisable()
        {
            foreach (IDisableComponent component in _disableComponents)
                component.OnDisable();
        }
    } 
}
