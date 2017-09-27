using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SharpGL.SceneGraph.Assets;
using SharpGL;

namespace MapGL
{
    public partial class CastecMapUI : UserControl
    {
        #region - Definition of Enum - 
        /// <summary> 定義繪圖的圖層 </summary>
        public enum ItemLayout : int
        {
            /// <summary> 置頂 </summary>
            Top = 8,

            /// <summary> 置底 </summary>
            Bottom = 0,

            /// <summary> 格線圖層 </summary>
            Grid = 1,

            /// <summary> 坐標軸圖層 </summary>
            Axis = 2,

            /// <summary> 點位圖層 </summary>
            Point = 3,

            /// <summary> 線段圖層 </summary>
            Line = 4,

            /// <summary> 充電站圖層 </summary>
            Power = 5,

            /// <summary> 目標點圖層 </summary>
            Goal = 6,

            /// <summary> 車子圖層（車子的圖層必須高於目標點圖層、充電站圖層，這樣才有踩點的效果） </summary>
            Car = 7,
        }

        /// <summary> 繪圖指定形狀 </summary>
        public enum Shape
        {
            Ellipse,
            Rectangle,
            FillEllipse,
            FillRectangle,
            Cursor
        }
        #endregion - Definition of Enum - 

        #region - Data Type Class - 
        /// <summary> 物件資訊型態 </summary>
        public class Pos
        {
            private double mTheta;

            /// <summary> List 索引不可填 0 </summary>
            public uint id { get; set; }

            /// <summary> x 座標 </summary>
            public double x { get; set; }

            /// <summary> y 座標 </summary>
            public double y { get; set; }

            /// <summary> 物件名稱 </summary>
            public string name { get; set; }

            /// <summary> 物件定位的朝向角度 </summary>
            public double theta
            {
                get
                {
                    return mTheta;
                }
                set
                {
                    double thetaTmp = value % 360;
                    if (thetaTmp < 0)
                        mTheta = thetaTmp + 360;
                    else
                        mTheta = thetaTmp;
                }
            }

            public Pos() : this(0, 0, 0, 0) { }

            public Pos(double posX, double posY) : this(0, posX, posY, 0) { }

            public Pos(double posX, double posY, double posTheta) : this(0, posX, posY, posTheta) { }

            public Pos(uint posID, double posX, double posY, double posTheta)
            {
                id = posID;
                x = posX;
                y = posY;
                theta = posTheta;
            }
        }

        /// <summary> 線段型態 </summary>
        public class MapLIne
        {
            /// <summary> 直線的起始點位 </summary>
            public int x1 { get; set; }
            public int y1 { get; set; }

            /// <summary> 直線的末端點位 </summary>
            public int x2 { get; set; }
            public int y2 { get; set; }

            public MapLIne() : this(0, 0, 0, 0) { }

            public MapLIne(int xStart, int yStart, int xEnd, int yEnd)
            {
                x1 = xStart;
                y1 = yStart;
                x2 = xEnd;
                y2 = yEnd;
            }
        }

        /// <summary> 點位群組化設定 </summary>
        public class PointGroup
        {
            private string mName = "";
            private List<Point> mDataList = new List<Point>();
            private Color mColor = Color.Black;
            private float mPointsSize = 1;
            private int mLayer = (int)ItemLayout.Point;
            private bool mEnableSampling = false;
            private object lockData = new object();

            public PointGroup() { }

            /// <summary> 集合的名稱 </summary>
            public string Name
            {
                set
                {
                    mName = value;
                }

                get
                {
                    return mName;
                }
            }

            /// <summary> 點位集合 </summary>
            public List<Point> DataList
            {
                set
                {
                    lock (lockData)
                        if (value != null) mDataList.AddRange(value);
                }

                get
                {
                    lock (lockData)
                        return mDataList;
                }
            }

            /// <summary> 此集合的所有點位顏色</summary>
            public Color Color
            {
                set
                {
                    mColor = value;
                }

                get
                {
                    return mColor;
                }
            }

            /// <summary> 設定所有點位的顯示大小 </summary>
            public float PointsSize
            {
                set { mPointsSize = value; }
                get { return mPointsSize; }
            }

            /// <summary> 設定此集合的圖層 </summary>
            public int Layer
            {
                set
                {
                    mLayer = value;
                }

                get
                {
                    return mLayer;
                }
            }

            /// <summary> 是否致能取樣 </summary>
            public bool EnableSampling
            {
                set
                {
                    mEnableSampling = value;
                }

                get
                {
                    return mEnableSampling;
                }
            }
        }

        /// <summary> 線段群組化設定 </summary>
        public class LineGroup
        {
            private string mName = "";
            private List<MapLIne> mDataList = new List<MapLIne>();
            private Color mColor = Color.Black;
            private float mBorderSize = 1;
            private int mLayer = (int)ItemLayout.Point;
            private bool mEnableSampling = false;
            private object lockData = new object();

            public LineGroup() { }

            /// <summary> 集合的名稱 </summary>
            public string Name
            {
                set
                {
                    mName = value;
                }

                get
                {
                    return mName;
                }
            }

            /// <summary> 線段集合 </summary>
            public List<MapLIne> DataList
            {
                set
                {
                    lock (lockData)
                        if (value != null) mDataList.AddRange(value);
                }

                get
                {
                    lock (lockData)
                        return mDataList;
                }
            }

            /// <summary> 此集合的所有線段顏色</summary>
            public Color Color
            {
                set
                {
                    mColor = value;
                }

                get
                {
                    return mColor;
                }
            }

            public float BorderSize
            {
                set
                {
                    mBorderSize = value;
                }

                get
                {
                    return mBorderSize;
                }
            }

            /// <summary> 設定此集合的圖層 </summary>
            public int Layer
            {
                set
                {
                    mLayer = value;
                }

                get
                {
                    return mLayer;
                }
            }

            /// <summary> 是否致能取樣 </summary>
            public bool EnableSampling
            {
                set
                {
                    mEnableSampling = value;
                }

                get
                {
                    return mEnableSampling;
                }
            }
        }

        /// <summary> 中空多邊形的資料型態 </summary>
        private class Polygon
        {
            private uint mId = 1;
            private string mName = "";
            private List<Point> mPointList = new List<Point>();
            private Color mColor = Color.Black;
            private float mBorderSize = 1;
            private int mLayer = (int)ItemLayout.Point;

            public Polygon() { }

            /// <summary> 索引值需 > 0，否則會出錯 </summary>
            public uint Id
            {
                set
                {
                    if (value == 0)
                        throw new Exception("Id != 0, Please set correct number (Id > 0)");
                    else
                        mId = value;
                }

                get
                {
                    return mId;
                }
            }

            /// <summary> 物件的名稱 </summary>
            public string Name
            {
                set
                {
                    mName = value;
                }

                get
                {
                    return mName;
                }
            }

            /// <summary> 每個角的頂點座標 </summary>
            public List<Point> PointList
            {
                set
                {
                    mPointList = value;
                }

                get
                {
                    return mPointList;
                }
            }

            /// <summary> 物件線的顏色 </summary>
            public Color Color
            {
                set
                {
                    mColor = value;
                }

                get
                {
                    return mColor;
                }
            }

            /// <summary> 物件線的粗度 </summary>
            public float BorderSize
            {
                set
                {
                    mBorderSize = value;
                }

                get
                {
                    return mBorderSize;
                }
            }

            /// <summary> 設定此物件的圖層 </summary>
            public int Layer
            {
                set
                {
                    mLayer = value;
                }

                get
                {
                    return mLayer;
                }
            }
        }

        /// <summary> 實心多邊形的資料型態 </summary>
        private class PolygonFillFull
        {
            private uint mId = 1;
            private string mName = "";
            private Shape mItem;
            private List<Pos> mPos = new List<Pos>();
            private List<Size> mSize = new List<Size>();
            private Color mColor = Color.Black;
            private int mLayer = (int)ItemLayout.Point;
            private object lockPos = new object();
            private object lockSize = new object();
            public PolygonFillFull() { }

            /// <summary> 索引值需 > 0，否則會出錯 </summary>
            public uint Id
            {
                set
                {
                    if (value == 0)
                        throw new Exception("Id != 0, Please set correct number (Id > 0)");
                    else
                        mId = value;
                }

                get
                {
                    return mId;
                }
            }

            /// <summary> 物件的名稱 </summary>
            public string Name
            {
                set
                {
                    mName = value;
                }

                get
                {
                    return mName;
                }
            }

            public Shape shape
            {
                set
                {
                    mItem = value;
                }

                get
                {
                    return mItem;
                }
            }

            /// <summary> 物體中心 </summary>
            public List<Pos> Center
            {
                set
                {
                    lock (lockPos)
                        if (value != null) mPos.AddRange(value);
                }

                get
                {
                    lock (lockPos)
                        return mPos;
                }
            }

            /// <summary> 物體大小 </summary>
            public List<Size> Frame
            {
                set
                {
                    lock (lockSize)
                        if (value != null) mSize.AddRange(value);
                }

                get
                {
                    lock (lockSize)
                        return mSize;
                }
            }

            /// <summary> 物件線的顏色 </summary>
            public Color Color
            {
                set
                {
                    mColor = value;
                }

                get
                {
                    return mColor;
                }
            }

            /// <summary> 設定此物件的圖層 </summary>
            public int Layer
            {
                set
                {
                    mLayer = value;
                }

                get
                {
                    return mLayer;
                }
            }
        }
        #endregion - Data Type Class - 

        #region - [Member] Sets - 
        private List<PointGroup> mPointsSet = new List<PointGroup>();
        private List<LineGroup> mLinesSet = new List<LineGroup>();

        private List<Polygon> mPolygonSet = new List<Polygon>();
        private List<PolygonFillFull> mPolygonFillFullSet = new List<PolygonFillFull>();
        #endregion - [Member] Sets - 

        #region - [Member] Set Point - 

        private List<Pos> mListPower = new List<Pos>();
        private Point mTranslate;
        private Point mMousePoint;
        private Point mMouseClickPoint;
        private Point mMouseDownPoint;
        private Point mMouseUpPoint;

        // 紀錄滑鼠點集瞬間的座標
        private Point mMouseClickPos = new Point();

        #endregion - [Member] Set Point - 

        #region - [Member] Event Bool - 
        private bool mIsMoseDown = false;
        private bool mIsMouseClick = false;
        private bool mDisableMouseClick = false;

