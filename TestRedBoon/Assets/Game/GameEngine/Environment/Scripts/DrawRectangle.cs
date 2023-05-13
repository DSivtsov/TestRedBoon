using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DrawRectangle : MonoBehaviour
{
    [SerializeField] private Transform _rectanglePrefab;

    [SerializeField] private Transform _transformParent;

    //private void Awake()
    //{
    //    Draw(new Vector2(), new Vector2(100, 100));
    //}
    /// <summary>
    /// Build new Rectangle GameObject 
    /// </summary>
    /// <param name="bottomLeftAngle">BottomLeftAngle</param>
    /// <param name="widthHeight">TopRightAngle</param>
    /// <param name="nameNewRectangle">name new GameObject</param>
    [Button]
    public void Draw(Vector2 bottomLeftAngle, Vector2 widthHeight, string nameNewRectangle = "NewRectangle")
    {
        if (widthHeight.x * widthHeight.y == 0)
            widthHeight = Vector2.one;
        //Transform transformRectangle = Instantiate<Transform>(_rectanglePrefab, bottomLeftAngle, Quaternion.identity, _transformParent);
        Transform transformRectangle = Instantiate<Transform>(_rectanglePrefab);
        transformRectangle.position = bottomLeftAngle;
        transformRectangle.localScale = widthHeight;
        transformRectangle.name = nameNewRectangle;
        //transformRectangle.GetComponent<LineRenderer>().widthMultiplier = 5;
    }
}
