using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehiclePlanner.Partial.VehiclePlannerUI {

    ///// <summary>
    ///// 系統列圖示類
    ///// </summary>
    //public class CtNotifyIcon : IDisposable {

    //    #region Declaration - Field

    //    /// <summary>
    //    /// 系統列圖示右鍵選單物件
    //    /// </summary>
    //    private ContextMenu mContextMenu = new ContextMenu();

    //    /// <summary>
    //    /// 右鍵圖示物件
    //    /// </summary>
    //    private NotifyIcon mNotifyIcon = new NotifyIcon();

    //    /// <summary>
    //    /// 要隱藏的表單參考
    //    /// </summary>
    //    private Form rForm = null;

    //    /// <summary>
    //    /// 右鍵選項集合
    //    /// </summary>
    //    private MenuItems mMenuItems = null;

    //    #endregion Declaration - Field

    //    #region Declaration - Properties

    //    /// <summary>
    //    /// 系統列圖示是否可視
    //    /// </summary>
    //    public bool Visible { get { return mNotifyIcon.Visible; } set { mNotifyIcon.Visible = value; } }

    //    /// <summary>
    //    /// 右鍵選項集合
    //    /// </summary>
    //    public MenuItems MenuItems {
    //        get {
    //            return mMenuItems;
    //        }
    //        set {
    //            mMenuItems = value;
    //            mContextMenu.MenuItems.AddRange(mMenuItems.Items.ToArray());
    //        }
    //    }

    //    #endregion Declaration - Properties

    //    #region Declaration - Events

    //    /// <summary>
    //    /// 滑鼠雙擊系統列視窗事件
    //    /// </summary>
    //    public event MouseEventHandler OnMouseDoubleClick {
    //        add {
    //            mNotifyIcon.MouseDoubleClick += value;
    //        }
    //        remove {
    //            mNotifyIcon.MouseDoubleClick -= value;
    //        }
    //    }

    //    /// <summary>
    //    /// 滑鼠放開事件
    //    /// </summary>
    //    public event MouseEventHandler OnMouseUp {
    //        add {
    //            mNotifyIcon.MouseUp += value;
    //        }
    //        remove {
    //            mNotifyIcon.MouseUp -= value;
    //        }

    //    }

    //    #endregion Declaration - Events

    //    #region Function - Consturctors

    //    /// <summary>
    //    /// 一般建構方法
    //    /// </summary>
    //    /// <param name="form">要隱藏的表單參考</param>
    //    /// <param name="caption">系統列圖示標題</param>
    //    /// <param name="icon">系統列圖示Icon</param>
    //    public CtNotifyIcon(Form form, string caption = "NotifyIcon", Icon icon = null) {
    //        rForm = form;
    //        mNotifyIcon.Icon = icon ?? rForm.Icon;
    //        mNotifyIcon.Text = caption;
    //        mNotifyIcon.ContextMenu = mContextMenu;
    //    }

    //    #endregion Function - Consturctors

    //    #region Function - Public Methods

    //    /// <summary>
    //    /// 增加右鍵選項
    //    /// </summary>
    //    /// <param name="item"></param>
    //    public void MenuItemAdd(MenuItem item) {
    //        if (!mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Add(item);
    //    }

    //    /// <summary>
    //    /// 移除右鍵選項
    //    /// </summary>
    //    /// <param name="item"></param>
    //    public void MenuItemRemove(MenuItem item) {
    //        if (mContextMenu.MenuItems.Contains(item)) mContextMenu.MenuItems.Remove(item);
    //    }

    //    /// <summary>
    //    /// 顯示系統列圖示
    //    /// </summary>
    //    public void ShowIcon() {
    //        mNotifyIcon.Visible = true;
    //    }

    //    /// <summary>
    //    /// 隱藏系統列圖示
    //    /// </summary>
    //    public void HideIcon() {
    //        mNotifyIcon.Visible = false;
    //    }

    //    /// <summary>
    //    /// 顯示系統列提示
    //    /// </summary>
    //    /// <param name="title"></param>
    //    /// <param name="context"></param>
    //    /// <param name="tmo">多久以後關閉</param>
    //    /// <param name="icon">Icon類型</param>
    //    public void ShowBalloonTip(string title, string context, int tmo = 15, ToolTipIcon icon = ToolTipIcon.Info) {
    //        mNotifyIcon.ShowBalloonTip(tmo, title, context, icon);
    //    }

    //    /// <summary>
    //    /// 顯示右鍵選單
    //    /// </summary>
    //    public void ShowMenuItem() {
    //        /*-- 以反射方式執行ShowContextMenu方法顯示右鍵選單 --*/
    //        Type t = typeof(NotifyIcon);
    //        MethodInfo mi = t.GetMethod("ShowContextMenu", BindingFlags.NonPublic | BindingFlags.Instance);
    //        mi.Invoke(this.mNotifyIcon, null);
    //    }

    //    #endregion Function - Public Methods

    //    #region IDisposable Support
    //    private bool disposedValue = false; // 偵測多餘的呼叫

    //    protected virtual void Dispose(bool disposing) {
    //        if (!disposedValue) {
    //            if (disposing) {
    //                // TODO: 處置 Managed 狀態 (Managed 物件)。

    //            }

    //            // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
    //            // TODO: 將大型欄位設為 null。

    //            HideIcon();

    //            (mMenuItems as MenuItems)?.Dispose();
    //            mMenuItems = null;

    //            mNotifyIcon?.Dispose();
    //            mNotifyIcon = null;

    //            mContextMenu?.Dispose();
    //            mContextMenu = null;

    //            disposedValue = true;
    //        }
    //    }

    //    // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
    //    // ~CtNotifyIcon() {
    //    //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
    //    //   Dispose(false);
    //    // }

    //    // 加入這個程式碼的目的在正確實作可處置的模式。
    //    public void Dispose() {
    //        // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
    //        Dispose(true);
    //        // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
    //        // GC.SuppressFinalize(this);
    //    }
    //    #endregion


    //}

    ///// <summary>
    ///// 右鍵選單管理類
    ///// </summary>
    ///// <remarks>
    ///// 用於建立
    ///// </remarks>
    //public class MenuItems : IDisposable {

    //    #region Declaration - Fields

    //    /// <summary>
    //    /// 右鍵選單集合
    //    /// </summary>
    //    private List<MenuItem> mMenuItems = new List<MenuItem>();

    //    /// <summary>
    //    /// 右鍵選單事件集合
    //    /// </summary>
    //    private List<EventHandler> mClickEvents = new List<EventHandler>();

    //    #endregion Declaration -Fields

    //    #region Declaration Properties

    //    /// <summary>
    //    /// 右鍵選單集合
    //    /// </summary>
    //    public List<MenuItem> Items {
    //        get {
    //            return mMenuItems;
    //        }
    //    }

    //    #endregion Declaration - Properties

    //    #region Function - Public Methods

    //    /// <summary>
    //    /// 傳入選單標題、方法、是否可視，建立新的右鍵選單後加入集合
    //    /// </summary>
    //    /// <param name="caption">選單標題</param>
    //    /// <param name="even">選單觸發時的處理方法</param>
    //    /// <param name="enable">是否可視</param>
    //    /// <returns>建構出的<see cref="MenuItem"/></returns>
    //    public MenuItem AddMenuItem(string caption, Action<object, EventArgs> even = null, bool enable = true) {
    //        MenuItem item = new MenuItem();
    //        item.Text = caption;
    //        item.Index = mMenuItems.Count;
    //        item.Enabled = enable;
    //        EventHandler handler = even == null ? null : new EventHandler(even);
    //        mClickEvents.Add(handler);

    //        if (even != null) item.Click += handler;

    //        mMenuItems.Add(item);
    //        return item;
    //    }

    //    /// <summary>
    //    /// 從右鍵選單集合中移除指定物件
    //    /// </summary>
    //    /// <param name="item">目標物件</param>
    //    public void RemoveMenuItem(MenuItem item) {
    //        if (!mMenuItems.Contains(item)) return;

    //        int index = mMenuItems.IndexOf(item);

    //        if (mClickEvents[index] != null) item.Click -= mClickEvents[index];
    //    }

    //    /// <summary>
    //    /// 清空右鍵選單集合
    //    /// </summary>
    //    public void Clear() {
    //        for (int i = 0; i < mMenuItems.Count; i++) {
    //            if (mClickEvents[i] != null) mMenuItems[i].Click -= mClickEvents[i];
    //            mMenuItems[i].Dispose();
    //            mMenuItems[i] = null;
    //        }
    //        mMenuItems.Clear();
    //        mMenuItems = null;

    //        mClickEvents.Clear();
    //        mClickEvents = null;
    //    }

    //    #endregion Funciton - Public Methods

    //    #region IDisposable Support
    //    private bool disposedValue = false; // 偵測多餘的呼叫

    //    protected virtual void Dispose(bool disposing) {
    //        if (!disposedValue) {
    //            if (disposing) {
    //                // TODO: 處置 Managed 狀態 (Managed 物件)。
    //                Clear();
    //            }

    //            // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
    //            // TODO: 將大型欄位設為 null。

    //            disposedValue = true;
    //        }
    //    }

    //    // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
    //    // ~MenuItems() {
    //    //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
    //    //   Dispose(false);
    //    // }

    //    // 加入這個程式碼的目的在正確實作可處置的模式。
    //    public void Dispose() {
    //        // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
    //        Dispose(true);
    //        // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
    //        // GC.SuppressFinalize(this);
    //    }
    //    #endregion

    //}

    /// <summary>
    /// 提供框線顏色設定功能之GroupBox
    /// </summary>
    public class CtGroupBox : GroupBox {
        private System.Drawing.Color _BorderColor = System.Drawing.Color.Red;
        [Description("設定或取得外框顏色")]
        public System.Drawing.Color BorderColor {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        protected override void OnPaint(PaintEventArgs e) {
            //取得text字型大小
            Size FontSize = TextRenderer.MeasureText(this.Text,
                                                     this.Font);

            //畫框線
            #region 原始範例
            //Rectangle rec = new Rectangle(e.ClipRectangle.Y,
            //                              this.Font.Height / 2,
            //                              e.ClipRectangle.Width - 1,
            //                              e.ClipRectangle.Height - 1 -
            //                              this.Font.Height / 2);
            //e.ClipRectangle這個值在自動隱藏停靠視窗下似乎是變動
            //因此改用固定的GroupBox.Size
            #endregion 原始範例

            Rectangle rec = new Rectangle(0,
                                          this.Font.Height / 2,
                                          this.Width - 1,
                                          this.Height - 1 -
                                          this.Font.Height / 2);


            e.Graphics.DrawRectangle(new Pen(BorderColor), rec);

            //填滿text的背景
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor),
                new Rectangle(6, 0, FontSize.Width, FontSize.Height));

            //text
            e.Graphics.DrawString(this.Text, this.Font,
                new Pen(this.ForeColor).Brush, 6, 0);
        }
    }

}