        //之後有需要使用到 CreateList 的話再把註解取消掉
        //private bool mEnSavePoints = true;
        //private bool mEnSaveGoals = true;
        //private bool mEnSaveCars = false;
        //private bool mEnSaveLines = true;
        //private bool mEnSavePowers = false;
        #endregion - [Member] Event Bool - 

        #region - [Member] Others - 
        private object mLkPointsSet = new object();
        private object mLkLinesSet = new object();

        /// <summary> 暫存選取的物件編號 </summary>
        private uint[] selectBuf = new uint[4];

        /// <summary> 儲存照片編號 </summary>
		private uint mGLList = 0;
        #endregion - [Member] Others - 

        #region - [Properties] Parameters of Display - 

        private Font mFontText = new Font("Arial", 12);
        [Description("文字的字體"), Category("")]
        public Font FontText
        {
            set { mFontText = value; }
            get { return mFontText; }
        }

        private int mResolution = 1;
        [Description("解析度"), Category("")]
        public int Resolution
        {
            set
            {
                if (value > 1)
                    mResolution = value;
                else
                    mResolution = 1;
            }

            get
            {
                return mResolution;
            }
        }

        private uint mCountCar = 1;
        [Description("車子數量，之後多台車會需要設定這個項目")/*, Category("")*/]
        /// <summary>
        /// 之後多台車會需要設定這個項目
        /// </summary>
        public uint CountTotalCar
        {
            set
            {
                if (value > 1)
                    mCountCar = value;
                else
                    mCountCar = 0;
            }

            get
            {
                return mCountCar;
            }
        }

        private uint mCountPower = 1;
        [Description("充電站數量")/*, Category("")*/]
        public uint CountTotalPower
        {
            set
            {
                mCountPower = value;
            }

            get
            {
                return mCountPower;
            }
        }

        private int mSizeGrid = 100;
        [Description("格線的大小"), Category("Overlooking of Items")]
        public int SizeGrid
        {
            set
            {
                mSizeGrid = value;
            }

            get
            {
                return mSizeGrid;
            }
        }

        private double mZoom = 1.5;
        [Description("畫面縮放比例")]
        public double Zoom
        {
            get
            {
                return mZoom;
            }

            set
            {
                mZoom = value;

                if (value > 300)
                    mZoom = 300;
                else if (value < 1)
                    mZoom = 1;
                else
                    mZoom = value;

                //Console.WriteLine("[Zoom] " + mZoom);
            }
        }

        private Size mIconSizeCar = new Size(100, 100);
        [Description("車子的大小"), Category("Overlooking of Items")]
        public Size SizeCar
        {
            set
            {
                mIconSizeCar = value;
            }

            get
            {
                return mIconSizeCar;
            }
        }

        private Size mIconSizeGoal = new Size(500, 500);
        [Description("Goal 的大小"), Category("Overlooking of Items")]
        public Size SizeGoal
        {
            set
            {
                mIconSizeGoal = value;
            }

            get
            {
                return mIconSizeGoal;
            }
        }
        #endregion - [Properties] Parameters of Display - 

        #region - [Properties] Enum - 

        /// <summary> 指定車子形狀 </summary>
		private Shape mCarShape = Shape.Ellipse;
        [Description("車子的形狀"), Category("Overlooking of Items")]
        public Shape CarShape
        {
            set
            {
                mCarShape = value;
            }

            get
            {
                return mCarShape;
            }
        }

        /// <summary> 指定目標形狀 </summary>
		private Shape mGoalShape = Shape.Ellipse;
        [Description("目標的形狀"), Category("Overlooking of Items")]
        public Shape GoalShape
        {
            set
            {
                mGoalShape = value;
            }

            get
            {
                return mGoalShape;
            }
        }

        #endregion - [Properties] Enum - 

        #region - [Properties] Point & Pos - 

        private List<Pos> mListGoal = new List<Pos>();
        protected internal List<Pos> ListGoal
        {
            set
            {
                mListGoal = value;
            }

            get
            {
                return mListGoal;
            }
        }

        private List<Pos> mListCar = new List<Pos>();
        protected internal List<Pos> ListCar
        {
            set
            {
                mListCar = value;
            }

            get
            {
                return mListCar;
            }
        }

        protected internal List<Pos> ListPower
        {
            set
            {
                mListPower = value;
            }

            get
            {
                return mListPower;
            }
        }

        private Pos mPosCar = new Pos(0, 0, 0);
        public Pos PosCar
        {
            set
            {
                mPosCar = null;
                mPosCar = value;
            }

            get
            {
                return mPosCar;
            }
        }

        protected internal Point GLMousePoint
        {
            get
            {
                return mMousePoint;
            }
        }

        public Point mMinPos = new Point(0, 0);
        [Description("輸入座標最小值")]
        public Point MinPos
        {
            set
            {
                mMinPos = value;
            }

            get
            {
                //if (mMinPos == new Point(0, 0))
                //{
                //    mMinPos = new Point(-Width / 2, -Height / 2);
                //}

                return mMinPos;
            }
        }

        public Point mMaxPos = new Point(0, 0);
        [Description("輸入座標最大值")]
        public Point MaxPos
        {
            set
            {
                mMaxPos = value;
            }

            get
            {
                //if (mMaxPos == new Point(0, 0))
                //{
                //    mMaxPos = new Point(Width / 2, Height / 2);
                //}

                return mMaxPos;
            }
        }
        #endregion - [Properties] Point & Pos - 

        #region - [Properties] Enable Display Items - 
        //private bool mEnableReDraw = true;
        //[Description("是否暫存地圖GL"), Category("Enable Display Item")]
        //public bool EnableReDraw
        //{
        //    get
        //    {
        //        return mEnableReDraw;
        //    }

        //    set
        //    {
        //        mEnableReDraw = value;
        //    }
        //}

        private bool mEnableCar = false;
        [Description("是否顯示車子"), Category("Enable Display Item")]
        public bool EnableCar
        {
            get
            {
                return mEnableCar;
            }

            set
            {
                mEnableCar = value;
            }
        }

        private bool mEnableGoal = false;
        [Description("是否顯示目標點"), Category("Enable Display Item")]
        public bool EnableGoal
        {
            get
            {
                return mEnableGoal;
            }

            set
            {
                mEnableGoal = value;
            }
        }

        private bool mEnablePower = false;
        [Description("是否顯示充電站"), Category("Enable Display Item")]
        public bool EnablePower
        {
            get
            {
                return mEnablePower;
            }

            set
            {
                mEnablePower = value;
            }
        }

        private bool mEnableAxis = false;
        [Description("是否顯示座標軸"), Category("Enable Display Item")]
        public bool EnableAxis
        {
            get
            {
                return mEnableAxis;
            }

            set
            {
                mEnableAxis = value;
            }
        }

        private bool mEnableMouseLocatation = false;
        [Description("是否顯示游標座標"), Category("Enable Display Item")]
        public bool EnableMouseLocatation
        {
            get
            {
                return mEnableMouseLocatation;
            }

            set
            {
                mEnableMouseLocatation = value;
            }
        }

        private bool mEnableGrid = false;
        [Description("是否顯示格線"), Category("Enable Display Item")]
        public bool EnableGrid
        {
            set
            {
                mEnableGrid = value;
            }

            get
            {
                return mEnableGrid;
            }
        }
        #endregion - [Properties] Enable Display Items - 

        #region - [Properties] Color of Items - 
        /// <summary> 畫布背景顏色 </summary>
        private Color mColorBackground = Color.Black;
        /// <summary> 滑鼠游標座標的字體顏色（此項設為對比色不得開放修改） </summary>
        private Color mColorMousePos = Color.White;
        [Description("畫布背景顏色"), Category("Color of Items")]
        public Color ColorBackground
        {
            set
            {
                mColorBackground = value;
                mColorMousePos = Color.FromArgb((255 - mColorBackground.A) / 2, (255 - mColorBackground.R) / 2, (255 - mColorBackground.G) / 2, (255 - mColorBackground.B) / 2);
            }

            get
            {
                return mColorBackground;
            }
        }

        /// <summary> X 坐標軸顏色 </summary>
        private Color mColorAxisX = Color.Red;
        [Description("X 坐標軸顏色"), Category("Color of Items")]
        public Color ColorAxisX
        {
            set
            {
                mColorAxisX = value;
            }

            get
            {
                return mColorAxisX;
            }
        }

        /// <summary> Y 坐標軸顏色 </summary>
        private Color mColorAxisY = Color.Green;
        [Description("Y 坐標軸顏色"), Category("Color of Items")]
        public Color ColorAxisY
        {
            set
            {
                mColorAxisY = value;
            }

            get
            {
                return mColorAxisY;
            }
        }

        /// <summary> 網格線顏色 </summary>
        private Color mColorGrid = Color.Gray;
        [Description("網格線顏色"), Category("Color of Items")]
        public Color ColorGrid
        {
            set
            {
                mColorGrid = value;
            }

            get
            {
                return mColorGrid;
            }
        }

        /// <summary> 障礙物顏色 </summary>
        private Color mColorObstacle = Color.Maroon;
        [Description("障礙物顏色"), Category("Color of Items")]
        public Color ColorObstacle
        {
            set
            {
                mColorObstacle = value;
            }

            get
            {
                return mColorObstacle;
            }
        }

        /// <summary> 標示的字體顏色 </summary>
        private Color mColorTextPoint = Color.Yellow;
        [Description("標示的字體顏色"), Category("Color of Items")]
        public Color ColorTextPoint
        {
            set
            {
                mColorTextPoint = value;
            }

            get
            {
                return mColorTextPoint;
            }
        }

        /// <summary> 車子符號顏色 </summary>
        private Color mColorCarIcon = Color.Blue;
        [Description("車子符號顏色"), Category("Color of Items")]
        public Color ColorCarIcon
        {
            set
            {
                mColorCarIcon = value;
            }

            get
            {
                return mColorCarIcon;
            }
        }

        /// <summary> 目標符號顏色 </summary>
        private Color mColorGoalIcon = Color.Red;
        [Description("目標符號顏色"), Category("Color of Items")]
        public Color ColorGoalIcon
        {
            set
            {
                mColorGoalIcon = value;
            }

            get
            {
                return mColorGoalIcon;
            }
        }

