using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragCube.Logic;

namespace DragCube.Client
{
    public static class Utils
    {
        public static Direct ToDirection(this Vector2 dir)
        {
            if (Mathf.Abs(dir.x) < float.Epsilon &&
                Mathf.Abs(dir.y) < float.Epsilon)
                return Direct.N;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                return dir.x < 0 ? Direct.L : Direct.R;
            else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                return dir.y < 0 ? Direct.D : Direct.U;

            return Direct.N;
        }
    }
}
