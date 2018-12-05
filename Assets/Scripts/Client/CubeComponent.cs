using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DragCube.Logic;
using DragCube.Client;

namespace DrugCube.Client
{
    public class CubeComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,IEndDragHandler
    {
        public const int kCubeWidth  = 32;
        public const int kCubeHeight = 32;

        [SerializeField]
        private Image Image;
        [SerializeField]
        private SpriteRef Refs;
        public Animator Animator { get; private set; }

        private void Awake()
        {
            this.Animator = this.Image.gameObject.GetComponent<Animator>();
        }

        public int Row
        {
            get
            {
                var temp = (this.transform.localPosition.y + kCubeHeight / 2) / kCubeHeight;
                var r = -Mathf.FloorToInt(temp);
                return r;
            }
        }
        public int Col
        {
            get
            {
                return Mathf.FloorToInt(this.transform.localPosition.x / kCubeWidth + 0.5f);
            }
        }

        private int BoardWidth;
        private int BoardHeight;
        public void Init(int w, int h)
        {
            this.BoardWidth = w;
            this.BoardHeight = h;
        }

        public void SetLocalPostion(Vector2 pos)
        {
            if (pos.x < 0)
                pos.x = 0;

            if (pos.y > 0)
                pos.y = 0;

            this.transform.localPosition = pos;
        }

        private Vector2 ClampPostion(Vector2 pos)
        {
            if (pos.x < -kCubeWidth/2)
            {
                pos.x = this.BoardWidth - kCubeWidth / 2;
            }
            else if (pos.x > this.BoardWidth - kCubeWidth/2)
            {
                pos.x = -kCubeWidth / 2;
            }

            if (pos.y > kCubeHeight/2)
            {
                pos.y = -this.BoardHeight + kCubeHeight / 2;
            }
            else if (pos.y < -this.BoardHeight + kCubeHeight/2)
            {
                pos.y = kCubeHeight / 2;
            }

            return pos;
        }

        public void Move(Vector3 delta)
        {
            var pos = this.transform.localPosition + delta;
            this.transform.localPosition = this.ClampPostion(pos);
        }

        public void Refresh(Cube cube)
        {
            switch (cube.DirectType)
            {
                case (int) Direct.U: this.Image.sprite = this.Refs["U"]; break;
                case (int) Direct.D: this.Image.sprite = this.Refs["D"]; break;
                case (int) Direct.L: this.Image.sprite = this.Refs["L"]; break;
                case (int) Direct.R: this.Image.sprite = this.Refs["R"]; break;
                case (int) Direct.U | (int) Direct.L: this.Image.sprite = this.Refs["UL"]; break;
                case (int) Direct.U | (int) Direct.R: this.Image.sprite = this.Refs["UR"]; break;
                case (int) Direct.D | (int) Direct.L: this.Image.sprite = this.Refs["DL"]; break;
                case (int) Direct.D | (int) Direct.R: this.Image.sprite = this.Refs["DR"]; break;
                default : break;
            }
        }

        #region Event Handler
        public Action<int, int, Vector2> OnDrag;
        public Action<int, int, Vector2> OnDragEnd;
        public Action<int, int> OnClick;
        public Action<Vector2> OnDown;
        public Action OnUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDown.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUp.Invoke();
        }

        public void ClickButton()
        {
            OnClick.Invoke(this.Row, this.Col);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            //Debug.Log(string.Format("Draging! Pos: {0}", eventData.position));
            OnDrag.Invoke(this.Row, this.Col, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log(string.Format("end drag! {0}", eventData.position));
            OnDragEnd.Invoke(this.Row, this.Col, eventData.position);
        }
        #endregion
    }
}
