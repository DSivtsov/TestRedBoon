using Game.Gameplay.Player;
using Sirenix.OdinInspector;
using System;
using UnityEngine;


public class Character : MonoBehaviour, IHitPointComponent, IDamageComponent, ISpeedComponent
{
    public event Action<float> onHitPointChanged;
    public event Action<float> onDamageChanged;
    public event Action<float> onSpeedChanged;

    [SerializeField] private int _hitPoints;
    [SerializeField] private int _damange;
    [SerializeField] private float _speed;

    public int GetHitPoints() => _hitPoints;

    public int GetDamange() => _damange;

    public float GetSpeed() => _speed;
    
    [Button]
    public void SetHitPoints(int hitPoints)
    {
        _hitPoints = hitPoints;
        onHitPointChanged?.Invoke(hitPoints);
    }
    
    [Button]
    public void SetDamange(int damange)
    {
        _damange = damange;
        onDamageChanged?.Invoke(damange);
    }

    [Button]
    public void SetSpeed(int speed)
    {
        _speed = speed;
        onSpeedChanged?.Invoke(speed);
    }

}

