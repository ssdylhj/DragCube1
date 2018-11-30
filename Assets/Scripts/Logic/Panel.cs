using System;
using System.Collections;
using System.Collections.Generic;

namespace DragCube.Logic
{
    public partial class Panel
    {
        public Cube[,] Cubes { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Panel()
        {
            this.Row = 5;
            this.Col = 5;
            this.Cubes = new Cube[this.Row, this.Col];

            init();
        }
        
        private void init()
        {
            for (int r = 0; r < this.Row; r++)
            {
                for (int c = 0; c < this.Col; c++)
                    this.Cubes[r, c] = generateCube();
            }
        }

        public void Reset()
        {
            init();
        }

        public void MoveUp(int c)
        {
            Cube temp = this.Cubes[0, c];
            for (int r = 0; r < this.Row - 1; r++)
                this.Cubes[r, c] = this.Cubes[r + 1, c];

            this.Cubes[this.Row - 1, c] = temp;
        }

        public void MoveDown(int c)
        {
            Cube temp = this.Cubes[this.Row - 1, c];
            for (int r = this.Row - 1; r > 0; r--)
                this.Cubes[r, c] = this.Cubes[r - 1, c];

            this.Cubes[0, c] = temp;
        }

        public void MoveLeft(int r)
        {
            Cube temp = this.Cubes[r, 0];
            for (int c = 0; c < this.Col - 1; c++)
                this.Cubes[r, c] = this.Cubes[r, c + 1];

            this.Cubes[r, this.Col - 1] = temp;
        }

        public void MoveRight(int r)
        {
            Cube temp = this.Cubes[r, this.Col - 1];
            for (int c = this.Col - 1; c > 0; c--)
                this.Cubes[r, c] = this.Cubes[r, c - 1];

            this.Cubes[r, 0] = temp;
        }

        public void ScanAndDelCubes(Action<int, int, Cube> todo)
        {
            for (int r = 0; r < this.Row; r++)
            {
                for (int c = 0; c < this.Col; c++)
                {
                    if (this.Cubes[r, c].Flag == true)
                        continue;

                    tagClosePath(r, c);
                }
            }

            for (int r = 0; r < this.Row; r++)
            {
                for (int c = 0; c < this.Col; c++)
                {
                    if (this.Cubes[r, c].Flag == true)
                    {
                        Cube cube;
                        delCube(r, c, out cube);

                        todo(r, c, cube);
                    }
                }
            }
        }

        private void delCube(int row, int col, out Cube cube)
        {
            // 上方依次下落
            for (int r = row; r > 0; r--)
                this.Cubes[r, col] = this.Cubes[r - 1, col];

            // 重新生成一个在最上方
            cube = generateCube();
            this.Cubes[0, col] = cube;
        }

        private Cube generateCube()
        {
            int dType= 0;

            int dNum = 4;
            int delta = Utils.Random(dNum);
            dType |= 1 << delta;
            if (Utils.Random(2) == 0)
                dType |= 1 << (delta + 1 % dNum);

            Cube cube = new Cube(dType);
            return cube;
        }
    }
}
