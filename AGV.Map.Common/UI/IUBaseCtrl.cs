using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV.Map.Common.UI
{
    public delegate void DelDrag(ISingle<ITowardPair> single , uint id);
    /// <summary>
    /// UI 控制底層
    /// </summary>
    public interface IUIBaseCtrl
    {
        /// <summary>
        /// 拖曳事件
        /// </summary>
        event DelDrag DragEvent;

        /// <summary>
        /// 獲得控制底層
        /// </summary>
        IUIBaseCtrl BaseCtrl { get; }

        /// <summary>
        /// 平移
        /// </summary>
        IPair Translate { get; }

        /// <summary>
        /// 縮放
        /// </summary>
        IBound<double> Zoom { get; }

        /// <summary>
        /// 設定焦點
        /// </summary>
        void Focus<T>(T focus) where T : IPair;

        /// <summary>
        /// 設定焦點
        /// </summary>
        void Focus(int x, int y);

        /// <summary>
        /// 實際座標轉螢幕座標
        /// </summary>
        IPair GLToScreen<T>(T gl) where T : IPair;

        /// <summary>
        /// 實際座標轉螢幕座標
        /// </summary>
        IPair GLToScreen(int x, int y);

        /// <summary>
        /// 新地圖(刪除所有共用 Database)
        /// </summary>
        void NewMap();

        /// <summary>
        /// 螢幕座標轉實際座標
        /// </summary>
        IPair ScreenToGL<T>(T screen) where T : IPair;

        /// <summary>
        /// 螢幕座標轉實際座標
        /// </summary>
        IPair ScreenToGL(int x, int y);
    }
}