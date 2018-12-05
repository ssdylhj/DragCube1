using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragCube.Logic;
using DragCube.Client;

namespace DrugCube.Client
{
    public class GameComponent : MonoBehaviour
    {

        [SerializeField]
        private GameObject CubeOb;

        public Panel Panel { get; private set; }

        private List<CubeComponent> cubes = new List<CubeComponent>();

        private void Awake()
        {
            this.Panel = new Panel();
            Init();
        }

        private void Init()
        {
            var w = CubeComponent.kCubeWidth * this.Panel.Col;
            var h = CubeComponent.kCubeHeight * this.Panel.Row;

            for (var r = 0; r < this.Panel.Row; r++)
            {
                for (var c = 0; c < this.Panel.Col; c++)
                {
                    var go = Instantiate(this.CubeOb);
                    go.transform.SetParent(this.transform, false);
                    go.SetActive(true);

                    var cubeComponent = go.GetComponent<CubeComponent>();
                    cubeComponent.Init(w, h);
                    cubeComponent.SetLocalPostion(this.GetCubePosition(r, c));
                    cubeComponent.OnDrag = this.Drag;
                    cubeComponent.OnDragEnd = this.HandleEndDrag;
                    cubeComponent.OnClick = this.HandleOnClick;
                    cubeComponent.OnDown = this.HandleOnDown;
                    cubeComponent.OnUp = this.HandleOnUp;

                    cubes.Add(cubeComponent);
                }
            }

            this.Refresh();
        }

        private bool dragFlag = false;
        private HDirection DragDirection;
        private readonly List<CubeComponent> DragCubes = new List<CubeComponent>();
        private Vector2? LastPos = null;
        private void Drag(int r, int c, Vector2 pos)
        {
            if (!this.dragFlag)
                this.dragFlag = true;

            Vector2 localPos = this.transform.InverseTransformPoint(pos);
            var v = localPos - this.DownPosition;
            var d = v.ToDirection().ToHDirection();

            if (this.DragDirection != HDirection.None && this.DragDirection != d)
            {
                return;
            }

            if (this.DragDirection == HDirection.None)
            {
                this.DragDirection = v.ToDirection().ToHDirection();
            }

            if (this.DragDirection != HDirection.None)
                this.InitCubeBeDrag(r, c, this.DragDirection);

            this.DragMove(localPos);
        }

        private void DragMove(Vector2 pos)
        {
            var delta = this.ClampDelta(pos - this.LastPos.Value);
            this.LastPos = pos;
               
            foreach (var dc in this.DragCubes)
            {
                dc.Move(delta);
            }
        }

        private Vector2 ClampDelta(Vector2 delta)
        {
            if (this.DragDirection == HDirection.Horizon)
            {
                delta.y = 0;
                delta.x = Mathf.Abs(delta.x) < 5 ? delta.x : Mathf.Sign(delta.x) * 5;

            }
            else if (this.DragDirection == HDirection.Vertical)
            {
                delta.x = 0;
                delta.y = Mathf.Abs(delta.y) < 5 ? delta.y : Mathf.Sign(delta.y) * 5;
            }

            return delta;
        }

        private void InitCubeBeDrag(int r, int c, HDirection direct)
        {
            this.DragCubes.Clear();

            foreach(var cube in this.cubes)
            {
                if (direct == HDirection.Horizon && cube.Row == r)
                    this.DragCubes.Add(cube);

                if (direct == HDirection.Vertical && cube.Col == c)
                    this.DragCubes.Add(cube);
            }
        }

