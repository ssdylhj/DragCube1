using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DragCube.Logic
{
    public class Cube
    {
        public int DirectType { get; private set; }
        public bool Flag;

        public Cube()
        {
            this.DirectType = (int) Direct.N;
        }

        public Cube(int dType)
        {
            this.DirectType = dType;
            this.Flag = false;
        }

        public bool IsConnectTo(Direct direct)
        {
            return (this.DirectType & (int) direct) != 0;
        }

        // 该方块上有几条连线
        public int ConnectionNum()
        {
            int num = 0;
            if ((this.DirectType & (int) Direct.U) != 0) num++;
            if ((this.DirectType & (int) Direct.D) != 0) num++;
            if ((this.DirectType & (int) Direct.L) != 0) num++;
            if ((this.DirectType & (int) Direct.R) != 0) num++;
            return num;
        }
    }
}