        /// <summary> 充電站符號顏色 </summary>
        private Color mColorPowerIcon = Color.Orange;
        [Description("充電站符號顏色"), Category("Color of Items")]
        public Color ColorPowerIcon
        {
            set
            {
                mColorPowerIcon = value;
            }

            get
            {
                return mColorPowerIcon;
            }
        }

        #endregion - [Properties] Color of Items - 

        #region - [Properties] Envents - 
        // 建立事件，通知外部使用者
        public delegate void DelMouseSelectObj(string name, double x, double y);
        private event DelMouseSelectObj mMouseSelectObj;
        [Description("當 CastecMapUI 上的物件被滑鼠點選時會觸發的事件"), Category("")]
        public event DelMouseSelectObj MouseSelectObj
        {
            add { mMouseSelectObj += value; }
            remove { mMouseSelectObj -= value; }
        }

        public delegate void DelMouseClickRealPos(double posX, double posY);
        private event DelMouseClickRealPos mMouseClickRealPos;
        [Description("當 CastecMapUI 被滑鼠點選時會觸發的事件"), Category("")]
        public event DelMouseClickRealPos MouseClickRealPos
        {
            add { mMouseClickRealPos += value; }
            remove { mMouseClickRealPos -= value; }
        }

        /// <summary>
        /// 選取區域的起始點與終點
        /// </summary>
        /// <param name="beginX"> 起始點的 X </param>
        /// <param name="beginY"> 起始點的 Y </param>
        /// <param name="endX"> 終點的 X </param>
        /// <param name="endY"> 終點的 Y </param>
        public delegate void DelMouseSelectRange(int beginX, int beginY, int endX, int endY);
        private event DelMouseSelectRange mMouseSelectRange;
        public event DelMouseSelectRange MouseSelectRange
        {
            add { mMouseSelectRange += value; }
            remove { mMouseSelectRange -= value; }
        }

        #endregion - [Properties] Envents - 

        /// <summary> CastecMapUI 建構子 </summary>
        public CastecMapUI()
        {
            InitializeComponent();
            MapDisplayer.MouseWheel += new MouseEventHandler(MapDisplayer_MouseWheel);
        }

        #region - Draw Text - 
        /// <summary>
        /// 繪出 2D 文字
        /// </summary>
        /// <param name="gl"> 傳入 OpenGL 物件</param>
        /// <param name="str"> 要顯示的字串內容 </param>
        /// <param name="x"> 在 x 位置顯示 </param>
        /// <param name="y"> 在 y 位置顯示 </param>
        /// <param name="colorText"> 文字顏色 </param>
        /// <param name="fontSize"> 文字大小 </param>
        private void DrawText2D(OpenGL gl, string str, double x, double y, Color colorText, float fontSize)
        {
            gl.PushMatrix();


            gl.DrawText((int)x, (int)y, colorText.R / 255, colorText.G / 255, colorText.B / 255, "Arial", fontSize, str);

            gl.PopMatrix();
        }

        /// <summary>
        /// 繪出具有 z 圖層維度的文字
        /// </summary>
        /// <param name="gl"> 傳入 OpenGL 物件</param>
        /// <param name="str"> 要顯示的字串內容 </param>
        /// <param name="x"> 在 x 位置顯示 </param>
        /// <param name="y"> 在 y 位置顯示 </param>
        /// <param name="colorText"> 文字顏色 </param>
        /// <param name="fontSize"> 文字大小 </param>
        /// <param name="layer">文字預設的圖層為置頂</param>
        private void DrawText(OpenGL gl, string str, double x, double y, Color colorText, float fontSize, int layer = (int)ItemLayout.Top)
        {
            gl.Color(colorText.R / 255.0, colorText.G / 255.0, colorText.B / 255.0, colorText.A / 255.0);
            gl.PushMatrix();
            gl.Translate(x + 100, y + 100, -layer);
            gl.Scale(20 * Zoom, 20 * Zoom, 20 * Zoom);
            gl.DrawText3D(mFontText.SystemFontName, fontSize, 0, 10, str);
            gl.PopMatrix();
        }
        #endregion - Draw Text - 

        #region - Draw Points - 
        /// <summary>
        /// 繪畫單點
        /// </summary>
        /// <param name="x">點位 x</param>
        /// <param name="y">點位 y</param>
        /// <param name="color">點的顏色</param>
        /// <param name="pointSize">點的大小</param>
        /// <param name="layer">點的顯示圖層</param>
        /// <param name="lineName">點的識別名稱</param>
        public void DrawPoint(int x, int y, Color color, float pointSize, int layer = (int)ItemLayout.Point, string lineName = "")
        {
            int i = 0;
            bool getGroup = false; ;
            int numGroup = 0;
            lock (mLkPointsSet)
            {

                for (i = 0; i < mPointsSet.Count; i++)
                {
                    if (lineName == mPointsSet[i].Name)
                    {
                        getGroup = true;
                        numGroup = i;
                        break;
                    }
                }
                if (getGroup)
                {
                    mPointsSet[numGroup].DataList.Add(new Point(x, y));
                }
                else
                {
                    PointGroup pt = new PointGroup();
                    pt.Name = lineName;
                    pt.Color = color;
                    pt.PointsSize = pointSize;
                    pt.Layer = layer;
                    pt.DataList.Add(new Point(x, y));
                    mPointsSet.Add(pt);

                    pt = null;
                }

            }
        }

        /// <summary>
        /// 一次繪畫群點
        /// </summary>
        /// <param name="points">群點點位</param>
        /// <param name="color">群點顏色</param>
        /// <param name="nameGroup">群點的識別名稱</param>
        /// <param name="borderSize">群點的大小</param>
        /// <param name="layer">群點顯示圖層</param>
        public void DrawPoints(List<Point> points, Color color, string nameGroup, float pointSize, bool enableSampling = false, int layer = (int)ItemLayout.Line)
        {
            lock (mLkPointsSet)
            {
                int i = 0;
                bool getGroup = false; ;
                int numGroup = 0;
                lock (mLkPointsSet)
                {
                    for (i = 0; i < mPointsSet.Count; i++)
                    {
                        if (nameGroup == mPointsSet[i].Name)
                        {
                            getGroup = true;
                            numGroup = i;
                            break;
                        }
                    }
                    if (getGroup)
                    {
                        mPointsSet[numGroup].DataList.AddRange(points);
                    }
                    else
                    {
                        PointGroup pointSet = new PointGroup();
                        pointSet.Name = nameGroup;
                        pointSet.Color = color;
                        pointSet.PointsSize = pointSize;
                        pointSet.Layer = layer;
                        pointSet.DataList = points;
                        pointSet.EnableSampling = enableSampling;
                        mPointsSet.Add(pointSet);

                        pointSet = null;
                    }
                }
            }
        }

        /// <summary>
        /// 指定移除識別名稱的單點或群點
        /// </summary>
        /// <param name="nameGroup">識別名稱</param>
        public void RemoveGroupPoint(string nameGroup)
        {
            List<int> index = new List<int>();

            lock (mLkPointsSet)
            {
                for (int i = 0; i < mPointsSet.Count; ++i)
                {
                    if (mPointsSet[i].Name == nameGroup)
                    {
                        index.Add(i);
                        mPointsSet[i].DataList = null;
                        mPointsSet.RemoveAt(i);

                        if (mPointsSet.Count > 1)
                            i--;
                    }
                }
            }

            if (index.Count > 1)
            {
                for (int i = 0; i < index.Count; i++)
                {
                    mPointsSet[index[i]].DataList = null;
                    mPointsSet.RemoveAt(index[i]);
                }
            }
        }

        /// <summary>
        /// 指定移除識別名稱的單點或群點
        /// </summary>
        /// <param name="nameGroup">識別名稱</param>
        public void RemoveGroupPoint(string nameGroup1, string nameGroup2)
        {
            lock (mLkPointsSet)
            {
                for (int i = 0; i < mPointsSet.Count; ++i)
                {
                    if (mPointsSet[i].Name == nameGroup1 || mPointsSet[i].Name == nameGroup2)
                    {
                        mPointsSet[i].DataList = null;
                        mPointsSet.RemoveAt(i);

                        if (mPointsSet.Count > 1)
                            i--;
                    }
                }
            }
        }

        /// <summary>
        /// 移除畫面上所有點位
        /// </summary>
        public void RemoveAllPoints()
        {
            lock (mLkPointsSet)
            {
                for (int i = 0; i < mPointsSet.Count; ++i)
                {
                    mPointsSet[i].DataList = null;
                    mPointsSet[i] = null;
                }
                mPointsSet.Clear();
            }
        }

        private void DrawPoints(OpenGL gl, List<Point> point, Color pointColor, int layer = (int)ItemLayout.Point, float pointSize = 1, bool enableSampling = false)
        {
            int dist = (enableSampling && ((int)(Zoom / 3.5) > 1)) ? (int)(Zoom / 3.5) : 1;

            if (point.Count > 0)
            {
                gl.Color(pointColor.R / 255.0, pointColor.G / 255.0, pointColor.B / 255.0, pointColor.A / 255.0);
                gl.PointSize(pointSize);

                gl.Begin(OpenGL.GL_POINTS);

                //center = new Point((MinPos.X + MaxPos.X) / 2, (MinPos.Y + MaxPos.Y) / 2);

                for (int i = 0; i < point.Count; i += dist)
                {
                    gl.Vertex(point[i].X, point[i].Y, -layer);
                }

                gl.End();

            }
        }

        private void DrawPoints(OpenGL gl, List<Point> point, Color pointColor, ItemLayout layer = ItemLayout.Point, float pointSize = 1)
        {
            lock (mLkPointsSet)
            {
                DrawPoints(gl, point, pointColor, (int)layer, pointSize); 
            }
        }

        private void DrawPoints(OpenGL gl, List<PointGroup> pointSet)
        {
            lock (mLkPointsSet)
            {
                if (pointSet != null && pointSet.Count > 0)
                    for (int i = 0; i < pointSet.Count; i++)
                    {
                        DrawPoints(gl, pointSet[i].DataList, pointSet[i].Color, pointSet[i].Layer, pointSet[i].PointsSize, pointSet[i].EnableSampling);
                    } 
            }
        }
        #endregion - Draw Points - 