        private void HandleEndDrag(int r, int c, Vector2 pos)
        {
            do
            {
                Vector2 localPos = this.transform.InverseTransformPoint(pos);
                var distance = localPos - this.DownPosition;

                var moveCount = 0;
                if (this.DragDirection == HDirection.None)
                    break;
                else if (this.DragDirection == HDirection.Horizon)
                    moveCount = Mathf.FloorToInt(Mathf.Abs(distance.x) / CubeComponent.kCubeWidth);
                else if (this.DragDirection == HDirection.Vertical)
                    moveCount = Mathf.FloorToInt(Mathf.Abs(distance.y) / CubeComponent.kCubeHeight);

                for (var i = 0; i < moveCount; ++i)
                    this.LogicMove(distance.ToDirection(), r, c);

                this.Refresh();
            } while (false);

            this.dragFlag = false;
            this.LastPos = null;
            this.DragDirection = HDirection.None;
            this.DelCubes();
            //this.Refresh();
        }

        private void LogicMove(Direct direct, int r, int c)
        {
            switch (direct)
            {
                case Direct.U: this.Panel.MoveUp(c); break;
                case Direct.D: this.Panel.MoveDown(c); break;
                case Direct.L: this.Panel.MoveLeft(r); break;
                case Direct.R: this.Panel.MoveRight(r); break;
                default: break;
            }
        }

        private Dictionary<int, List<int>> delCubePoses = new Dictionary<int, List<int>>();
        private Dictionary<int, List<Cube>> newCubes = new Dictionary<int, List<Cube>>();
        private void DelCubes()
        {
            delCubePoses = new Dictionary<int, List<int>>();
            newCubes = new Dictionary<int, List<Cube>>();
            this.Panel.ScanAndDelCubes(
                (r, c, cube) => {
                    if (! delCubePoses.ContainsKey(c))
                        delCubePoses.Add(c, new List<int>());
                    delCubePoses[c].Add(r);

                    if (! newCubes.ContainsKey(c))
                        newCubes.Add(c, new List<Cube>());
                    newCubes[c].Add(cube);
                }
            );

            this.ShowAnimation();
        }

        private float? animationTime = null;
        private void ShowAnimation()
        {
            foreach (var kvp in delCubePoses)
            {
                foreach (var r in kvp.Value)
                {
                    this.animationTime = 1f;
                    var cc = this.GetCube(r, kvp.Key);
                    cc.Animator.SetTrigger("Dispear");
                }
            }

            foreach (var kvp in newCubes)
                foreach(var c in kvp.Value)
                Debug.Log(string.Format("new cube {0} be created!", c));
        }

        private void CollectDelInfo(int r, int c, Cube cube)
        {

        }

        private Vector2 DownPosition = Vector2.zero;
        private void HandleOnDown(Vector2 pos)
        {
            this.DownPosition = this.transform.InverseTransformPoint(pos);
            this.LastPos = DownPosition;
            dragFlag = true;

            Debug.Log(string.Format("Down Pos: {0}", this.DownPosition));
        }

        private void HandleOnUp()
        {
            dragFlag = false;
        }

        private void HandleOnClick(int r, int c)
        {
            //this.Panel.ScanAndDelCubes();
            UnityEngine.Debug.LogWarningFormat("{0}", this.Panel.CannotPlayAnyMore());
        }

        private void Refresh()
        {
            for(var r = 0; r < this.Panel.Row; ++r)
            {
                for(var c=0; c < this.Panel.Col;++c)
                {
                    var idx = this.Panel.Col * r + c;
                    this.cubes[idx].SetLocalPostion(this.GetCubePosition(r, c));
                    this.cubes[idx].Refresh(this.Panel.Cubes[r, c]);
                }
            }
        }

        private Vector2 Pos = Vector2.zero;
        private Vector2 GetCubePosition(int r, int c)
        {
            /// 以左上角为起点
            Pos.x = c * CubeComponent.kCubeWidth;
            Pos.y = -r * CubeComponent.kCubeHeight;

            return Pos;
        }

        private CubeComponent GetCube(int r, int c)
        {
            var idx = this.Panel.Col * r + c;
            return this.cubes[idx];
        }

        private void Update()
        {
            if (!this.animationTime.HasValue)
                return;

            this.animationTime = this.animationTime.Value - Time.deltaTime;
            if (this.animationTime.Value <=0)
            {
                this.animationTime = null;
                this.DelCubes();
                this.Refresh();
            }
        }
    }
}
