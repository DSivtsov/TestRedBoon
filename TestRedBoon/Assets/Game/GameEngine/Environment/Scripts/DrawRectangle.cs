using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameEngine.PathFinder
{
    public class DrawRectangle : MonoBehaviour
    {
        [SerializeField] private Transform _rectanglePrefab;

        /// <summary>
        /// Build new Rectangle GameObject 
        /// </summary>
        /// <param name="normalizedRectangle">NormalizedRectangle</param>
        /// <param name="nameNewRectangle">name new GameObject</param>
        [Button]
        public void Draw(NormalizedRectangle normalizedRectangle, string nameNewRectangle = "NewRectangle")
        {
            Transform transformRectangle = Instantiate<Transform>(_rectanglePrefab);
            transformRectangle.position = normalizedRectangle.BottomLeftAngel;
            transformRectangle.localScale = normalizedRectangle.SizeXY;
            transformRectangle.name = nameNewRectangle;
            //transformRectangle.GetComponent<LineRenderer>().widthMultiplier = 5;
        }
    } 
}
