using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public class Finder : IPathFinder
    {
        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            List<Vector2> gotPatch = new List<Vector2>();
            //Debug.Log(this);
            return gotPatch;
        }
    }
}