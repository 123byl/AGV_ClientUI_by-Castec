using CtParamEditor.Comm;
using DataGridViewRichTextBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtParamEditor.Core.Internal.Component {

    /// <summary>
    /// 儲存格樣式管理器
    /// </summary>
    public class CtCellStyles : ICellStyles {

        /// <summary>
        /// 一般樣式
        /// </summary>
        public IExFont Regular { get; set; } = new ExFont("微軟正黑體", 12, FontStyle.Regular, Color.Black, Color.Empty);
        /// <summary>
        /// 標記樣式
        /// </summary>
        public IExFont Highlight { get; set; } = new ExFont("微軟正黑體", 12, FontStyle.Bold, Color.Red, Color.Yellow);
        /// <summary>
        /// 必填儲存格樣式
        /// </summary>
        public IExFont RequiredCell { get; set; } = new ExFont("微軟正黑體", 12, FontStyle.Bold, Color.Red, Color.Orange);
        /// <summary>
        /// 已編輯欄位樣式
        /// </summary>
        public IExFont ModifiedRow { get; set; } = new ExFont("微軟正黑體", 12, FontStyle.Italic | FontStyle.Bold, Color.Black, Color.Empty);
        /// <summary>
        /// 已編輯儲存格樣式
        /// </summary>
        public IExFont ModifiedCell { get; set; } = new ExFont("微軟正黑體", 12, FontStyle.Italic | FontStyle.Bold, Color.Red, Color.Empty);

    }
}
