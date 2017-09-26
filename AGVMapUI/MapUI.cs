using SharpGL;
using System;
using System.Windows.Forms;

namespace AGVMap
{
    /// <summary>
    /// 地圖控制元件
    /// </summary>
    public partial class MapUI : OpenGLControl
    {
        /// <summary>
        /// 預設建構子
        /// </summary>
        public MapUI()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(MapDisplayer_MouseWheel);
        }

        /// <summary>
        /// 地圖控制器，所有使用者皆透過此介面對地圖操作
        /// </summary>
        public IMapCtrl Ctrl { get { return mCtrl; } }

        /// <summary>
        /// X 軸顏色
        /// </summary>
        private readonly IColor AxisXColor = new Color(System.Drawing.Color.Red);

        /// <summary>
        /// Y 軸顏色
        /// </summary>
        private readonly IColor AxisYColor = new Color(System.Drawing.Color.Green);

        /// <summary>
        /// 背景顏色
        /// </summary>
        private readonly IColor BackgroundColor = new Color(System.Drawing.Color.Wheat);

        /// <summary>
        /// 選取區顏色
        /// </summary>
        private readonly IColor ChooseAreaColor = new Color(System.Drawing.Color.Aqua, 50);

        /// <summary>
        /// 網格顏色
        /// </summary>
        private readonly IColor GridColor = new Color(System.Drawing.Color.Gray);

        /// <summary>
        /// 地圖控制器
        /// </summary>
        private MapCtrl mCtrl = new MapCtrl();

        /// <summary>
        /// 網格 CallList 編號
        /// </summary>
        private uint mGridList = 0;

        /// <summary>
        /// 點是否按下
        /// </summary>
        private bool mIsMousePress = false;

        /// <summary>
        /// 滑鼠按下時的位置(實際座標)
        /// </summary>
        private Pair mMouseDownPos = new Pair();

        /// <summary>
        /// 提示工具
        /// </summary>
        private ToolTip mToolTip = new ToolTip();

        /// <summary>
        /// 繪製選擇區域
        /// </summary>
        private void DrawChooseArea()
        {
            if (mCtrl.MouseType != EMouseType.SelectedMode || !mIsMousePress) return;
            OpenGL gl = OpenGL;
            {
                gl.PushMatrix();
                {
                    gl.Translate(0, 0, (int)ELayer.Selected);
                    gl.Color(ChooseAreaColor.ToArray);
                    Area area = new Area(mCtrl.MouseCurrentPos, mMouseDownPos);
                    gl.Begin(OpenGL.GL_POLYGON);
                    {
                        gl.Vertex(area.Min.X, area.Min.Y);
                        gl.Vertex(area.Max.X, area.Min.Y);
                        gl.Vertex(area.Max.X, area.Max.Y);
                        gl.Vertex(area.Min.X, area.Max.Y);
                    }
                    gl.End();
                }
                gl.PopMatrix();
            }
        }

        /// <summary>
        /// 繪製格線
        /// </summary>
        private void DrawGrid()
        {
            if (mCtrl.Zoom > 40 || !mCtrl.EnableGrid) return;

            int gridSize = 1000; //mm
            int gridCount = 50;

            int mapSize = gridSize * gridCount;
            OpenGL gl = OpenGL;

            // 顏色、大小
            gl.Color(GridColor.ToArray);
            gl.LineWidth(0.1f);

            // 座標設置
            gl.PushMatrix();
            {
                gl.LoadIdentity();
                gl.Translate((mCtrl.Translate.X) % gridSize, (mCtrl.Translate.Y) % gridSize, (int)ELayer.Grid);
                if (mGridList != 0)
                {
                    gl.CallList(mGridList);
                }
                else
                {
                    mGridList = gl.GenLists(1);
                    gl.NewList(mGridList, OpenGL.GL_COMPILE_AND_EXECUTE);
                    {
                        gl.LineWidth(1.0f);
                        gl.Begin(OpenGL.GL_LINES);
                        {
                            for (int ii = -gridCount; ii <= gridCount; ++ii)
                            {
                                gl.Vertex(ii * gridSize, -mapSize);
                                gl.Vertex(ii * gridSize, +mapSize);
                                gl.Vertex(-mapSize, ii * gridSize);
                                gl.Vertex(+mapSize, ii * gridSize);
                            }
                        }
                        gl.End();
                    }
                    gl.EndList();
                }
            }
            gl.PopMatrix();
        }

        /// <summary>
        /// 繪製六大種類的圖形
        /// </summary>
        private void DrawMap()
        {
            OpenGL gl = OpenGL;
            mCtrl.Draw(gl);
        }

        /// <summary>
        /// 繪製 X、Y 軸
        /// </summary>
        private void DrawXY()
        {
            int length = 1000; //mm
            int triangle = 50; //mm
            OpenGL gl = OpenGL;

            gl.LineWidth(4.0f);
            gl.PushMatrix();
            {
                // X 軸
                gl.Color(AxisXColor.ToArray);
                gl.Begin(OpenGL.GL_LINES);
                {
                    gl.Vertex(0, 0, (int)ELayer.Axis);
                    gl.Vertex(length - triangle, 0, (int)ELayer.Axis);
                }
                gl.End();
                gl.Begin(OpenGL.GL_TRIANGLES);
                {
                    gl.Vertex(length - triangle, -triangle / 2);
                    gl.Vertex(length, 0);
                    gl.Vertex(length - triangle, +triangle / 2);
                }
                gl.End();
                // Y 軸
                gl.Color(AxisYColor.ToArray);
                gl.Begin(OpenGL.GL_LINES);
                {
                    gl.Vertex(0, 0, (int)ELayer.Axis);
                    gl.Vertex(0, length - triangle, (int)ELayer.Axis);
                }
                gl.End();
                gl.Begin(OpenGL.GL_TRIANGLES);
                {
                    gl.Vertex(-triangle / 2, length - triangle);
                    gl.Vertex(0, length);
                    gl.Vertex(+triangle / 2, length - triangle);
                }
                gl.End();
            }
            gl.PopMatrix();
        }

