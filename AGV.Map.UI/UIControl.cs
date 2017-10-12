using AGV.Map.Common;
using AGV.Map.Core;
using AGV.Map.Core.GLExtension;
using SharpGL;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AGV.Map.UI
{
    public partial class UIControl : OpenGLControl
    {
        private IDragManager mDragManager = Factory.FDrag.DragManager();

        private uint mDragTargetID;

        /// <summary>
        /// 網格指標
        /// </summary>
        private IntPtr mGridPtr;

        /// <summary>
        /// 網格大小
        /// </summary>
        private int mGridSize = 1000;

        /// <summary>
        /// 網格頂點數量
        /// </summary>
        private int mGridVertexCount = -1;

        private bool mISMousePress = false;
        private IRecode<IPair> mMousePosition = Factory.FOthers.Recode<IPair>(Factory.FGeometry.Pair());

        private ToolTip mToolTip = new ToolTip();

        public UIControl()
        {
            InitializeComponent();
            MouseWheel += UIControl_MouseWheel;
            MouseDown += UIControl_MouseDown;
            MouseMove += UIControl_MouseMove;
            MouseUp += UIControl_MouseUp;
            MouseDoubleClick += UIControl_MouseDoubleClick;
            GDIDraw += UIControl_GDIDraw;
        }

        /// <summary>
        /// 繪製坐標軸
        /// </summary>
        private void DrawAxis()
        {
            int axisLength = 1000;
            OpenGL gl = OpenGL;
            gl.LineWidth(5);
            gl.Begin(OpenGL.GL_LINES);
            {
                gl.Color(AxisXColor.GetFloats());
                gl.Vertex(0, 0);
                gl.Vertex(axisLength, 0);
                gl.Color(AxisYColor.GetFloats());
                gl.Vertex(0, 0);
                gl.Vertex(0, axisLength);
            }
            gl.End();
            gl.BeginStipple(ELineStyle._1111111011111110);
            {
                gl.Begin(OpenGL.GL_LINES);
                {
                    gl.Color(AxisXColor.GetFloats());
                    gl.Vertex(0, 0);
                    gl.Vertex(-axisLength, 0);
                    gl.Color(AxisYColor.GetFloats());
                    gl.Vertex(0, 0);
                    gl.Vertex(0, -axisLength);
                }
                gl.End();
            }
            gl.EndStipple();
        }

        private void DrawDataBase()
        {
            OpenGL gl = OpenGL;
            Database.ForbiddenAreaGM.Draw(gl);
            Database.ForbiddenLineGM.Draw(gl);
            Database.AGVGM.Draw(gl);
            Database.GoalGM.Draw(gl);
            Database.LaserPointsGM.Draw(gl);
            Database.NarrowLineGM.Draw(gl);
            Database.ObstacleLinesGM.Draw(gl);
            Database.ObstaclePointsGM.Draw(gl);
            Database.ParkingGM.Draw(gl);
            Database.PowerGM.Draw(gl);
        }

        private void DrawDragManager()
        {
            OpenGL gl = OpenGL;
            mDragManager.Draw(gl);
        }

        /// <summary>
        /// 繪製格線
        /// </summary>
        private void DrawGrid()
        {
            if (Zoom.Value > 40) return;

            OpenGL gl = OpenGL;

            // 顏色、大小
            gl.Color(GridColor.GetFloats());
            gl.LineWidth(1);

            // 座標設置
            gl.PushMatrix();
            {
                gl.LoadIdentity();
                gl.Translate((Translate.X) % mGridSize, (Translate.Y) % mGridSize, (int)EType.Grid);
                if (mGridVertexCount != -1)
                {
                    gl.DrawArrays(OpenGL.GL_LINES, mGridVertexCount, mGridPtr);
                }
                else
                {
                    GenGridVertex();
                }
            }
            gl.PopMatrix();
        }

        private void DrawNames()
        {
            OpenGL gl = OpenGL;
            PushNames(Database.AGVGM);
            PushNames(Database.GoalGM);
            PushNames(Database.PowerGM);
            gl.DrawTextList(GLToText);
        }

        private IDragable FindAreaDragTarget<T>(ISaftyDictionary<T> dictionary, IPair pos, ref uint ID) where T : ISingle<IArea>, IDragable
        {
            uint resID = 0;
            IDragable res = null;
            dictionary.SaftyForLoop((id, value) =>
            {
                if (value.Interference(pos))
                {
                    resID = id;
                    res = value;
                }
            });
            ID = resID;
            return res;
        }

        private IDragable FindDragTarget(IPair pos)
        {
            IDragable res = null;
            res = FindTowardPairDragTarget(Database.AGVGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindTowardPairDragTarget(Database.PowerGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindTowardPairDragTarget(Database.GoalGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindTowardPairDragTarget(Database.ParkingGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindLineDragTarget(Database.ForbiddenLineGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindLineDragTarget(Database.NarrowLineGM, pos, ref mDragTargetID);
            if (res != null) return res;
            res = FindAreaDragTarget(Database.ForbiddenAreaGM, pos, ref mDragTargetID);
            return res;
        }

        private IDragable FindLineDragTarget<T>(ISaftyDictionary<T> dictionary, IPair pos, ref uint ID) where T : ISingle<ILine>, IDragable
        {
            uint resID = 0;
            IDragable res = null;
            dictionary.SaftyForLoop((id, value) =>
            {
                if (value.Interference(pos))
                {
                    resID = id;
                    res = value;
                }
            });
            ID = resID;
            return res;
        }

        private IDragable FindTowardPairDragTarget<T>(ISaftyDictionary<T> dictionary, IPair pos, ref uint ID) where T : ISingle<ITowardPair>, IDragable
        {
            uint resID = 0;
            IDragable res = null;
            dictionary.SaftyForLoop((id, value) =>
            {
                if (value.Interference(pos))
                {
                    resID = id;
                    res = value;
                }
            });
            ID = resID;
            return res;
        }

        /// <summary>
        /// 產生網格
        /// </summary>
        private void GenGridVertex()
        {
            int rowCount = 15;
            int columonCount = 30;
            int[] vertex = new int[(rowCount * 2 + 1 + columonCount * 2 + 1) * 4];
            mGridVertexCount = vertex.Length / 2;
            int index = 0;
            for (int row = -rowCount; row <= rowCount; ++row)
            {
                vertex[index] = -columonCount * mGridSize; // begin.X
                index++;
                vertex[index] = row * mGridSize; // begin.Y
                index++;
                vertex[index] = columonCount * mGridSize; // end.X
                index++;
                vertex[index] = row * mGridSize; // end.Y
                index++;
            }
            for (int column = -columonCount; column <= columonCount; ++column)
            {
                vertex[index] = column * mGridSize; // begin.X
                index++;
                vertex[index] = -rowCount * mGridSize; // begin.Y
                index++;
                vertex[index] = column * mGridSize; // end.X
                index++;
                vertex[index] = rowCount * mGridSize; // end.Y
                index++;
            }
            mGridPtr = Marshal.AllocHGlobal(vertex.Length * sizeof(int));
            Marshal.Copy(vertex, 0, mGridPtr, vertex.Length);
        }

        private IPair GLToText(IPair gl)
        {
            IPair screen = GLToScreen(gl.X, gl.Y);
            return Factory.FGeometry.Pair(screen.X, Height - screen.Y);
        }

        /// <summary>
        /// 初始化畫布
        /// </summary>
        private void InitialDraw()
        {
            OpenGL gl = OpenGL;
            gl.ClearColor(BackgroundColor.R / 255.0f, BackgroundColor.G / 255.0f, BackgroundColor.B / 255.0f, BackgroundColor.A / 255.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            if (ShowNames) gl.ClearText();
            // 投影矩陣
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();
            // 畫布的大小（正交）

            gl.Ortho(-Zoom.Value * Width / 2, Zoom.Value * Width / 2, -Zoom.Value * Height / 2, Zoom.Value * Height / 2, -10, 100);
            // 繪圖矩陣
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();
            //線條去鋸齒
            // gl.Enable(OpenGL.GL_LINE_SMOOTH);

            // 點去鋸齒
            // gl.Enable(OpenGL.GL_POINT_SMOOTH);

            // 多邊形去鋸齒
            // gl.Enable(OpenGL.GL_POLYGON_SMOOTH);

            //// 多邊形去鋸齒
            // gl.Enable(OpenGL.GL_SMOOTH);

            //深度測試
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            //設定混和模式
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.CullFace(OpenGL.GL_CCW);
            // 設定座標原點
            gl.Translate(Translate.X, Translate.Y, 0.0f);
        }

        /// <summary>
        /// 將現實座標從 from 移動到 target
        /// </summary>
        private void MoveMap(IPair from, IPair target)
        {
            Translate.X += (target.X - from.X);
            Translate.Y += (target.Y - from.Y);
        }

        private void PushNames<T>(ISaftyDictionary<T> dictionary) where T : ISingle<ITowardPair>, IDragable
        {
            OpenGL gl = OpenGL;
            dictionary.SaftyForLoop((id, value) =>
            {
                gl.PushText(value.Data.Position, value.Name);
            });
        }

        private void ShowTip(IPair pos)
        {
            string msg = string.Format("({0},{1})", pos.X, pos.Y);
            string orgMsg = mToolTip.GetToolTip(this);
            if (msg != orgMsg)
            {
                mToolTip.SetToolTip(this, msg);
            }
        }

        private void UIControl_GDIDraw(object sender, RenderEventArgs args)
        {
            InitialDraw();
            if (ShowGrid) DrawGrid();
            if (ShowAxis) DrawAxis();
            DrawDataBase();
            DrawDragManager();
            if (ShowNames) DrawNames();
        }

        private void UIControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AllowEdit)
            {
                mDragTargetID = 0;
                mDragManager.DragTaeget = FindDragTarget(ScreenToGL(e.X, e.Y));
            }
        }

        private void UIControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMousePosition.Now = ScreenToGL(e.X, e.Y);
            mMousePosition.Push();
            mISMousePress = true;
            if (AllowEdit && mDragManager.Status == EDragStatus.Ready)
            {
                mDragManager.TakeControl(mMousePosition.Now);
            }
        }

        private void UIControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMousePosition.Now = ScreenToGL(e.X, e.Y);
            ShowTip(mMousePosition.Now);
            if (mISMousePress && AllowEdit && mDragManager.Status == EDragStatus.Dragging)
            {
                mDragManager.Drag(mMousePosition.Now);
                if (mDragManager.DragTaeget == null) mDragTargetID = 0;
                if (mDragManager.DragTaeget is ISingle<ITowardPair>) DragEvent?.Invoke((ISingle<ITowardPair>)mDragManager.DragTaeget, mDragTargetID);
                return;
            }
            if (mISMousePress)
            {
                MoveMap(mMousePosition.Old, mMousePosition.Now);
                mMousePosition.Now = ScreenToGL(e.X, e.Y);
            }
            mMousePosition.Push();
        }

        private void UIControl_MouseUp(object sender, MouseEventArgs e)
        {
            mDragManager.ReleaseControl();
            mISMousePress = false;
        }

        private void UIControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IPair orgMousePoint = ScreenToGL(e.X, e.Y);
            Focus(orgMousePoint);
            if (e.Delta > 0) Zoom.Value *= 1.2;
            if (e.Delta < 0) Zoom.Value /= 1.2;
            MoveMap(orgMousePoint, ScreenToGL(e.X, e.Y));
        }
    }
}