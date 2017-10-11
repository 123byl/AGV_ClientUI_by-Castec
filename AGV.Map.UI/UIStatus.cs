using AGV.Map.Common;
using AGV.Map.Core;

namespace AGV.Map.UI
{
    partial class UIControl : IUStatus
    {
        /// <summary>
        /// 是否允許滑鼠編輯
        /// </summary>
        public bool AllowEdit { get; set; } = true;

        /// <summary>
        /// 是否畫坐標軸
        /// </summary>
        public bool ShowAxis { get; set; } = true;

        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool ShowFPS { get { return this.DrawFPS; } set { this.DrawFPS = value; } }

        /// <summary>
        /// 是否畫網格
        /// </summary>
        public bool ShowGrid { get; set; } = true;

        /// <summary>
        /// 是否顯示物件名稱
        /// </summary>
        public bool ShowNames { get; set; } = true;

        /// <summary>
        /// 獲得狀態控制項
        /// </summary>
        public IUStatus Status { get { return this; } }

        #region 顏色

        /// <summary>
        /// X 軸顏色
        /// </summary>
        public IColor AxisXColor { get; } = Factory.FOthers.Color(System.Drawing.Color.Red);

        /// <summary>
        /// Y 軸顏色
        /// </summary>
        public IColor AxisYColor { get; } = Factory.FOthers.Color(System.Drawing.Color.Green);

        /// <summary>
        /// 背景色
        /// </summary>
        public IColor BackgroundColor { get; } = Factory.FOthers.Color(System.Drawing.Color.Wheat);

        /// <summary>
        /// 網格顏色
        /// </summary>
        public IColor GridColor { get; } = Factory.FOthers.Color(System.Drawing.Color.Gray);

        /// <summary>
        /// 文字顏色
        /// </summary>
        public IColor TextColor { get; } = Factory.FOthers.Color(System.Drawing.Color.Black);

        #endregion 顏色
    }
}