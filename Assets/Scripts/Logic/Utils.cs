using System;
using System.Collections;
using System.Collections.Generic;

namespace DragCube.Logic
{
    public enum Direct
    {
        N = 0,
        U = 1,
        R = 2,
        D = 4,
        L = 8,
    }

    public enum HDirection
    {
        None,
        Horizon,
        Vertical
    }

    public class Pos
    {
        public int R;
        public int C;

        public Pos(Pos pos)
        {
            this.R = pos.R;
            this.C = pos.C;
        }

        public Pos(int r, int c)
        {
            this.R = r;
            this.C = c;
        }

        public void Copy(Pos pos)
        {
            this.R = pos.R;
            this.C = pos.C;
        }

        public bool Equal(Pos p)
        {
            return p.R == this.R && p.C == this.C;
        }
    }

    public static class Utils
    {
        private static Random gRandom = new Random();

        public static int Random(int max)
        {
            return gRandom.Next(max);
        }

        public static int Random(int min, int max)
        {
            return gRandom.Next(min, max);
        }

        public static HDirection ToHDirection(this Direct direct)
        {
            switch (direct)
            {
                case Direct.D:
                case Direct.U:
                    return HDirection.Vertical;
                case Direct.L:
                case Direct.R:
                    return HDirection.Horizon;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    return HDirection.None;
            }
        }
    }
}
