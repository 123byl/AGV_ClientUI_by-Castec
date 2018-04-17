using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtOutLookBar.Public {

    /// <summary>
    /// 滑鼠點擊事件發報者
    /// </summary>
    public interface IClickSender {
        /// <summary>
        /// 滑鼠點擊事件
        /// </summary>
        EventHandler Click { get; set; }
    }

    /// <summary>
    /// 分類控制項
    /// </summary>
    public interface IOutlookCategory:IControl {
        string Text { get; set; }
        Font Font { get; set; }
        OutlookBar Parent { get; set; }
       
        Color BackColor { get; set; }
        int IconSpacing { get; }
        Font ItemFont { get; set; }
        Size ItemSize { get; set; }
        int Margin { get; }
        int RowCount { get; set; }
        IClickSender AddItem(string caption, Image image=null, int enumIdx=-1);
    }

    /// <summary>
    /// 選項控制項
    /// </summary>
    public interface IOutlookItem  :IControl{
        IOutlookCategory Parent { get; set; }
        Point Location { get; set; }
        event EventHandler Click;
        int EnumIdx { get; }
    }

    /// <summary>
    /// 具有<see cref="Control"/>型態之類別
    /// </summary>
    public interface IControl {
        /// <summary>
        /// 回傳<see cref="Control"/>型態值
        /// </summary>
        Control Control { get; }
    }

}
