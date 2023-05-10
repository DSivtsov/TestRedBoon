using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using Pattern.MVP;

public class TestProductSystem : MonoBehaviour
{
    [Inject]
    [ShowInInspector]
    ProductSystem _productSystem;
}