        #region - Draw Line - 
        /// <summary>
        /// 繪畫線段
        /// </summary>
        /// <param name="xStart">起點的 x </param>
        /// <param name="yStart">起點的 y </param>
        /// <param name="xEnd">終點的 x </param>
        /// <param name="yEnd">終點的 y </param>
        /// <param name="color">線段顏色</param>
        /// <param name="borderSize">線段粗度</param>
        /// <param name="layer">自訂顯示圖層</param>
        /// <param name="lineName">自訂線段的識別名稱</param>
        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd, Color color, float borderSize = 1, int layer = (int)ItemLayout.Line, string lineName = "")
        {
            int i = 0;
            bool bGroupExist=false;
            lock (mLkLinesSet)
            {
                for(i = 0; i < mLinesSet.Count; i++)
                {
                    if (lineName == mLinesSet[i].Name)
                    {
                        bGroupExist = true;
                        break;
                    }
                }
                if (bGroupExist) mLinesSet[i].DataList.Add(new MapLIne(xStart, yStart, xEnd, yEnd));
                else
                {
                    LineGroup line = new LineGroup();
                    line.Name = lineName;
                    line.Color = color;
                    line.BorderSize = borderSize;
                    line.Layer = layer;
                    line.DataList.Add(new MapLIne(xStart, yStart, xEnd, yEnd));                    
                    mLinesSet.Add(line);
                    line = null;
                }
            }
        }

        /// <summary>
        /// 繪畫群組線段
        /// </summary>
        /// <param name="lines">所有線段</param>
        /// <param name="color">所有線段的顏色</param>
        /// <param name="nameGroup">自訂群組線段的識別名稱</param>
        /// <param name="enableSampling">是否由元件自行取樣</param>
        /// <param name="borderSize">所有線段粗度</param>
        /// <param name="layer">自訂群組線段的顯示圖層</param>
        public void DrawLines(List<MapLIne> lines, Color color, string nameGroup, bool enableSampling = false, float borderSize = 1.0f, int layer = (int)ItemLayout.Line)
        {
            int i = 0;
            bool bGroupExist = false;
            lock (mLkLinesSet)
            {
                for (i = 0; i < mLinesSet.Count; i++)
                {
                    if (nameGroup == mLinesSet[i].Name)
                    {
                        bGroupExist = true;
                        break;
                    }
                }
                if (bGroupExist) mLinesSet[i].DataList.AddRange(lines);
                else
                {
                    LineGroup lineSet = new LineGroup();
                    lineSet.Name = nameGroup;
                    lineSet.Color = color;
                    lineSet.BorderSize = borderSize;
                    lineSet.Layer = layer;
                    lineSet.DataList = lines;
                    lineSet.EnableSampling = enableSampling;
                    mLinesSet.Add(lineSet);
                    lineSet = null;
                }
            }
        }

        /// <summary>
        /// 指定移除識別名稱的線段或群組線段
        /// </summary>
        /// <param name="nameGroup">識別名稱</param>
        public void RemoveGroupLine(string nameGroup)
        {
            List<int> index = new List<int>();

            lock (mLkLinesSet)
            {
                for (int i = 0; i < mLinesSet.Count; i++)
                {
                    if (string.Equals(mLinesSet[i].Name, nameGroup))//(mLinesSet[i].Name == nameGroup)
                    {
                        index.Add(i);
                    }
                }
            }

            if (index.Count > 0)
            {
                if (mLinesSet[index[0]] != null)
                {
                    mLinesSet[index[0]].DataList = null;
                    mLinesSet.RemoveAt(index[0]);
                }

                for (int i = 1; i < index.Count; i++)
                {
                    if (mLinesSet[index[i] - 1] != null)
                    {
                        mLinesSet[index[i] - 1].DataList = null;
                        mLinesSet.RemoveAt(index[i] - 1);
                    }
                }
            }
        }

        public void RemoveGroupLine(string nameGroup1, string nameGroup2)
        {
            lock (mLkLinesSet)
            {
                for (int i = 0; i < mLinesSet.Count; ++i)
                {
                    if (mLinesSet[i].Name == nameGroup1 || mLinesSet[i].Name == nameGroup2)
                    {
                        mLinesSet[i].DataList = null;
                        mLinesSet.RemoveAt(i);

                        if (mLinesSet.Count > 1)
                            i--;
                    }

                }
            }
        }

        /// <summary>
        /// 移除畫面上所有線段（不包含網格與坐標軸線）
        /// </summary>
        public void RemoveAllLines()
        {
            lock (mLkLinesSet)
            {
                for (int i = 0; i < mLinesSet.Count; ++i)
                {
                    mLinesSet[i].DataList = null;
                    mLinesSet[i] = null;
                }
                mLinesSet.Clear();
            }
        }

        /// <summary>
        /// 一次繪製多條直線
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="lines">線條集合（List<Line>）</param>
        /// <param name="lineColor">線條顏色</param>
        /// <param name="borderSize">線條粗度</param>
        private void DrawLines(OpenGL gl, List<MapLIne> lines, Color lineColor, float borderSize = 5, int layer = 0, bool enableSampling = false)
        {
            int dist = (enableSampling && ((int)Zoom / 10 > 1)) ? (int)Zoom / 10 : 1;

            if (lines.Count > 0)
            {
                gl.LineWidth(borderSize);

                gl.Color(lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0, lineColor.A / 255.0);

                gl.Begin(OpenGL.GL_LINES);

                for (int i = 0; i < lines.Count; i += dist)
                {
                    gl.Vertex(lines[i].x1, lines[i].y1, -layer);
                    gl.Vertex(lines[i].x2, lines[i].y2, -layer);
                }

                gl.End();
            }
        }

        /// <summary>
        /// 一次繪製多條直線
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="lines">線條集合（List<Line>）</param>
        /// <param name="lineColor">線條顏色</param>
        /// <param name="borderSize">線條粗度</param>
        private void DrawLines(OpenGL gl, List<MapLIne> lines, int scale, Color lineColor, float borderSize = 5, int layer = 0)
        {
            int dist = ((int)Zoom / 10 > 1) ? (int)Zoom / 10 : 1;

            if (lines.Count > 0)
            {
                gl.LineWidth(borderSize);

                gl.Color(lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0, lineColor.A / 255.0);

                gl.Begin(OpenGL.GL_LINES);

                for (int i = 0; i < lines.Count; i += dist)
                {
                    gl.Vertex(lines[i].x1, lines[i].y1, -layer);
                    gl.Vertex(lines[i].x2, lines[i].y2, -layer);
                }

                gl.End();
            }
        }

        /// <summary>
        /// 繪製所有不同粗細、顏色的線條
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="lineSet">線條總集合</param>
        private void DrawLines(OpenGL gl, List<LineGroup> lineSet)
        {
            lock (mLkLinesSet)
            {
                if (lineSet != null && lineSet.Count > 0)
                    for (int i = 0; i < lineSet.Count; i++)
                    {
                        DrawLines(gl, lineSet[i].DataList, lineSet[i].Color, lineSet[i].BorderSize, lineSet[i].Layer, lineSet[i].EnableSampling);
                    } 
            }
        }

        /// <summary>
        /// 繪製單條直線
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="start">線條起始點位</param>
        /// <param name="end">線條末端點位</param>
        /// <param name="lineColor">線條顏色</param>
        /// <param name="borderSize">線條粗度</param>
        private void DrawLine(OpenGL gl, Point start, Point end, Color lineColor, float borderSize = 5, int layer = 0)
        {
            lock (mLkLinesSet)
            {
                gl.LineWidth(borderSize);

                gl.Color(lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0, lineColor.A / 255.0);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(start.X, start.Y, -layer);
                gl.Vertex(end.X, end.Y, -layer);
                gl.End(); 
            }
        }

        /// <summary>
        /// 繪出單向線條式箭頭的直線（箭頭朝向線的終點）
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="start">線條起始點</param>
        /// <param name="end">線條終點</param>
        /// <param name="lineColor">線條顏色</param>
        /// <param name="borderSize">線條粗度</param>
        /// <param name="arrawSize">箭頭大小</param>
        /// <param name="layer">於第幾圖層繪出</param>
        private void DrawLineArrow1(OpenGL gl, Point start, Point end, Color lineColor, float borderSize = 5, double arrawSize = 200, int layer = 0)
        {
            List<MapLIne> lines = new List<MapLIne>();
            double aS, aD, a0, a1 = 25;

            a0 = Math.Atan2(end.Y - start.Y, end.X - start.X);
            aS = a0 + (a1 * Math.PI / 180);
            aD = a0 - (a1 * Math.PI / 180);

            lines.Add(new MapLIne(start.X, start.Y, end.X, end.Y));
            lines.Add(new MapLIne(end.X, end.Y, end.X - (int)(arrawSize * Math.Cos(aS)), end.Y - (int)(arrawSize * Math.Sin(aS))));
            lines.Add(new MapLIne(end.X, end.Y, end.X - (int)(arrawSize * Math.Cos(aD)), end.Y - (int)(arrawSize * Math.Sin(aD))));

            DrawLines(gl, lines, lineColor, borderSize, layer);
        }

        /// <summary>
        /// 繪出單向實心式箭頭的直線（箭頭朝向線的終點）
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="start">線條起始點</param>
        /// <param name="end">線條終點</param>
        /// <param name="lineColor">線條顏色</param>
        /// <param name="borderSize">線條粗度</param>
        /// <param name="arrawSize">箭頭大小</param>
        /// <param name="layer">於第幾圖層繪出</param>
        private void DrawLineArrow2(OpenGL gl, Point start, Point end, Color lineColor, float borderSize = 5, double arrawSize = 200, int layer = 0)
        {
            double aS, aD, a0, a1 = 25;

            a0 = Math.Atan2(end.Y - start.Y, end.X - start.X);
            aS = a0 + (a1 * Math.PI / 180);
            aD = a0 - (a1 * Math.PI / 180);

            DrawLine(gl, start, end, lineColor, borderSize, layer);

            gl.Begin(OpenGL.GL_POLYGON);
            gl.Vertex(end.X, end.Y, -layer);
            gl.Vertex(end.X - arrawSize * Math.Cos(aS), end.Y - arrawSize * Math.Sin(aS), -layer);
            gl.Vertex(end.X - arrawSize * Math.Cos(aD), end.Y - arrawSize * Math.Sin(aD), -layer);
            gl.End();

            //gl.PopMatrix();
        }

        #endregion - Draw Line - 

