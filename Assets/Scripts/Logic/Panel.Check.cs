using System;
using System.Collections;
using System.Collections.Generic;

namespace DragCube.Logic
{
    public partial class Panel
    {
        private void tagClosePath(int r, int c)
        {
            List<Pos> poses = new List<Pos>(); // 记录路径
            Direct direct = Direct.N;  // 记录前进方向
            bool isCircle = (this.Cubes[r, c].ConnectionNum() == 2);

            Pos startPos = new Pos(r, c);
            Pos nowPos = new Pos(0, 0);

            nowPos.Copy(startPos);

            do
            {
                Cube me = this.Cubes[nowPos.R, nowPos.C];
                poses.Add(new Pos(nowPos));

                // 向上走一步。上一步向下的话这一步就不能向上
                if (direct != Direct.D && me.IsConnectTo(Direct.U))
                {
                    if (nowPos.R <= 0)
                        return;

                    Cube target = this.Cubes[nowPos.R - 1, nowPos.C];
                    if (!target.IsConnectTo(Direct.D))
                        return;

                    nowPos.R--;
                    direct = Direct.U;
                    continue;
                }

                // 向下走一步。上一步向上的话这一步就不能向下
                if (direct != Direct.U && me.IsConnectTo(Direct.D))
                {
                    if (nowPos.R >= this.Row - 1)
                        return;

                    Cube target = this.Cubes[nowPos.R + 1, nowPos.C];
                    if (!target.IsConnectTo(Direct.U))
                        return;

                    nowPos.R++;
                    direct = Direct.D;
                    continue;
                }

                // 向左走一步。上一步向右的话这一步就不能向左
                if (direct != Direct.R && me.IsConnectTo(Direct.L))
                {
                    if (nowPos.C <= 0)
                        return;

                    Cube target = this.Cubes[nowPos.R, nowPos.C - 1];
                    if (!target.IsConnectTo(Direct.R))
                        return;

                    nowPos.C--;
                    direct = Direct.L;
                    continue;
                }

                // 向右走一步。上一步向左的话这一步就不能向右
                if (direct != Direct.L && me.IsConnectTo(Direct.R))
                {
                    if (nowPos.C >= this.Col - 1)
                        return;

                    Cube target = this.Cubes[nowPos.R, nowPos.C + 1];
                    if (!target.IsConnectTo(Direct.L))
                        return;

                    nowPos.C++;
                    direct = Direct.R;
                    continue;
                }

                // 走到这里说明走不下去了，如果不是环路则应该消除
                if (!isCircle)
                    break;

                return;
            } while (!nowPos.Equal(startPos));

            foreach (Pos p in poses)
                this.Cubes[p.R, p.C].Flag = true;
        }

        public bool CannotPlayAnyMore()
        {
            bool hasUL = false;
            bool hasUR = false;
            bool hasDL = false;
            bool hasDR = false;
            int UNum = 0;
            int DNum = 0;
            int LNum = 0;
            int RNum = 0;

            // 先统计各种块
            for (int r = 0; r < this.Row; r++)
            {
                for (int c = 0; c < this.Col; c++)
                {
                    Cube cube = this.Cubes[r, c];
                    if (cube.ConnectionNum() == 1)
                    {
                        if (cube.IsConnectTo(Direct.U)) UNum++;
                        else if (cube.IsConnectTo(Direct.D)) DNum++;
                        else if (cube.IsConnectTo(Direct.L)) LNum++;
                        else if (cube.IsConnectTo(Direct.R)) RNum++;
                    }
                    else if(cube.ConnectionNum() == 2)
                    {
                        if (cube.IsConnectTo(Direct.U) && cube.IsConnectTo(Direct.L)) hasUL = true;
                        else if (cube.IsConnectTo(Direct.U) && cube.IsConnectTo(Direct.R)) hasUR = true;
                        else if (cube.IsConnectTo(Direct.D) && cube.IsConnectTo(Direct.L)) hasDL = true;
                        else if (cube.IsConnectTo(Direct.D) && cube.IsConnectTo(Direct.R)) hasDR = true;
                    }
                }
            }

            // 把可以继续玩需要的各种最小集排除
            if ((UNum > 0 && DNum > 0) || (LNum > 0 && RNum > 0))
                return false;

            if (hasUL && hasUR && hasDL && hasDR)
                return false;

            if (UNum > 0 && LNum > 0 && hasDR)
                return false;

            if (UNum > 0 && RNum > 0 && hasDL)
                return false;

            if (DNum > 0 && LNum > 0 && hasUR)
                return false;

            if (DNum > 0 && RNum > 0 && hasUL)
                return false;

            if (UNum >= 2 && hasDL && hasDR)
                return false;

            if (DNum >= 2 && hasUL && hasUR)
                return false;

            if (LNum >= 2 && hasUR && hasDR)
                return false;

            if (RNum >= 2 && hasUL && hasDL)
                return false;

            return true;
        }
    }
}
