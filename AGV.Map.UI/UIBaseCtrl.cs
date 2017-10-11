using AGV.Map.Common;
using AGV.Map.Common.UI;
using AGV.Map.Core;

namespace AGV.Map.UI
{
    public partial class UIControl : IUIBaseCtrl
    {
        /// <summary>
        /// 拖曳事件
        /// </summary>
        public event DelDrag DragEvent;

        /// <summary>
        /// 獲得控制底層
        /// </summary>
        public IUIBaseCtrl BaseCtrl { get { return this; } }

        /// <summary>
        /// 平移
        /// </summary>
        public IPair Translate { get; } = Factory.FGeometry.Pair();

        /// <summary>
        /// 縮放
        /// </summary>
        public IBound<double> Zoom { get; } = Factory.FOthers.Bound<double>(1, 60);

        /// <summary>
        /// 設定焦點
        /// </summary>
        public void Focus<T>(T focus) where T : IPair
        {
            Focus(focus.X, focus.Y);
        }

        /// <summary>
        /// 設定焦點
        /// </summary>
        public void Focus(int x, int y)
        {
            Translate.X = -x;
            Translate.Y = -y;
        }

        /// <summary>
        /// 實際座標轉螢幕座標
        /// </summary>
        public IPair GLToScreen<T>(T gl) where T : IPair
        {
            return GLToScreen(gl.X, gl.Y);
        }



        /// <summary>
        /// 實際座標轉螢幕座標
        /// </summary>
        public IPair GLToScreen(int x, int y)
        {
            double mX = (x + Translate.X) / Zoom.Value;
            double mY = (y + Translate.Y) / Zoom.Value;
            return Factory.FGeometry.Pair((int)(mX + Width / 2), (int)(Height / 2 - mY));
        }

        /// <summary>
        /// 新地圖(刪除所有共用 Database 並釋放拖曳對象)
        /// </summary>
        public void NewMap()
        {
            mDragTargetID = 0;
            mDragManager.DragTaeget = null;
            Database.AGVGM.Clear();
            Database.ForbiddenAreaGM.Clear();
            Database.ForbiddenLineGM.Clear();
            Database.GoalGM.Clear();
            Database.LaserPointsGM.Clear();
            Database.NarrowLineGM.Clear();
            Database.ObstacleLinesGM.Clear();
            Database.ObstaclePointsGM.Clear();
            Database.ParkingGM.Clear();
            Database.PowerGM.Clear();
        }

        /// <summary>
        /// 螢幕座標轉實際座標
        /// </summary>
        public IPair ScreenToGL<T>(T screen) where T : IPair
        {
            return ScreenToGL(screen.X, screen.Y);
        }

        /// <summary>
        /// 螢幕座標轉實際座標
        /// </summary>
        public IPair ScreenToGL(int x, int y)
        {
            double mX = x - Width / 2;
            double mY = Height / 2 - y;
            return Factory.FGeometry.Pair((int)(mX * Zoom.Value - Translate.X), (int)(mY * Zoom.Value - Translate.Y));
        }
    }
}