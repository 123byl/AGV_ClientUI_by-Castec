using SharpGL;
using System;

namespace AGVMap
{
    /// <summary>
    /// 具執行緒安全的可繪滑鼠拖曳控制管理器
    /// </summary>
    public class DDragM : ISafty, IDrawable
    {
        /// <summary>
        /// 可繪滑鼠選拖曳控制管理器建構子
        /// </summary>
        public DDragM()
        {
        }

        /// <summary>
        /// 當下的受控的物件
        /// </summary>
        public ICtrlable CtrlObject { get; private set; } = null;

        /// <summary>
        /// 獲得受控對象的名稱
        /// </summary>
        public string CurrentCtrlObjName {
            get {
                lock (Key)
                {
                    if (CtrlObject != null && CtrlObject is INameable) return ((INameable)CtrlObject).Name;
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 執行緒鎖
        /// </summary>
        public object Key { get; } = new object();

        /// <summary>
        /// 屬性
        /// </summary>
        public DDragMProperty Property { get; } = new DDragMProperty();

        /// <summary>
        /// 清除受控對象
        /// </summary>
        public void ClearCtrlObj()
        {
            lock (Key)
            {
                CtrlObject = null;
                mSelected = -1;
                mCtrlPointSet.Clear();
            }
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        public void Draw(OpenGL gl)
        {
            lock (Key)
            {
                mCtrlPointSet.ForEach((item) =>
                {
                    Draw(gl, item.CtrlPoint, item.Size, Property.LineWidth, Property.Color);
                });
                if (mSelected >= 0 && mSelected < mCtrlPointSet.Count)
                {
                    Draw(gl, mCtrlPointSet.At(mSelected).CtrlPoint, mCtrlPointSet.At(mSelected).Size, Property.SelectdLineWidth, Property.SelectedColor);
                }
            }
        }

        /// <summary>
        /// 是否有被選中的控制點
        /// </summary>
        public bool HasSelectedCtrlPoint()
        {
            lock (Key)
            {
                return mSelected >= 0 && mSelected < mCtrlPointSet.Count;
            }
        }

        /// <summary>
        /// 滑鼠按下時的位置
        /// </summary>
        public void MouseDown(IPair mousePos)
        {
            lock (Key)
            {
                mMousePos = new Pair(mousePos);
                mSelected = FindMousePressTarget(mousePos);
            }
        }

        /// <summary>
        /// 滑鼠拖曳
        /// </summary>
        public void MouseMoving(IPair mousePos)
        {
            lock (Key)
            {
                if (CtrlObject != null && mSelected != -1 && mSelected < mCtrlPointSet.Count)
                {
                    mCtrlPointSet.At(mSelected).CtrlPoint = new Pair(mCtrlPointSet.At(mSelected).CtrlPoint) + new Pair(mousePos) - mMousePos;
                    mCtrlPointSet.ClearAndAddRange(CtrlObject.GetMouseDragPoint());
                    mMousePos = new Pair(mousePos);
                }
            }
        }

        /// <summary>
        /// 滑鼠放開
        /// </summary>
        public void MouseUp()
        {
            lock (Key)
            {
                mSelected = -1;
            }
        }

        /// <summary>
        /// 設定受控物件，並重設控制點集合
        /// </summary>
        public void SetCtrlObj(ICtrlable obj)
        {
            lock (Key)
            {
                CtrlObject = obj;
                if (CtrlObject != null)
                    mCtrlPointSet.ClearAndAddRange(CtrlObject.GetMouseDragPoint());
                else
                    mCtrlPointSet.Clear();
            }
        }

        /// <summary>
        /// 當下所有控制點
        /// </summary>
        private ListSet<DragPoint> mCtrlPointSet = new ListSet<DragPoint>();

        /// <summary>
        /// 滑鼠最後位置
        /// </summary>
        private Pair mMousePos = new Pair();

        /// <summary>
        /// 被選取的控制點編號
        /// </summary>
        private int mSelected = -1;

        /// <summary>
        /// 繪圖
        /// </summary>
        private void Draw(OpenGL gl, IPair center, IPair size, float lineWidth, IColor color)
        {
            gl.PushMatrix();
            {
                gl.LineWidth(lineWidth);
                gl.Translate(center.X, center.Y, 0);
                gl.Color(color.ToArray);
                gl.Begin(OpenGL.GL_LINE_LOOP);
                {
                    gl.Vertex(+size.X / 2, -size.Y / 2, (int)Property.Layer);
                    gl.Vertex(+size.X / 2, +size.Y / 2, (int)Property.Layer);
                    gl.Vertex(-size.X / 2, +size.Y / 2, (int)Property.Layer);
                    gl.Vertex(-size.X / 2, -size.Y / 2, (int)Property.Layer);
                }
                gl.End();
            }
            gl.PopMatrix();
        }

        /// <summary>
        /// 獲得滑鼠點擊的對象編號
        /// </summary>
        private int FindMousePressTarget(IPair mousePos)
        {
            int index = 0;
            int res = -1;
            mCtrlPointSet.ForEach((item) =>
            {
                Pair distance = new Pair(item.CtrlPoint) - new Pair(mousePos);
                if (Math.Abs(distance.X) <= item.Size.X / 2 && Math.Abs(distance.Y) <= item.Size.Y / 2)
                {
                    res = index;
                }
                index++;
            });
            return res;
        }
    }
}