        /// <summary>
        /// 初始化畫布
        /// </summary>
        private void InitialDraw()
        {
            OpenGL gl = OpenGL;
            gl.ClearColor(BackgroundColor.R / 255.0f, BackgroundColor.G / 255.0f, BackgroundColor.B / 255.0f, BackgroundColor.A / 255.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // 投影矩陣
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();
            // 畫布的大小（正交）
            gl.Ortho(-mCtrl.Zoom * Width / 2, mCtrl.Zoom * Width / 2, -mCtrl.Zoom * Height / 2, mCtrl.Zoom * Height / 2, -10, 1000);
            // 繪圖矩陣
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();
            //線條去鋸齒
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            // 點去鋸齒
            gl.Enable(OpenGL.GL_POINT_SMOOTH);

            // 多邊形去鋸齒
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);

            //// 多邊形去鋸齒
            gl.Enable(OpenGL.GL_SMOOTH);

            //深度測試
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            //設定混和模式
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.CullFace(OpenGL.GL_CCW);
            // 設定座標原點
            gl.Translate(mCtrl.Translate.X, mCtrl.Translate.Y, 0.0f);
        }

        /// <summary>
        /// 畫面縮放
        /// </summary>
        private void MapDisplayer_MouseWheel(object sender, MouseEventArgs e)
        {
            IPair orgMousePoint = Mouse2GL(e.X, e.Y);
            mCtrl.Focus(orgMousePoint);
            if (e.Delta > 0) mCtrl.Zoom *= MapCtrl.ZoomStep;
            if (e.Delta < 0) mCtrl.Zoom /= MapCtrl.ZoomStep;
            MoveMap(orgMousePoint, Mouse2GL(e.X, e.Y));
        }

        private void MapUI_DoubleClick(object sender, EventArgs e)
        {
            if (mCtrl.MouseType == EMouseType.EditMode) mCtrl.DragSetCtrlObj(mCtrl.FindControllTarget(mCtrl.MouseCurrentPos));
        }

        /// <summary>
        /// 繪圖
        /// </summary>
        private void MapUI_GDIDraw(object sender, SharpGL.RenderEventArgs args)
        {
            InitialDraw();
            DrawGrid();
            DrawXY();
            DrawChooseArea();
            DrawMap();
        }

        /// <summary>
        /// 畫面平移、選擇
        /// </summary>
        private void MapUI_MouseDown(object sender, MouseEventArgs e)
        {
            mMouseDownPos = new Pair(Mouse2GL(e.X, e.Y));
            mIsMousePress = true;
            mCtrl.DragMouseDown(mMouseDownPos);
        }

        /// <summary>
        /// 畫面平移、選擇
        /// </summary>
        private void MapUI_MouseMove(object sender, MouseEventArgs e)
        {
            //獲得選取目標名稱
            string name = mCtrl.CurrentCtrlObjName();
            //顯示座標提示
            mCtrl.MouseCurrentPos = new Pair(Mouse2GL(e.X, e.Y));
            string msg = string.Format("{0}({1},{2})", name, mCtrl.MouseCurrentPos.X, mCtrl.MouseCurrentPos.Y);
            string orgMsg = mToolTip.GetToolTip(this);
            if (msg != orgMsg)
            {
                mToolTip.SetToolTip(this, msg);
            }
            //計算畫面移動
            if ((mCtrl.MouseType == EMouseType.TranslationMode || (mCtrl.MouseType == EMouseType.EditMode && !mCtrl.DragHasSelectedCtrlPoint())) && mIsMousePress)
            {
                MoveMap(mMouseDownPos, mCtrl.MouseCurrentPos);
                mMouseDownPos = new Pair(Mouse2GL(e.X, e.Y));
            }
            //拖曳
            if (mCtrl.MouseType == EMouseType.EditMode && mIsMousePress)
            {
                mCtrl.DragMouseMoving(Mouse2GL(e.X, e.Y));
            }
        }

        private void MapUI_MouseUp(object sender, MouseEventArgs e)
        {
            //拖曳
            if (mCtrl.MouseType == EMouseType.EditMode && mIsMousePress)
            {
                mCtrl.DragMouseUp();
            }
            mIsMousePress = false;
        }

        /// <summary>
        /// 滑鼠座標轉實際座標
        /// </summary>
        private IPair Mouse2GL(int mouseX, int mouseY)
        {
            double mX = (mouseX) - Width / 2;
            double mY = (Height - mouseY) - Height / 2;
            return new Pair((int)(mX * mCtrl.Zoom - mCtrl.Translate.X), (int)(mY * mCtrl.Zoom - mCtrl.Translate.Y));
        }

        /// <summary>
        /// 滑鼠座標轉實際座標
        /// </summary>
        private IPair Mouse2GL(IPair mouse)
        {
            return Mouse2GL(mouse.X, mouse.Y);
        }

        /// <summary>
        /// 將現實座標從 from 移動到 target
        /// </summary>
        private void MoveMap(IPair from, IPair target)
        {
            mCtrl.Translate.X += (target.X - from.X);
            mCtrl.Translate.Y += (target.Y - from.Y);
        }
    }
}