        #region - Draw Shape - 
        /// <summary>
        /// 矩形圖形填滿資料收集
        /// </summary>
        /// <param name="center"></param>
        /// <param name="body"></param>
        /// <param name="color"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="layer"></param>
        public void DrawRectangleFillFull(Pos center, Size body, Color color, string name = "", uint id = 1, int layer = (int)ItemLayout.Goal)
        {
            int i = 0;
            bool getGroup = false;
            int numGroup = mPolygonFillFullSet.Count;
            for (i = 0; i < numGroup; i++)
            {
                if (mPolygonFillFullSet[i].Name == (string.Format("{0}_{1}", name, id)))
                {
                    getGroup = true;
                    numGroup = i;
                    break;
                }
            }
            if (getGroup)
            {
                mPolygonFillFullSet[numGroup].Center.Add(center);
                mPolygonFillFullSet[numGroup].Frame.Add(body);
            }
            else
            {
                PolygonFillFull item = new PolygonFillFull();
                item.Center.Add(center);
                item.Frame.Add(body);
                item.Id = id;
                item.Name = string.Format("{0}_{1}", name, id);
                item.shape = Shape.FillRectangle;
                item.Color = color;
                item.Layer = layer;
                mPolygonFillFullSet.Add(item);
            }

        }

        public void RemovePolygonFillFull(string name)
        {
            string shapename = "";
            for (int i = 0; i < mPolygonFillFullSet.Count; i++)
            {
                shapename = string.Format("{0}_{1}", name, mPolygonFillFullSet[i].Id);
                if (mPolygonFillFullSet[i].Name == shapename)
                {
                    mPolygonFillFullSet[i].Center = null;
                    mPolygonFillFullSet[i].Frame = null;
                    mPolygonFillFullSet.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemovePolygonFillFull(uint id)
        {
            for (int i = 0; i < mPolygonFillFullSet.Count; i++)
            {
                if (mPolygonFillFullSet[i].Id == id)
                {
                    mPolygonFillFullSet[i].Center = null;
                    mPolygonFillFullSet[i].Frame = null;
                    mPolygonFillFullSet.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveAllPolygonFillFull()
        {
            for (int i = 0; i < mPolygonFillFullSet.Count; i++)
            {
                mPolygonFillFullSet[i].Name = null;
                mPolygonFillFullSet[i].Frame = null;
                mPolygonFillFullSet[i].Center = null;
            }
            mPolygonFillFullSet.Clear();
        }

        /// <summary>
        /// 從 enum Shape 中選出想要繪出的形狀(底層圖案，暫時不改)
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="item">從 Shape 形狀列舉中擇一形狀</param>
        /// <param name="locX">多邊形的中心點 X</param>
        /// <param name="locY">多邊形的中心點 Y</param>
        /// <param name="theta">形狀要旋轉幾度角度（0 ~ 359 度）</param>
        /// <param name="iconColor">形狀顏色</param>
        /// <param name="body">形狀的大小</param>
        /// <param name="layer">指定形狀要放置的圖層</param>
        private void DrawShape(OpenGL gl, Shape item, double locX, double locY, double theta, Color iconColor, Size body, float borderSize = 1, int layer = (int)ItemLayout.Top)
        {
            switch (item)
            {
                case Shape.Ellipse:
                    {
                        DrawPolygon(gl, ShapeOval(body.Width, body.Height), locX, locY, iconColor, theta, borderSize, layer);
                    }
                    break;

                case Shape.Rectangle:
                    {
                        DrawPolygon(gl, ShapeRectangle(body.Width, body.Height), locX, locY, iconColor, theta, borderSize, layer);
                    }
                    break;

                case Shape.FillEllipse:
                    {
                        DrawPolygonFillFull(gl, ShapeOval(body.Width, body.Height), locX, locY, iconColor, theta, layer);
                    }
                    break;

                case Shape.FillRectangle:
                    {
                        DrawPolygonFillFull(gl, ShapeRectangle(body.Width, body.Height), locX, locY, iconColor, theta, layer);
                    }                    
                    break;
                case Shape.Cursor:
                    break;
            }
        }

        /// <summary>
        /// 繪畫底層以外的多邊形圖案(需填滿的圖案)
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="shapedata">圖形資料</param>
        /// <param name="borderSize">形狀大小</param>
        private void DrawShape(OpenGL gl, List<PolygonFillFull> shapedata)
        {
            int i = 0, j = 0;
            int datacnt = shapedata.Count;
            List<Pos> center = new List<Pos>(); ;
            List<Size> frame = new List<Size>();
            List<Point> point = new List<Point>();
            Color FillColor = new Color();

            for (i = 0; i < datacnt; i++)
            {
                center = shapedata[i].Center;
                frame = shapedata[i].Frame;
                FillColor = shapedata[i].Color;

                switch (shapedata[i].shape)
                {
                    case Shape.Ellipse:
                        {
                            for (j = 0; j < center.Count; j++)
                            {
                                DrawPolygon(gl, ShapeOval(frame[j].Width, frame[j].Height), center[j].x, center[j].y, FillColor, 0, 1, -(int)ItemLayout.Point);
                            }
                        }
                        break;

                    case Shape.Rectangle:
                        {
                            for (j = 0; j < center.Count; j++)
                            {
                                DrawPolygon(gl, ShapeRectangle(frame[j].Width, frame[j].Height), center[j].x, center[j].y, FillColor, 0, 1, -(int)ItemLayout.Point);
                            }
                        }
                        break;

                    case Shape.FillEllipse:
                        {
                            for (j = 0; j < center.Count; j++)
                            {
                                DrawPolygonFillFull(gl, ShapeOval(frame[j].Width, frame[j].Height), center[j].x, center[j].y, FillColor, 0, -(int)ItemLayout.Point);
                            }
                        }
                        break;

                    case Shape.FillRectangle:
                        {
                            gl.LoadIdentity();
                            gl.PushMatrix();
                            gl.Translate(mTranslate.X * Zoom, mTranslate.Y * Zoom, 0.0);
                            gl.Disable(OpenGL.GL_DEPTH_TEST);
                            gl.Enable(OpenGL.GL_BLEND);

                            gl.Begin(OpenGL.GL_QUADS);



                            for (j = 0; j < center.Count; j++)
                            {
                                if (Color.White == FillColor)
                                    if (Zoom > 30)
                                        gl.Color(mColorGrid.R / 255.0, mColorGrid.G / 255.0, mColorGrid.B / 255.0, 0);
                                    else
                                        gl.Color(ColorBackground.R / 255.0, ColorBackground.G / 255.0, ColorBackground.B / 255.0, 0);
                                else
                                    gl.Color(FillColor.R / 255.0, FillColor.G / 255.0, FillColor.B / 255.0, 0.7);
                                point = ShapeRectangle(frame[j].Width, frame[j].Height);
                                gl.Vertex(point[0].X + center[j].x, point[0].Y + center[j].y, -shapedata[i].Layer);
                                gl.Vertex(point[1].X + center[j].x, point[1].Y + center[j].y, -shapedata[i].Layer);
                                gl.Vertex(point[2].X + center[j].x, point[2].Y + center[j].y, -shapedata[i].Layer);
                                gl.Vertex(point[3].X + center[j].x, point[3].Y + center[j].y, -shapedata[i].Layer);
                            }

                            gl.End();
                            gl.BlendFunc(OpenGL.GL_ONE, OpenGL.GL_SRC_ALPHA);
                            gl.Disable(OpenGL.GL_BLEND);
                            gl.Enable(OpenGL.GL_DEPTH_TEST);
                            gl.PopMatrix();
                            //gl.DepthMask(1);
                        }
                        break;
                }
            }
            center = null;
            frame = null;
            point = null;
        }


        /// <summary>
        /// 繪製填滿顏色的多邊形
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="shape">多邊形所有折點的座標</param>
        /// <param name="tx">多邊形的中心點 X</param>
        /// <param name="ty">多邊形的中心點 Y</param>
        /// <param name="FillColor">填充顏色</param>
        /// <param name="theta">形狀要旋轉幾度角度（0 ~ 359 度）</param>
        /// <param name="layer">指定形狀要放置的圖層</param>
        private void DrawPolygonFillFull(OpenGL gl, List<Point> shape, double tx, double ty, Color FillColor, double theta, int layer = 0)
        {
            double d, primeRadian, radian = theta * Math.PI / 180;

            gl.Color(FillColor.R / 255.0, FillColor.G / 255.0, FillColor.B / 255.0);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Translate(mTranslate.X * Zoom + tx, mTranslate.Y * Zoom + ty, 0.0);

            gl.Begin(OpenGL.GL_POLYGON);

            for (int i = 0; i < shape.Count; i++)
            {
                primeRadian = Math.Atan2(shape[i].Y, shape[i].X);
                d = Math.Sqrt((shape[i].X) * (shape[i].X) + (shape[i].Y) * (shape[i].Y));
                gl.Vertex(d * Math.Cos(radian + primeRadian), d * Math.Sin(radian + primeRadian), -layer);
            }

            gl.End();
            gl.PopMatrix();
        }


        /// <summary>
        /// 繪製空心的多邊形
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="shape">多邊形所有折點的座標</param>
        /// <param name="tx">多邊形的中心點 X</param>
        /// <param name="ty">多邊形的中心點 Y</param>
        /// <param name="lineColor">連線顏色</param>
        /// <param name="theta">多邊形旋轉角度</param>
        /// <param name="borderSize"></param>
        private void DrawPolygon(OpenGL gl, List<Point> shape, double tx, double ty, Color lineColor, double theta, float borderSize = 1, int layer = 0)
        {
            double d, primeRadian, radian = theta * Math.PI / 180;

            gl.Color(lineColor.R / 255.0, lineColor.G / 255.0, lineColor.B / 255.0);
            gl.LineWidth(borderSize);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Translate(mTranslate.X * Zoom + tx, mTranslate.Y * Zoom + ty, 0);

            gl.Begin(OpenGL.GL_LINE_LOOP);

            for (int i = 0; i < shape.Count; i++)
            {
                primeRadian = Math.Atan2(shape[i].Y, shape[i].X);
                d = Math.Sqrt((shape[i].X) * (shape[i].X) + (shape[i].Y) * (shape[i].Y));
                //gl.Vertex(shape[i].X, shape[i].Y, 0.0);
                //gl.Vertex(d * Math.Cos(theta * PI_180) + shape[i].X, d * Math.Sin(theta * PI_180) + shape[i].Y, 0.0);
                gl.Vertex(d * Math.Cos(radian + primeRadian), d * Math.Sin(radian + primeRadian), -layer);
            }

            gl.End();

            gl.PopMatrix();
        }
        #endregion - Draw Shape - 

        #region - Draw Coordinate System - 
        /// <summary>
        /// XY 座標軸線
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="borderSize">軸線粗度</param>
        /// <param name="layer">軸線的圖層</param>
        private void DrawCoordinate(OpenGL gl, float borderSize = 1, int layer = (int)ItemLayout.Axis)
        {
            Point center = new Point((MinPos.X + MaxPos.X) / 2, (MinPos.Y + MaxPos.Y) / 2);

            if (EnableAxis)
            {
                DrawLineArrow1(
                    gl,
                    new Point(MinPos.X, 0),
                    new Point(MaxPos.X, 0),
                    mColorAxisX,
                    borderSize,
                    mSizeGrid,
                    layer
                );
                DrawLineArrow1(
                    gl,
                    new Point(0, MinPos.Y),
                    new Point(0, MaxPos.Y),
                    mColorAxisY,
                    borderSize,
                    mSizeGrid,
                    layer
                );
            }
        }

        /// <summary>
        /// 繪畫背景格線
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="gridColor">格線顏色</param>
        /// <param name="tx">格線原點 x 座標</param>
        /// <param name="ty">格線原點 y 座標</param>
        /// <param name="layer">格線的圖層</param>
        private void DrawGrid(OpenGL gl, Color gridColor, double tx, double ty, int layer = (int)ItemLayout.Grid)
        {
            int maxCount = 5000;

            if (EnableGrid)
            {
                gl.Color(gridColor.R / 255.0, gridColor.G / 255.0, gridColor.B / 255.0);
                gl.LineWidth(0.05f);
                gl.PushMatrix();
                gl.LoadIdentity();
                gl.Translate(tx % mSizeGrid, ty % mSizeGrid, 0);

                gl.Begin(OpenGL.GL_LINES);

                for (int i = 0; i < maxCount; i++)
                {
                    //+x
                    gl.Vertex((i) * mSizeGrid, -(maxCount) * mSizeGrid, -layer);
                    gl.Vertex((i) * mSizeGrid, +(maxCount) * mSizeGrid, -layer);
                    //-x
                    gl.Vertex(-(i) * mSizeGrid, -(maxCount) * mSizeGrid, -layer);
                    gl.Vertex(-(i) * mSizeGrid, +(maxCount) * mSizeGrid, -layer);
                    //+y
                    gl.Vertex(-(maxCount) * mSizeGrid, (i) * mSizeGrid, -layer);
                    gl.Vertex(+(maxCount) * mSizeGrid, (i) * mSizeGrid, -layer);
                    //-y
                    gl.Vertex(-(maxCount) * mSizeGrid, -(i) * mSizeGrid, -layer);
                    gl.Vertex(+(maxCount) * mSizeGrid, -(i) * mSizeGrid, -layer);
                }

                gl.End();
                gl.PopMatrix();
            }
        }

        /// <summary>
        /// 顯示滑鼠游標位置
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="pt">字體顏色</param>
        /// <param name="fontSize">字體大小</param>
        private void DrawMousePos(OpenGL gl, Color pt, int fontSize)
        {
            Point pos = new Point(0, 0);
            byte[] colorPixel = new byte[6];

            if (EnableMouseLocatation)
            {
                pos = MouseToGL();

                DrawShape(
                    gl, GoalShape, pos.X, pos.Y, 0,
                    Color.FromArgb(ColorGoalIcon.A, ColorGoalIcon.R * 200 / 255, ColorGoalIcon.G * 200 / 255, ColorGoalIcon.B * 200 / 255),
                    SizeGoal,
                    3,
                    (int)ItemLayout.Grid
                );

                DrawText(gl, String.Format("({0},{1})", pos.X, pos.Y), pos.X, pos.Y, pt, mFontText.Size);
            }
        }
        #endregion - Draw Coordinate System - 

        #region - Draw Item - 
        // The texture identifier.
        Texture texture = new Texture();

        // 這個顯示程式有 Bug，無法正常顯示 png 檔的 icon（先留著，日後再找其他方法修正）
        private void DrawIcon(OpenGL gl, double zoom, int xPos, int yPos, int zPos = -6)
        {
            //Bitmap orimg = new Bitmap(mCarIcon);
            Bitmap img = new Bitmap(100, 100);

            //  Create our texture object from a file. This creates the texture for OpenGL.
            texture.Create(gl, img);//new Bitmap(Properties.Resources.CarIcon, 100, 100)//Properties.Resources.CarIcon

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHT1);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            //gl.LoadIdentity();
            gl.Translate(xPos, yPos, zPos);

            //  Bind the texture.
            texture.Bind(gl);

            //gl.Scale(20 * zoom / 100.0, 20 * zoom / 100.0, 20 * zoom / 100.0);

            gl.PushMatrix();

            gl.Begin(OpenGL.GL_POLYGON); //.GL_QUADS
            {
                // Front Face
                // Bottom Left Of The Texture and Quad
                gl.TexCoord(0.0, 0.0);
                gl.Vertex(1.0 * zoom, 1.0 * zoom, 0.0);

                // Bottom Right Of The Texture and Quad
                gl.TexCoord(-1.0, 0.0);
                gl.Vertex(-1.0 * zoom, 1.0 * zoom, 0.0);

                // Top Right Of The Texture and Quad
                gl.TexCoord(-1.0, -1.0);
                gl.Vertex(-1.0 * zoom, -1.0 * zoom, 0.0);

                // Top Left Of The Texture and Quad
                gl.TexCoord(0.0, -1.0);
                gl.Vertex(1.0 * zoom, -1.0 * zoom, 0.0);
            }

            gl.End();

            gl.Flush();
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_LIGHT0);
            gl.Disable(OpenGL.GL_LIGHT1);
        }

        private void DrawCar(OpenGL gl, Shape item, double locX, double locY, double theta, Color iconColor, Size body, int layer = (int)ItemLayout.Car)
        {
            double halfHeight = body.Height / 2;
            double halfWidth = body.Width / 2;
            double radian = theta * Math.PI / 180;
            Color AntiColor = Color.FromArgb(255 - iconColor.A, 255 - iconColor.R, 255 - iconColor.G, 255 - iconColor.B);

            if (EnableCar)
            {
                DrawShape(gl, item, locX, locY, theta, iconColor, body, 3, layer);

                DrawLineArrow1
                (
                    gl,
                    new Point((int)locX, (int)locY),
                    new Point((int)(locX + halfWidth * Math.Cos(radian)), (int)(locY + halfWidth * Math.Sin(radian))),
                    (item == Shape.FillEllipse || item == Shape.FillRectangle) ? AntiColor : iconColor,
                    1,
                    SizeCar.Width / 4,
                    layer + 1
                );

                DrawLine
                (
                    gl,
                    new Point((int)(locX + halfHeight * Math.Cos(radian - (Math.PI / 2))), (int)(locY + halfHeight * Math.Sin(radian - (Math.PI / 2)))),
                    new Point((int)(locX + halfHeight * Math.Cos(radian + (Math.PI / 2))), (int)(locY + halfHeight * Math.Sin(radian + (Math.PI / 2)))),
                    (item == Shape.FillEllipse || item == Shape.FillRectangle) ? AntiColor : iconColor,
                    1,
                    layer + 1
                );

                DrawText(gl, String.Format("({0:F2}, {1:F2}) D{2:F2}", locX, locY, theta), locX - halfWidth, locY + halfHeight, ColorCarIcon, mFontText.Size);
            }
        }

        private void DrawItem(OpenGL gl, Shape item, double locX, double locY, double theta, Color iconColor, Size body, int layer = (int)ItemLayout.Goal)
        {
            double halfHeight = body.Height / 2;
            double halfWidth = body.Width / 2;
            double radian = theta * Math.PI / 180;

            if (EnableGoal)
            {
                DrawShape(gl, item, locX, locY, theta, iconColor, body, 3, layer);

                DrawLineArrow1
                (
                    gl,
                    new Point((int)locX, (int)locY),
                    new Point((int)(locX + halfWidth * Math.Cos(radian)), (int)(locY + halfWidth * Math.Sin(radian))),
                    Color.FromArgb(255 - iconColor.A, 255 - iconColor.R, 255 - iconColor.G, 255 - iconColor.B),
                    1,
                    SizeCar.Width / 4,
                    layer + 1
                );

                DrawLine
                (
                    gl,
                    new Point((int)(locX + halfHeight * Math.Cos(radian - (Math.PI / 2))), (int)(locY + halfHeight * Math.Sin(radian - (Math.PI / 2)))),
                    new Point((int)(locX + halfHeight * Math.Cos(radian + (Math.PI / 2))), (int)(locY + halfHeight * Math.Sin(radian + (Math.PI / 2)))),
                    Color.FromArgb(255 - iconColor.A, 255 - iconColor.R, 255 - iconColor.G, 255 - iconColor.B),
                    1,
                    layer + 1
                );

                DrawText(gl, String.Format("({0}, {1}) D{2}", locX, locY, theta), locX - halfWidth, locY - body.Height, ColorCarIcon, mFontText.Size);
            }
        }

        private void DrawItem(OpenGL gl, Shape item, Pos loc, Color iconColor, Size body, int layer = (int)ItemLayout.Goal)
        {
            // 每建一個元件都要做一次 PushName，以便之後作為反饋元件
            gl.PushName(loc.id);
            gl.PushMatrix();

            DrawItem(gl, item, loc.x, loc.y, loc.theta, iconColor, body, layer);

            gl.PopMatrix();
            gl.PopName();
        }

        private void DrawItem(OpenGL gl, Shape item, List<Pos> listLoc, Color iconColor, Size body, int layer = (int)ItemLayout.Goal)
        {
            foreach (Pos pos in listLoc)
            {
                DrawItem(gl, item, pos, iconColor, body, layer);
            }
        }

        #endregion - Draw Item - 

        #region - List Points of Shape - 

        /// <summary>
        /// 以原點 (0, 0) 為中心列出圓的點位
        /// </summary>
        /// <param name="diameter">直徑</param>
        /// <param name="sampling">以等角度取 sampling 個點</param>
        /// <returns>回傳 sampling 個點位</returns>
        private List<Point> ShapeCircle(int diameter, int sampling = 10)
        {
            List<Point> rData;

            rData = ShapeOval(diameter, diameter, sampling);

            return rData;
        }

        /// <summary>
        /// 以原點 (0, 0) 為中心列出橢圓的點位
        /// </summary>
        /// <param name="width">橢圓的寬</param>
        /// <param name="height">橢圓的高</param>
        /// <param name="sampling">以等角度取 sampling 個點</param>
        /// <returns>回傳 sampling 個點位</returns>
        private List<Point> ShapeOval(int width, int height, int sampling = 10)
        {
            double PIx2 = 2 * Math.PI;
            double count = Math.PI / sampling;

            List<Point> rData = new List<Point>();

            for (double d = 0; d < PIx2; d += count)
            {
                rData.Add(new Point((int)(0.5 * width * Math.Cos(d)), (int)(0.5 * height * Math.Sin(d))));
            }

            return rData;
        }

        /// <summary>
        /// 以原點 (0, 0) 為中心列出矩形的點位
        /// </summary>
        /// <param name="width">矩形的寬</param>
        /// <param name="height">矩形的高</param>
        /// <returns>回傳 4 個點位</returns>
        private List<Point> ShapeRectangle(int width, int height)
        {
            List<Point> rData = new List<Point>();

            rData.Add(new Point(-width / 2, height / 2));
            rData.Add(new Point(-width / 2, -height / 2));
            rData.Add(new Point(width / 2, -height / 2));
            rData.Add(new Point(width / 2, height / 2));

            return rData;
        }

        #endregion - List Points of Shape - 

        #region - Frame Focus & Fit - 
        /// <summary>
        /// 將整張地圖縮放在畫框內作顯示
        /// </summary>
        public void Fit()
        {
            double zoomX, zoomY;

            // 當視窗縮小時 GL 的 size 會為 0，會使 GL 跳錯誤
            if (MapDisplayer.Width != 0 && MapDisplayer.Height != 0)
            {
                zoomX = (MaxPos.X - MinPos.X) / MapDisplayer.Width / 0.8;
                zoomY = (MaxPos.Y - MinPos.Y) / MapDisplayer.Height / 0.8;
                Zoom = Math.Max(zoomX, zoomY);

                Focus(((MaxPos.X + MinPos.X) * 0.5), ((MaxPos.Y + MinPos.Y) * 0.5));
            }
        }

        /// <summary> 將畫面鎖定在顯示某個點位上 </summary>
        /// <param name="x"> 鎖定座標 X </param>
        /// <param name="y"> 鎖定座標 Y </param>
		public void Focus(double x, double y)
        {
            mTranslate.X = (int)((0.5 * MapDisplayer.Width - x) / Zoom);
            mTranslate.Y = (int)((0.5 * MapDisplayer.Height - y) / Zoom);
        }

        /// <summary>
        /// 顯示車子現在所在的位置
        /// </summary>
        /// <param name="zoom">調整縮放（範圍 1~300）</param>
		public void WhereAmI(double zoom)
        {
            Zoom = zoom;
            Focus(PosCar.x, PosCar.y);
        }

        /// <summary>
        /// 顯示車子現在所在的位置
        /// </summary>
		public void WhereAmI()
        {
            Focus(PosCar.x, PosCar.y);
        }
        #endregion - Frame Focus & Fit - 

        #region - Transformation - 
        /// <summary> 將滑鼠的座標轉換成實際座標 </summary>
        /// <returns>回傳座標值</returns>
        private Point MouseToGL()
        {
            Point glP = PointToGL(MousePosition);
            return glP;
        }

        /// <summary> 將畫布上的座標轉換成顯示的座標 </summary>
        /// <returns>回傳座標值</returns>
        private Point PointToGL(Point rPosition)
        {
            Point glP = new Point();

            // 座標系計算方式有 2 種：
            // 1. 以點為中心將點置入 GL 畫布中央
            // 2. 以 GL 畫布最左上角為起始點開始計算座標
            // 目前選擇 1. 的作法

            // 1. 以點為中心將點置入 GL 畫布中央
            glP.X = PointToClient(rPosition).X - MapDisplayer.Size.Width / 2;
            glP.Y = PointToClient(rPosition).Y - MapDisplayer.Size.Height / 2;

            // 2. 以 GL 畫布最左上角為起始點開始計算座標
            //glP.X = MousePosition.X - PointToScreen(MapDisplayer.Location).X - MapDisplayer.Size.Width / 2;
            //glP.Y = MousePosition.Y - PointToScreen(MapDisplayer.Location).Y - MapDisplayer.Size.Height / 2;

            // 將 Y 做鏡像翻轉
            glP.Y *= -1;

            // 減去坐標系中的偏移量
            glP.X -= mTranslate.X;
            glP.Y -= mTranslate.Y;

            // 將座標乘以縮放比例，為了預防座標失真，必須先乘以 1000 轉整數後再除以 1000
            glP.X *= (int)(Zoom * 1000);
            glP.Y *= (int)(Zoom * 1000);
            glP.X /= 1000;
            glP.Y /= 1000;

            return glP;
        }
        #endregion - Transformation - 

        #region - Register Points (Power & Goal) - 

        /// <summary>
        /// 向派車系統註冊單個充電站的資訊
        /// </summary>
        /// <param name="position">註冊內容</param>
        public void AddPositonPower(Pos position)
        {
            ListPower.Add(position);
            // ID = 0代表非法 ID
            ListPower[ListPower.Count - 1].id = (uint)ListPower.Count + CountTotalCar;
            ListPower[ListPower.Count - 1].name = string.Format(CastecMapUI.ItemLayout.Power.ToString() + "_" + ListPower.Count);
            UpdateIdNameOffset();
        }

        /// <summary>
        /// 向派車系統註冊單台車子的資訊
        /// </summary>
        /// <param name="position">註冊內容</param>
        public void AddPositonGoal(Pos position)
        {
            ListGoal.Add(position);
            // ID = 0代表非法 ID
            ListGoal[ListGoal.Count - 1].id = (uint)ListGoal.Count + CountTotalCar + CountTotalPower;
            ListGoal[ListGoal.Count - 1].name = string.Format(CastecMapUI.ItemLayout.Goal.ToString() + "_" + ListPower.Count);
            UpdateIdNameOffset();
        }

        /// <summary>
        /// 指定要移除充電站的 id
        /// </summary>
        /// <param name="id">充電站 id</param>
        public void RemovePositonPower(uint id)
        {
            int index = 0;

            for (int i = 0; i < ListPower.Count; i++)
            {
                if (ListPower[i].id == id)
                    index = i;
            }

            ListPower.RemoveAt(index);
        }

        /// <summary>
        /// 指定要移除目標點位的 id
        /// </summary>
        /// <param name="id">點位 id</param>
        public void RemovePositonGoal(uint id)
        {
            int index = 0;

            for (int i = 0; i < ListGoal.Count; i++)
            {
                if (ListGoal[i].id == id)
                    index = i;
            }

            ListGoal.RemoveAt(index);
        }

        /// <summary>
        /// 移除所有充電站點位
        /// </summary>
        public void RemoveAllPositonPower()
        {
            ListPower.Clear();
        }

        /// <summary>
        /// 移除所有目標點位
        /// </summary>
        public void RemoveAllPositonGoal()
        {
            ListGoal.Clear();
        }

        /// <summary>
        /// 更新所有點位的 id
        /// </summary>
        private void UpdateIdNameOffset()
        {
            uint goalOffset = 0;

            CountTotalCar = (uint)ListCar.Count;
            CountTotalPower = (uint)ListPower.Count;

            goalOffset = CountTotalCar + CountTotalPower;

            for (int i = 0; i < ListPower.Count; i++)
            {
                ListPower[i].id += CountTotalCar;
                ListPower[i].name = string.Format("{0}_{1}", ItemLayout.Power.ToString(), ListPower[i].id);
            }

            for (int i = 0; i < ListGoal.Count; i++)
            {
                ListGoal[i].id += goalOffset;
                ListGoal[i].name = string.Format("{0}_{1}", ItemLayout.Goal.ToString(), ListGoal[i].id);
            }
        }

        #endregion - Register Points (Power & Goal) - 

        #region - Draw Button - 
        /// <summary> 繪製透明按鈕 </summary>
        /// <param name="gl">  </param>
        /// <param name="buttonLocation"> 按鈕中心坐落的位置 </param>
        /// <param name="pickMatrix"> 選取矩陣 </param>
        private void Start_Picking(OpenGL gl, Point buttonLocation, Size pickMatrix)
        {
            int[] viewport = new int[4];
            //selectBuf = new uint[4];

            // 這裡清除 Name
            gl.InitNames();

            gl.SelectBuffer(4, selectBuf);
            gl.RenderMode(OpenGL.GL_SELECT);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();

            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
            gl.PickMatrix(buttonLocation.X, viewport[3] - buttonLocation.Y, pickMatrix.Width / Zoom, pickMatrix.Height / Zoom, viewport);

            // 設定畫布
            gl.Ortho(-Zoom * MapDisplayer.Width / 2, Zoom * MapDisplayer.Width / 2, -Zoom * MapDisplayer.Height / 2, Zoom * MapDisplayer.Height / 2, 100, -100);//100, -100
        }

        /// <summary> 停止繪製透明按鈕 </summary>
        /// <param name="gl"></param>
        private void Stop_Picking(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Flush();

            int hits = gl.RenderMode(OpenGL.GL_RENDER);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            processHits(hits, selectBuf);
        }

        /// <summary> 回饋畫布上哪個按鈕被點選 </summary>
        /// <param name="hits"> 數值 </param>
        /// <param name="buffer">  </param>
        private void processHits(int hits, uint[] buffer)
        {
            // 只想要第一個選到的ID
            if (hits != 0)
            {
                // 表示選到的物體是有ID的
                if (buffer[0] != 0)
                {
                    uint id = buffer[3];

                    // 搜尋 id
                    Pos select1 = ListGoal.Find(delegate (Pos p) { return p.id == id; });
                    if (select1 != null) mMouseSelectObj(select1.name, select1.x, select1.y);

                    // 搜尋 id
                    Pos select2 = ListPower.Find(delegate (Pos p) { return p.id == id; });
                    if (select2 != null) mMouseSelectObj(select2.name, select2.x, select2.y);
                }
            }
        }
        #endregion - Draw Button - 

        #region - Mouse Events - 
        private void MapDisplayer_MouseDown(object sender, MouseEventArgs e)
        {
            RemoveGroupLine("drag");
            RemoveGroupLine("select");

            mIsMoseDown = true;
            mMousePoint = MousePosition;

            mMouseDownPoint = MouseToGL();

            mMouseClickRealPos(mMouseDownPoint.X, mMouseDownPoint.Y);

            mDisableMouseClick = true;

            if (e.Button == MouseButtons.Left)
                mDisableMouseClick = true;
            else
                mDisableMouseClick = false;
        }

        private void MapDisplayer_MouseMove(object sender, MouseEventArgs e)
        {
            List<MapLIne> frame = new List<MapLIne>();
            Point MouseMove = new Point();

            if (mIsMoseDown)
            {
                RemoveGroupLine("select");
                if (e.Button == MouseButtons.Right)
                {
                    mTranslate.X += MousePosition.X - mMousePoint.X;
                    mTranslate.Y -= MousePosition.Y - mMousePoint.Y;
                    mMousePoint = MousePosition;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    MouseMove = MouseToGL();
                    frame.Add(new MapLIne(mMouseDownPoint.X, mMouseDownPoint.Y, MouseMove.X, mMouseDownPoint.Y));
                    frame.Add(new MapLIne(MouseMove.X, mMouseDownPoint.Y, MouseMove.X, MouseMove.Y));
                    frame.Add(new MapLIne(MouseMove.X, MouseMove.Y, mMouseDownPoint.X, MouseMove.Y));
                    frame.Add(new MapLIne(mMouseDownPoint.X, MouseMove.Y, mMouseDownPoint.X, mMouseDownPoint.Y));
                    RemoveGroupLine("drag");
                    DrawLines(frame, Color.Plum, "drag", false, 1.0f, (int)ItemLayout.Top);
                }
            }
        }

        private void MapDisplayer_MouseUp(object sender, MouseEventArgs e)
        {
            List<MapLIne> line = new List<MapLIne>();
            mIsMoseDown = false;
            RemoveGroupLine("drag");
            RemoveGroupLine("select");

            if (e.Button == MouseButtons.Left)
            {
                mMouseUpPoint = MouseToGL();
                mMouseSelectRange(mMouseDownPoint.X, mMouseDownPoint.Y, mMouseUpPoint.X, mMouseUpPoint.Y);
                line.Add(new MapLIne(mMouseDownPoint.X, mMouseDownPoint.Y, mMouseUpPoint.X, mMouseDownPoint.Y));
                line.Add(new MapLIne(mMouseUpPoint.X, mMouseDownPoint.Y, mMouseUpPoint.X, mMouseUpPoint.Y));
                line.Add(new MapLIne(mMouseUpPoint.X, mMouseUpPoint.Y, mMouseDownPoint.X, mMouseUpPoint.Y));
                line.Add(new MapLIne(mMouseDownPoint.X, mMouseUpPoint.Y, mMouseDownPoint.X, mMouseDownPoint.Y));
                DrawLines(line, Color.HotPink, "select", false, 1.0f, (int)ItemLayout.Top);
            }
        }

        private void MapDisplayer_MouseWheel(object sender, MouseEventArgs e)
        {
            int countDelta = 0;

            countDelta += e.Delta;

            if (countDelta < 0)
                Zoom *= 1.2;
            else
                Zoom /= 1.2;
        }
        #endregion - Mouse Events - 

        /// <summary> 開立暫存圖像List </summary>
        /// <param name="gl"></param>
        /// <param name="enSavePoints">致能儲存障礙物點位</param>
        /// <param name="enSaveGoals">致能儲存目標點位</param>
        /// <param name="enSaveCars">致能儲存車子點位</param>
        /// <param name="enSaveLines">致能儲存所有線段</param>
        /// <param name="enSavePowers">致能儲存充電站點位</param>
        /// <returns>回傳創建好的圖像索引值</returns>
        private uint CreateList(
            OpenGL gl,
            bool enSavePoints = true, bool enSaveGoals = false,
            bool enSaveCars = false, bool enSaveLines = false,
            bool enSavePowers = false
        )
        {
            uint listKey = 0;

            gl.DeleteLists(1, 1);

            listKey = gl.GenLists(1);

            gl.NewList(listKey, OpenGL.GL_COMPILE);

            if (enSavePoints)
            {
                DrawPoints(gl, mPointsSet);
            }

            if (enSaveGoals)
            {

            }

            if (enSaveCars) { }

            if (enSaveLines)
            {
                DrawLines(gl, mLinesSet);
            }

            if (enSavePowers) { }

            gl.EndList();

            return listKey;
        }

        /// <summary> 清除所有暫存畫面 </summary>
        public void ClearBuffer()
        {
            OpenGL gl = MapDisplayer.OpenGL;
            gl.DeleteLists(mGLList, 1);
            gl.ClearColor(mColorBackground.R / 255.0f, mColorBackground.G / 255.0f, mColorBackground.B / 255.0f, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        /// <summary> 初始化畫布 </summary>
        /// <param name="gl"></param>
        private void InitialDraw(OpenGL gl)
        {
            // 當格線縮到極小時，將整個畫布背景色直接採用格線顏色以當作格線縮小的感覺，以取代格線繪製。
            if (Zoom < 30)
                gl.ClearColor(mColorBackground.R / 255.0f, mColorBackground.G / 255.0f, mColorBackground.B / 255.0f, 0.0f);
            else
                if (EnableGrid)
                gl.ClearColor(ColorGrid.R / 255.0f, ColorGrid.G / 255.0f, ColorGrid.B / 255.0f, 0.0f);
            else
                gl.ClearColor(mColorBackground.R / 255.0f, mColorBackground.G / 255.0f, mColorBackground.B / 255.0f, 0.0f);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // 投影矩陣
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();

            // 畫布的大小（正交）
            gl.Ortho(-Zoom * MapDisplayer.Width / 2, Zoom * MapDisplayer.Width / 2, -Zoom * MapDisplayer.Height / 2, Zoom * MapDisplayer.Height / 2, 100, -100);//100, -100

            // 繪圖矩陣
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // MatrixMode 後要執行 LoadIdentity
            gl.LoadIdentity();

            // 線條去鋸齒
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            // 點去鋸齒
            gl.Enable(OpenGL.GL_POINT_SMOOTH);

            // 多邊形去鋸齒
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);

            // 設定座標原點
            gl.Translate(mTranslate.X * Zoom, mTranslate.Y * Zoom, 0.0);

        }

        /// <summary> 主要繪圖事件 </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MapDisplayer_GDIDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = MapDisplayer.OpenGL;

            //#region -test-
            //if (gl.GetError() != 0)
            //{
            //    Console.WriteLine(gl.ErrorString(gl.GetError()));
            //}
            //#endregion

            #region - 1. Initial Draw - 
            InitialDraw(gl);
            #endregion - 1. Initial Draw - 

            #region - 2. ReDraw - 
            // 之後需要重繪功能的話，再把註解取消
            //if (mEnableReDraw) // 重繪
            //{
            //    mEnableReDraw = false;
            //    mGLList = CreateList(gl, mEnSavePoints, mEnSaveGoals, mEnSaveCars, mEnSaveLines, mEnSavePowers);
            //}
            //else
            //{
            //    gl.CallList(mGLList);
            //}
            #endregion - 2. ReDraw - 

            #region - 3. Drawing Objects - 
            // 當格線縮到極小時，不繪製格線，以加快顯示速度。
            if (Zoom < 30)
                DrawGrid(gl, mColorGrid, mTranslate.X * Zoom, mTranslate.Y * Zoom);

            DrawCoordinate(gl, 1);

            // 顯示游標座標（換算成地圖上的實際座標）
            DrawMousePos(gl, mColorMousePos, 8);

            // 繪畫目標點位的標記
            DrawItem(gl, GoalShape, ListGoal, ColorGoalIcon, SizeGoal);

            // 繪畫充電站點位的標記
            DrawItem(gl, GoalShape, ListPower, ColorPowerIcon, SizeGoal);

            // 繪畫車子座標的標記
            DrawCar(gl, CarShape, PosCar.x, PosCar.y, PosCar.theta, ColorCarIcon, SizeCar);

            // 一次繪畫所有線段集合
            DrawLines(gl, mLinesSet);

            // 一次繪畫所有點位集合
            DrawPoints(gl, mPointsSet);

            //一次繪畫所有矩形圖案
            DrawShape(gl, mPolygonFillFullSet);

            #endregion - 3. Drawing Objects - 

            #region - Draw Icon of Mouse - 
            if (mDisableMouseClick)
            {
                mMouseClickPoint = PointToGL(mMousePoint);
                DrawShape(
                    gl, GoalShape, mMouseClickPoint.X, mMouseClickPoint.Y, 0,
                    Color.FromArgb(ColorGoalIcon.A, ColorGoalIcon.R * 200 / 255, ColorGoalIcon.G * 200 / 255, ColorGoalIcon.B * 200 / 255),
                    SizeGoal,
                    3,
                    (int)ItemLayout.Grid
                );
            }
            #endregion - Draw Icon of Mouse - 

            #region - Draw Transparent Button - 
            // 當滑鼠點擊時，繪畫物件的隱形按鈕（按鈕圖層）
            if (mIsMouseClick)
            {
                // 取消滑鼠點擊
                mIsMouseClick = false;
                Start_Picking(MapDisplayer.OpenGL, mMouseClickPos, SizeGoal);
                DrawItem(gl, GoalShape, ListGoal, ColorGoalIcon, SizeGoal);
                DrawItem(gl, GoalShape, ListPower, ColorPowerIcon, SizeGoal);
                Stop_Picking(MapDisplayer.OpenGL);
            }
            #endregion - Draw Transparent Button - 
        }

        /// <summary> 重設視窗大小時觸發顯示整個畫布 </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapDisplayer_Resize(object sender, EventArgs e)
        {
            Fit();
        }

        /// <summary> 當畫布偵測到滑鼠點擊時，紀錄點擊時的座標 </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapDisplayer_MouseClick(object sender, MouseEventArgs e)
        {
            mIsMouseClick = true;
            mMouseClickPos = e.Location;
        }

        /// <summary> 畫布載入時先設定預設背景 </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CastecMapUI_Load(object sender, EventArgs e)
        {
            OpenGL gl = MapDisplayer.OpenGL;
            gl.ClearColor(mColorBackground.R / 255.0f, mColorBackground.G / 255.0f, mColorBackground.B / 255.0f, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }
    }
}
