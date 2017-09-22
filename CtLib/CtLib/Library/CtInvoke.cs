using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CtLib.Module.Ultity;

namespace CtLib.Library {
    /// <summary>
    /// 調用各元件進行 Invoke 方法
    /// <para>用於避免元件因執行緒不同而無法進行操作</para>
    /// </summary>
    /// <remarks>
    /// 關於 Invoke 請參考 <see cref="Control.BeginInvoke(System.Delegate)"/> 與 <seealso cref="Control.BeginInvoke(System.Delegate)"/>
    /// 其中，這部分有大量使用到  委派 <see cref="System.Delegate"/>
    /// </remarks>
    public static class CtInvoke {

        #region Version

        /// <summary>CtInvoke 版本訊息</summary>
        /// <remarks><code>
        /// 0.0.0  Chi Sha [2007/08/15]
        ///     + LibText
        ///     
        /// 1.0.0  Ahern [2014/07/20]
        ///     + 從舊版CtLib搬移
        ///     
        /// 1.0.1  Ahern [2014/09/11]
        ///     + PictureBoxTag
        ///     
        /// 1.0.2  Ahern [2014/09/29]
        ///     + RadioButton 相關
        ///     
        /// 1.1.0  Ahern [2014/12/08]
        ///     - 移除所有Try-Catch，由使用的Function自行去包
        ///     - 移除多餘的 delegate
        ///     
        /// 1.1.1  Ahern [2015/03/12]
        ///     \ DataGridViewAddRow 改以泛型表示
        ///     
        /// 1.1.2  Ahern [2015/05/12]
        ///     + RichTextBoxClear
        ///     + ListBoxSelectedIndex
        ///     \ ListBoxAdd 改以泛型表示
        ///     
        /// 1.1.3  Ahern [2015/05/25]
        ///     \ 部分含有 string[] 與 List&lt;string&gt; 改以 IEnumerable&lt;string&gt; 取代
        ///     
        /// 1.1.4  Ahern [2015/06/02]
        ///     \ 含有 IEnumerable 之方法改以泛型方法 &lt;T&gt; 取代
        ///     
        /// 1.1.5  Ahern [2015/06/03]
        ///     \ 修正 string 視為 IEnumerable 問題
        ///     
        /// </code></remarks>
        public static readonly CtVersion @Version = new CtVersion(1, 1, 5, "2015/06/03", "Ahern Kuo");

        #endregion

        #region Functions - Operations

        #region Button Operations

        private delegate void DlgButtonBool(Button button, bool value);

        /// <summary>調用 <see cref="Button"/> 啟用選項(Enabled)</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ButtonEnable(Button button, bool enabled) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonBool(ButtonEnable), new object[] { button, enabled });
            else button.Enabled = enabled;
        }
        /// <summary>調用 <see cref="Button"/> 可視選項(Visible)</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ButtonVisible(Button button, bool visible) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonBool(ButtonVisible), new object[] { button, visible });
            else button.Visible = visible;
        }

        private delegate void DlgButtonText(Button button, string text);

        /// <summary>調用 <see cref="Button"/> 文字</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="text">欲顯示之文字</param>
        public static void ButtonText(Button button, string text) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonText(ButtonText), new object[] { button, text });
            else button.Text = text;
        }

        private delegate void DlgButtonColor(Button button, Color color);

        /// <summary>調用 <see cref="Button"/> 背景顏色</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="color">欲套用之顏色</param>
        public static void ButtonBackColor(Button button, Color color) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonColor(ButtonBackColor), new object[] { button, color });
            else button.BackColor = color;
        }

        /// <summary>調用 <see cref="Button"/> 前景顏色(Enabled)</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="color">欲套用之顏色</param>
        public static void ButtonForeColor(Button button, Color color) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonColor(ButtonForeColor), new object[] { button, color });
            else button.ForeColor = color;
        }

        private delegate void DlgButtonTag(Button button, object tag);

        /// <summary>調用 <see cref="Button"/> 標籤</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="tag">標籤 (任意物件)</param>
        public static void ButtonTag(Button button, object tag) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonTag(ButtonTag), new object[] { button, tag });
            else button.Tag = tag;
        }

        private delegate void DlgButtonFont(Button button, Font font);

        /// <summary>調用 <see cref="Button"/> 字體</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="font">字體</param>
        public static void ButtonFont(Button button, Font font) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonFont(ButtonFont), new object[] { button, font });
            else button.Font = font;
        }

        private delegate void DlgButtonImage(Button button, Image image);

        /// <summary>調用 <see cref="Button"/> 以更改圖片</summary>
        /// <param name="button">欲調用按鈕</param>
        /// <param name="image">欲更改之圖片</param>
        public static void ButtonImage(Button button, Image image) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonImage(ButtonImage), new object[] { button, image });
            else button.Image = image;
        }

        private delegate void DlgButtonInt(Button button, int value);

        /// <summary>調用 <see cref="Button"/> 以調整控制項寬度</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="width">欲調整之寬度</param>
        public static void ButtonWidth(Button button, int width) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonInt(ButtonWidth), new object[] { button, width });
            else button.Width = width;
        }

        /// <summary>調用 <see cref="Button"/> 以調整控制項高度</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="height">欲調整之高度</param>
        public static void ButtonHeight(Button button, int height) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonInt(ButtonHeight), new object[] { button, height });
            else button.Height = height;
        }

        /// <summary>調用 <see cref="Button"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="left">欲調整之位置</param>
        public static void ButtonLeft(Button button, int left) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonInt(ButtonLeft), new object[] { button, left });
            else button.Left = left;
        }

        /// <summary>調用 <see cref="Button"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="top">欲調整之位置</param>
        public static void ButtonTop(Button button, int top) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonInt(ButtonTop), new object[] { button, top });
            else button.Top = top;
        }

        private delegate void DlgButtonDInt(Button button, int val1, int val2);

        /// <summary>調用 <see cref="Button"/> 以調整控制項大小</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void ButtonSize(Button button, int width, int height) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonDInt(ButtonSize), new object[] { button, width, height });
            else button.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="Button"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void ButtonLocation(Button button, int left, int top) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonDInt(ButtonLocation), new object[] { button, left, top });
            else button.Location = new Point(left, top);
        }

        private delegate void DlgButtonSize(Button button, Size size);

        /// <summary>調用 <see cref="Button"/> 以調整控制項大小</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="size">欲調整之大小</param>
        public static void ButtonSize(Button button, Size size) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonSize(ButtonSize), new object[] { button, size });
            else button.Size = size;
        }

        private delegate void DlgButtonPoint(Button button, Point point);

        /// <summary>調用 <see cref="Button"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="button">欲調用之按鈕</param>
        /// <param name="point">欲調整之位置</param>
        public static void ButtonLocation(Button button, Point point) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButtonPoint(ButtonLocation), new object[] { button, point });
            else button.Location = point;
        }

        private delegate void DlgButton(Button button);

        /// <summary>調用 <see cref="Button"/> 以取得焦點</summary>
        /// <param name="button">欲調用之按鈕</param>
        public static void ButtonFocus(Button button) {
            if (button.InvokeRequired)
                button.BeginInvoke(new DlgButton(ButtonFocus), new object[] { button });
            else button.Focus();
        }
        #endregion

        #region ComboBox Operations

        private delegate void DlgComboBoxBool(ComboBox combobox, bool value);

        /// <summary>調用 <see cref="ComboBox"/> 啟用選項(Enabled)</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ComboBoxEnable(ComboBox combobox, bool enabled) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxBool(ComboBoxEnable), new object[] { combobox, enabled });
            else combobox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ComboBox"/> 可視選項(Visible)</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ComboBoxVisible(ComboBox combobox, bool visible) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxBool(ComboBoxVisible), new object[] { combobox, visible });
            else combobox.Visible = visible;
        }

        private delegate void DlgComboBoxText(ComboBox combobox, string text);

        /// <summary>調用 <see cref="ComboBox"/> 文字</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="text">欲顯示之文字</param>
        public static void ComboBoxText(ComboBox combobox, string text) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxText(ComboBoxText), new object[] { combobox, text });
            else combobox.Text = text;
        }

        private delegate void DlgComboBoxIns(ComboBox combobox, int index, object value);

        /// <summary>調用 <see cref="ComboBox"/> 以插入選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="index">欲插入之位置索引</param>
        /// <param name="value">欲插入之物件</param>
        public static void ComboBoxInsert(ComboBox combobox, int index, object value) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxIns(ComboBoxInsert), new object[] { combobox, index, value });
            else combobox.Items.Insert(index, value);
        }

        private delegate void DlgComboBoxObject(ComboBox combobox, object value);

        /// <summary>調用 <see cref="ComboBox"/> 以增加選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="value">欲新增之物件</param>
        public static void ComboBoxAdd(ComboBox combobox, object value) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxObject(ComboBoxAdd), new object[] { combobox, value });
            else combobox.Items.Add(value);
        }

        private delegate void DlgComboBoxAddList<TObj>(ComboBox combobox, IEnumerable<TObj> value);

        /// <summary>調用 <see cref="ComboBox"/> 以增加多個選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="value">欲新增之物件</param>
        public static void ComboBoxAdd<TValue>(ComboBox combobox, IEnumerable<TValue> value) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxAddList<TValue>(ComboBoxAdd), new object[] { combobox, value });
            else if (value is string) combobox.Items.Add(value);
            else combobox.Items.AddRange(value.Cast<object>().ToArray());
        }

        /// <summary>調用 <see cref="ComboBox"/> 以刪除選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="value">欲刪除之物件</param>
        public static void ComboBoxRemove(ComboBox combobox, object value) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxObject(ComboBoxRemove), new object[] { combobox, value });
            else combobox.Items.Remove(value);
        }

        private delegate void DlgComboBoxInt(ComboBox combobox, int index);

        /// <summary>調用 <see cref="ComboBox"/> 以刪除特定索引選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="index">欲刪除之索引</param>
        public static void ComboBoxRemove(ComboBox combobox, int index) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxInt(ComboBoxRemove), new object[] { combobox, index });
            else combobox.Items.RemoveAt(index);
        }

        private delegate void DlgComboBoxClear(ComboBox combobox);

        /// <summary>調用 <see cref="ComboBox"/> 以清空所有選項</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        public static void ComboBoxClear(ComboBox combobox) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxClear(ComboBoxClear), new object[] { combobox });
            else combobox.Items.Clear();
        }

        /// <summary>調用 <see cref="ComboBox"/> 以取得焦點</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        public static void ComboBoxFocus(ComboBox combobox) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxClear(ComboBoxFocus), new object[] { combobox });
            else combobox.Focus();
        }

        /// <summary>調用 <see cref="ComboBox"/> 以更改目前選取項目</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="index">欲更改之選項索引</param>
        public static void ComboBoxSelectedIndex(ComboBox combobox, int index) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxInt(ComboBoxSelectedIndex), new object[] { combobox, index });
            else combobox.SelectedIndex = index;
        }

        /// <summary>調用 <see cref="ComboBox"/> 以更改目前選取項目</summary>
        /// <param name="combobox">欲調用之ComboBox</param>
        /// <param name="item">欲更改之選項物件。此物件必須已存在於集合(Items)中</param>
        public static void ComboBoxSelectedItem(ComboBox combobox, object item) {
            if (combobox.InvokeRequired)
                combobox.BeginInvoke(new DlgComboBoxObject(ComboBoxSelectedItem), new object[] { combobox, item });
            else combobox.SelectedItem = item;
        }

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項寬度</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="width">欲調整之寬度</param>
        public static void ComboBoxWidth(ComboBox comboBox, int width) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxInt(ComboBoxWidth), new object[] { comboBox, width });
            else comboBox.Width = width;
        }

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項高度</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="height">欲調整之高度</param>
        public static void ComboBoxHeight(ComboBox comboBox, int height) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxInt(ComboBoxHeight), new object[] { comboBox, height });
            else comboBox.Height = height;
        }

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="left">欲調整之位置</param>
        public static void ComboBoxLeft(ComboBox comboBox, int left) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxInt(ComboBoxLeft), new object[] { comboBox, left });
            else comboBox.Left = left;
        }

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="top">欲調整之位置</param>
        public static void ComboBoxTop(ComboBox comboBox, int top) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxInt(ComboBoxTop), new object[] { comboBox, top });
            else comboBox.Top = top;
        }

        private delegate void DlgComboBoxDInt(ComboBox comboBox, int val1, int val2);

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項大小</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void ComboBoxSize(ComboBox comboBox, int width, int height) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxDInt(ComboBoxSize), new object[] { comboBox, width, height });
            else comboBox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void ComboBoxLocation(ComboBox comboBox, int left, int top) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxDInt(ComboBoxLocation), new object[] { comboBox, left, top });
            else comboBox.Location = new Point(left, top);
        }

        private delegate void DlgComboBoxSize(ComboBox comboBox, Size size);

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項大小</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="size">欲調整之大小</param>
        public static void ComboBoxSize(ComboBox comboBox, Size size) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxSize(ComboBoxSize), new object[] { comboBox, size });
            else comboBox.Size = size;
        }

        private delegate void DlgComboBoxPoint(ComboBox comboBox, Point point);

        /// <summary>調用 <see cref="ComboBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="comboBox">欲調用之 ComboBox</param>
        /// <param name="point">欲調整之位置</param>
        public static void ComboBoxLocation(ComboBox comboBox, Point point) {
            if (comboBox.InvokeRequired)
                comboBox.BeginInvoke(new DlgComboBoxPoint(ComboBoxLocation), new object[] { comboBox, point });
            else comboBox.Location = point;
        }

        #endregion

        #region CheckBox Operations

        private delegate void DlgCheckBoxBool(CheckBox chkbox, bool value);

        /// <summary>調用 <see cref="CheckBox"/> 啟用選項(Enabled)</summary>
        /// <param name="chkbox">欲調用之CheckBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void CheckBoxEnable(CheckBox chkbox, bool enabled) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxBool(CheckBoxEnable), new object[] { chkbox, enabled });
            else chkbox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="CheckBox"/> 可視選項(Visible)</summary>
        /// <param name="chkbox">欲調用之CheckBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void CheckBoxVisible(CheckBox chkbox, bool visible) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxBool(CheckBoxVisible), new object[] { chkbox, visible });
            else chkbox.Visible = visible;
        }

        private delegate void DlgCheckBoxColor(CheckBox chkbox, Color color);

        /// <summary>調用 <see cref="CheckBox"/> 以更改背景顏色</summary>
        /// <param name="chkbox">欲調用之CheckBox</param>
        /// <param name="color">欲更改之背景顏色</param>
        public static void CheckBoxBackColor(CheckBox chkbox, Color color) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxColor(CheckBoxBackColor), new object[] { chkbox, color });
            else chkbox.BackColor = color;
        }

        /// <summary>調用 <see cref="CheckBox"/> 以更改前景顏色</summary>
        /// <param name="chkbox">欲調用之CheckBox</param>
        /// <param name="color">欲更改之顏色</param>
        public static void CheckBoxForeColor(CheckBox chkbox, Color color) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxColor(CheckBoxForeColor), new object[] { chkbox, color });
            else chkbox.ForeColor = color;
        }

        /// <summary>調用 <see cref="CheckBox"/> 以更改勾選狀態</summary>
        /// <param name="chkbox">欲調用之CheckBox</param>
        /// <param name="value">欲更改之樣式   (True)勾選 (False)取消勾選</param>
        public static void CheckBoxChecked(CheckBox chkbox, bool value) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxBool(CheckBoxChecked), new object[] { chkbox, value });
            else chkbox.Checked = value;
        }

        private delegate void DlgCheckBoxInt(CheckBox chkbox, int value);

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項寬度</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="width">欲調整之寬度</param>
        public static void CheckBoxWidth(CheckBox chkbox, int width) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxInt(CheckBoxWidth), new object[] { chkbox, width });
            else chkbox.Width = width;
        }

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項高度</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="height">欲調整之高度</param>
        public static void CheckBoxHeight(CheckBox chkbox, int height) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxInt(CheckBoxHeight), new object[] { chkbox, height });
            else chkbox.Height = height;
        }

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="left">欲調整之位置</param>
        public static void CheckBoxLeft(CheckBox chkbox, int left) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxInt(CheckBoxLeft), new object[] { chkbox, left });
            else chkbox.Left = left;
        }

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="top">欲調整之位置</param>
        public static void CheckBoxTop(CheckBox chkbox, int top) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxInt(CheckBoxTop), new object[] { chkbox, top });
            else chkbox.Top = top;
        }

        private delegate void DlgCheckBoxDInt(CheckBox chkbox, int val1, int val2);

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項大小</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void CheckBoxSize(CheckBox chkbox, int width, int height) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxDInt(CheckBoxSize), new object[] { chkbox, width, height });
            else chkbox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void CheckBoxLocation(CheckBox chkbox, int left, int top) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxDInt(CheckBoxLocation), new object[] { chkbox, left, top });
            else chkbox.Location = new Point(left, top);
        }

        private delegate void DlgCheckBoxSize(CheckBox chkbox, Size size);

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項大小</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="size">欲調整之大小</param>
        public static void CheckBoxSize(CheckBox chkbox, Size size) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxSize(CheckBoxSize), new object[] { chkbox, size });
            else chkbox.Size = size;
        }

        private delegate void DlgCheckBoxPoint(CheckBox chkbox, Point point);

        /// <summary>調用 <see cref="CheckBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        /// <param name="point">欲調整之位置</param>
        public static void CheckBoxLocation(CheckBox chkbox, Point point) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBoxPoint(CheckBoxLocation), new object[] { chkbox, point });
            else chkbox.Location = point;
        }

        private delegate void DlgCheckBox(CheckBox chkbox);

        /// <summary>調用 <see cref="CheckBox"/> 以取得焦點</summary>
        /// <param name="chkbox">欲調用之 CheckBox</param>
        public static void CheckBoxFocus(CheckBox chkbox) {
            if (chkbox.InvokeRequired)
                chkbox.BeginInvoke(new DlgCheckBox(CheckBoxFocus), new object[] { chkbox });
            else chkbox.Focus();
        }
        #endregion

        #region DataGridView Operations

        private delegate void DlgDataGridViewBool(DataGridView dgv, bool value);

        /// <summary>調用 <see cref="DataGridView"/> 啟用選項(Enabled)</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void DataGridViewEnable(DataGridView dgv, bool enabled) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewBool(DataGridViewEnable), new object[] { dgv, enabled });
            else dgv.Enabled = enabled;
        }

        /// <summary>調用 <see cref="DataGridView"/> 可視選項(Visible)</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void DataGridViewVisible(DataGridView dgv, bool visible) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewBool(DataGridViewVisible), new object[] { dgv, visible });
            else dgv.Visible = visible;
        }

        private delegate void DlgDataGridViewCellData(DataGridView dgv, int row, int col, DataGridViewCell cell);

        /// <summary>>調用 <see cref="DataGridView"/> 以更改特定欄位數值</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="row">欲更改欄位之列數</param>
        /// <param name="col">欲更改欄位之行數</param>
        /// <param name="cell">資料格內容</param>
        public static void DataGridViewCellData(DataGridView dgv, int row, int col, DataGridViewCell cell) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewCellData(DataGridViewCellData), new object[] { dgv, row, col, cell });
            else dgv.Rows[row].Cells[col] = cell;
        }

        private delegate void DlgDataGridViewCellDataObj(DataGridView dgv, int row, int col, object value);

        /// <summary>>調用 <see cref="DataGridView"/> 以更改特定欄位數值</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="row">欲更改欄位之列數</param>
        /// <param name="col">欲更改欄位之行數</param>
        /// <param name="value">資料格內容</param>
        public static void DataGridViewCellData(DataGridView dgv, int row, int col, object value) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewCellDataObj(DataGridViewCellData), new object[] { dgv, row, col, value });
            else dgv.Rows[row].Cells[col].Value = value;
        }

        private delegate void DlgDataGridViewAddRows<TObj>(DataGridView dgv, IEnumerable<TObj> value, bool top, bool delete);

        /// <summary>調用 <see cref="DataGridView"/> 以增加一新列</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="value">各欄位數值</param>
        /// <param name="top">是否加入於最上方?  (True)於最上方加入 (False)於最後面新增一列</param>
        /// <param name="delete">如總列數大於100是否先刪除最後一列再新增?  (True)刪除最後列，避免過長 (False)直接新增</param>
        public static void DataGridViewAddRow<TValue>(DataGridView dgv, IEnumerable<TValue> value, bool top = true, bool delete = true) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewAddRows<TValue>(DataGridViewAddRow), new object[] { dgv, value, top, delete });
            else {

                /*-- 資料太多則先刪除最後一列 --*/
                if (delete && (dgv.Rows.Count > 100))
                    dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

                /*-- 組合資料 --*/
                int idx = 0;
                DataGridViewRow dgvRow = new DataGridViewRow();
                dgvRow.CreateCells(dgv);
                foreach (TValue item in value) {
                    dgvRow.Cells[idx].Value = item;
                    idx++;
                }

                /*-- 根據top屬性來插在最上面或最後 --*/
                if (top) dgv.Rows.Insert(0, dgvRow);
                else dgv.Rows.Add(dgvRow);

                dgv.ClearSelection();   //清除選取格數
            }
        }

        private delegate void DlgDataGridViewAddRowsColor<TObj>(DataGridView dgv, IEnumerable<TObj> value, Color color, bool top, bool delete);

        /// <summary>調用 <see cref="DataGridView"/> 以增加一新列</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="value">各欄位數值</param>
        /// <param name="color">新增列之背景顏色</param>
        /// <param name="top">是否加入於最上方?  (True)於最上方加入 (False)於最後面新增一列</param>
        /// <param name="delete">如總列數大於200是否先刪除最後一列再新增?  (True)刪除最後列，避免過長 (False)直接新增</param>
        public static void DataGridViewAddRow<TValue>(DataGridView dgv, IEnumerable<TValue> value, Color color, bool top = true, bool delete = true) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewAddRowsColor<TValue>(DataGridViewAddRow), new object[] { dgv, value, color, top, delete });
            else {

                /*-- 資料太多則先刪除最後一列 --*/
                if (delete && (dgv.Rows.Count > 200))
                    dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

                /*-- 組合資料 --*/
                int idx = 0;
                DataGridViewRow dgvRow = new DataGridViewRow();
                dgvRow.CreateCells(dgv);
                foreach (TValue item in value) {
                    dgvRow.Cells[idx].Value = item;
                    idx++;
                }

                /*-- 根據top屬性來插在最上面或最後 --*/
                if (top) {
                    dgv.Rows.Insert(0, dgvRow);
                    dgv.Rows[0].DefaultCellStyle.BackColor = color;
                } else {
                    dgv.Rows.Add(dgvRow);
                    dgv.Rows[dgv.Rows.Count - 1].DefaultCellStyle.BackColor = color;
                }
                dgv.ClearSelection();   //清除選取格數
            }
        }

        private delegate void DlgDataGridViewAddRow(DataGridView dgv, object value, bool top, bool delete);

        /// <summary>調用 <see cref="DataGridView"/> 以增加一新列</summary>
        /// <param name="dgv">欲調用之DataGridView</param>
        /// <param name="value">各欄位數值</param>
        /// <param name="top">是否加入於最上方?  (True)於最上方加入 (False)於最後面新增一列</param>
        /// <param name="delete">如總列數大於200是否先刪除最後一列再新增?  (True)刪除最後列，避免過長 (False)直接新增</param>
        public static void DataGridViewAddRow(DataGridView dgv, object value, bool top = true, bool delete = true) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewAddRow(DataGridViewAddRow), new object[] { dgv, value, top, delete });
            else {

                /*-- 資料太多則先刪除最後一列 --*/
                if (delete && (dgv.Rows.Count > 200))
                    dgv.Rows.RemoveAt(dgv.Rows.Count - 1);

                /*-- 組合資料 --*/
                DataGridViewRow dgvRow = new DataGridViewRow();
                dgvRow.CreateCells(dgv);
                dgvRow.Cells[0].Value = value;

                /*-- 根據top屬性來插在最上面或最後 --*/
                if (top) dgv.Rows.Insert(0, dgvRow);
                else dgv.Rows.Add(dgvRow);

                dgv.ClearSelection();   //清除選取格數
            }
        }

        private delegate void DlgDataGridViewClear(DataGridView dgv);

        /// <summary>調用 <see cref="DataGridView"/> 以清除目前所有資料</summary>
        public static void DataGridViewClear(DataGridView dgv) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewClear(DataGridViewClear), new object[] { dgv });
            else dgv.Rows.Clear();
        }

        private delegate void DlgDataGridViewInt(DataGridView dgv, int value);

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項寬度</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="width">欲調整之寬度</param>
        public static void DataGridViewWidth(DataGridView dgv, int width) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewInt(DataGridViewWidth), new object[] { dgv, width });
            else dgv.Width = width;
        }

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項高度</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="height">欲調整之高度</param>
        public static void DataGridViewHeight(DataGridView dgv, int height) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewInt(DataGridViewHeight), new object[] { dgv, height });
            else dgv.Height = height;
        }

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="left">欲調整之位置</param>
        public static void DataGridViewLeft(DataGridView dgv, int left) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewInt(DataGridViewLeft), new object[] { dgv, left });
            else dgv.Left = left;
        }

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="top">欲調整之位置</param>
        public static void DataGridViewTop(DataGridView dgv, int top) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewInt(DataGridViewTop), new object[] { dgv, top });
            else dgv.Top = top;
        }

        private delegate void DlgDataGridViewDInt(DataGridView dgv, int val1, int val2);

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項大小</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void DataGridViewSize(DataGridView dgv, int width, int height) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewDInt(DataGridViewSize), new object[] { dgv, width, height });
            else dgv.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void DataGridViewLocation(DataGridView dgv, int left, int top) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewDInt(DataGridViewLocation), new object[] { dgv, left, top });
            else dgv.Location = new Point(left, top);
        }

        private delegate void DlgDataGridViewSize(DataGridView dgv, Size size);

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項大小</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="size">欲調整之大小</param>
        public static void DataGridViewSize(DataGridView dgv, Size size) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewSize(DataGridViewSize), new object[] { dgv, size });
            else dgv.Size = size;
        }

        private delegate void DlgDataGridViewPoint(DataGridView dgv, Point point);

        /// <summary>調用 <see cref="DataGridView"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="dgv">欲調用之 DataGridView</param>
        /// <param name="point">欲調整之位置</param>
        public static void DataGridViewLocation(DataGridView dgv, Point point) {
            if (dgv.InvokeRequired)
                dgv.BeginInvoke(new DlgDataGridViewPoint(DataGridViewLocation), new object[] { dgv, point });
            else dgv.Location = point;
        }
        #endregion

        #region DateTimePicker Operations

        private delegate void DlgDateTimePickerBool(DateTimePicker timePicker, bool value);

        /// <summary>調用 <see cref="DateTimePicker"/> 啟用選項(Enabled)</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void DateTimePickerEnable(DateTimePicker timePicker, bool enabled) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerBool(DateTimePickerEnable), new object[] { timePicker, enabled });
            else timePicker.Enabled = enabled;
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 可視選項</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="visible">可視?  (True)可見 (False)不可見</param>
        public static void DateTimePickerVisible(DateTimePicker timePicker, bool visible) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerBool(DateTimePickerVisible), new object[] { timePicker, visible });
            else timePicker.Visible = visible;
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 勾選狀態</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="check">勾選?  (True)勾選 (False)取消勾選</param>
        public static void DateTimePickerChecked(DateTimePicker timePicker, bool check) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerBool(DateTimePickerChecked), new object[] { timePicker, check });
            else timePicker.Checked = check;
        }

        private delegate void DlgDateTimePickerValue(DateTimePicker timePicker, DateTime value);

        /// <summary>調用 <see cref="DateTimePicker"/> 以更改當前日期</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="value">欲更改之日期</param>
        public static void DateTimePickerValue(DateTimePicker timePicker, DateTime value) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerValue(DateTimePickerValue), new object[] { timePicker, value });
            else timePicker.Value = value;
        }

        private delegate void DlgDateTimePickerInt(DateTimePicker timePicker, int value);

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項寬度</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="width">欲調整之寬度</param>
        public static void DateTimePickerWidth(DateTimePicker timePicker, int width) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerInt(DateTimePickerWidth), new object[] { timePicker, width });
            else timePicker.Width = width;
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項高度</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="height">欲調整之高度</param>
        public static void DateTimePickerHeight(DateTimePicker timePicker, int height) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerInt(DateTimePickerHeight), new object[] { timePicker, height });
            else timePicker.Height = height;
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="left">欲調整之位置</param>
        public static void DateTimePickerLeft(DateTimePicker timePicker, int left) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerInt(DateTimePickerLeft), new object[] { timePicker, left });
            else timePicker.Left = left;
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="top">欲調整之位置</param>
        public static void DateTimePickerTop(DateTimePicker timePicker, int top) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerInt(DateTimePickerTop), new object[] { timePicker, top });
            else timePicker.Top = top;
        }

        private delegate void DlgDateTimePickerDInt(DateTimePicker timePicker, int val1, int val2);

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項大小</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void DateTimePickerSize(DateTimePicker timePicker, int width, int height) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerDInt(DateTimePickerSize), new object[] { timePicker, width, height });
            else timePicker.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void DateTimePickerLocation(DateTimePicker timePicker, int left, int top) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerDInt(DateTimePickerLocation), new object[] { timePicker, left, top });
            else timePicker.Location = new Point(left, top);
        }

        private delegate void DlgDateTimePickerSize(DateTimePicker timePicker, Size size);

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項大小</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="size">欲調整之大小</param>
        public static void DateTimePickerSize(DateTimePicker timePicker, Size size) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerSize(DateTimePickerSize), new object[] { timePicker, size });
            else timePicker.Size = size;
        }

        private delegate void DlgDateTimePickerPoint(DateTimePicker timePicker, Point point);

        /// <summary>調用 <see cref="DateTimePicker"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="timePicker">欲調用之 DateTimePicker</param>
        /// <param name="point">欲調整之位置</param>
        public static void DateTimePickerLocation(DateTimePicker timePicker, Point point) {
            if (timePicker.InvokeRequired)
                timePicker.BeginInvoke(new DlgDateTimePickerPoint(DateTimePickerLocation), new object[] { timePicker, point });
            else timePicker.Location = point;
        }
        #endregion

        #region Form Operations

        private delegate void DlgFormBool(Form form, bool value);

        /// <summary>調用 <see cref="Form"/> 啟用選項(Enabled)</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void FormEnable(Form form, bool enabled) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormBool(FormEnable), new object[] { form, enabled });
            else form.Enabled = enabled;
        }

        /// <summary>調用 <see cref="Form"/> 可視選項(Visible)</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void FormVisible(Form form, bool visible) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormBool(FormVisible), new object[] { form, visible });
            else {
                if (visible) form.Show();
                else form.Hide();
            }
        }

        private delegate void DlgFormString(Form form, string str);
        /// <summary>調用 <see cref="Form"/> 以更改標題</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="text">欲顯示之文字</param>
        public static void FormText(Form form, string text) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormString(FormText), new object[] { form, text });
            else form.Text = text;
        }

        /// <summary>調用 <see cref="Form"/> 以更改名稱</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="text">欲顯示之名稱</param>
        public static void FormName(Form form, string text) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormString(FormText), new object[] { form, text });
            else form.Name = text;
        }

        private delegate void DlgFormBgColor(Form form, Color color);

        /// <summary>調用 <see cref="Form"/> 背景顏色</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="color">欲套用之顏色</param>
        public static void FormBackColor(Form form, Color color) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormBgColor(FormBackColor), new object[] { form, color });
            else form.BackColor = color;
        }

        /// <summary>調用 <see cref="Form"/> 以關閉表單</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="owner">是否連擁有者都一起關閉?  (True)一起關閉 (False)單純關閉此表單</param>
        public static void FormClose(Form form, bool owner = false) {
            if (owner) {
                if (form.Owner.InvokeRequired)
                    form.Owner.BeginInvoke(new DlgFormBool(FormClose), new object[] { form, owner });
                else form.Owner.Close();
            } else {
                if (form.InvokeRequired)
                    form.BeginInvoke(new DlgFormBool(FormClose), new object[] { form, owner });
                else form.Close();
            }
        }

        private delegate void DlgFormInt(Form form, int value);

        /// <summary>調用 <see cref="Form"/> 以更改介面寬度(Width)</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="width">欲調整之寬度</param>
        public static void FormWidth(Form form, int width) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormInt(FormWidth), new object[] { form, width });
            else form.Width = width;
        }

        /// <summary>調用 <see cref="Form"/> 以更改介面高度(Height)</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="height">欲調整之高度</param>
        public static void FormHeight(Form form, int height) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormInt(FormHeight), new object[] { form, height });
            else form.Height = height;
        }

        /// <summary>調用 <see cref="Form"/> 以更改左邊距離螢幕邊緣位置</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="left">欲調整之位置</param>
        public static void FormLeft(Form form, int left) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormInt(FormLeft), new object[] { form, left });
            else form.Left = left;
        }

        /// <summary>調用 <see cref="Form"/> 以更改上緣距離螢幕邊緣位置</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="top">欲調整之位置</param>
        public static void FormTop(Form form, int top) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormInt(FormTop), new object[] { form, top });
            else form.Top = top;
        }

        private delegate void DlgFormDInt(Form form, int val1, int val2);

        /// <summary>調用 <see cref="Form"/> 以更改視窗大小</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="width">欲更改的寬度</param>
        /// <param name="height">欲更改的高度</param>
        public static void FormSize(Form form, int width, int height) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormDInt(FormSize), new object[] { form, width, height });
            else form.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="Form"/> 以更改位置，預設錨點(Anchor)在左上角</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="left">距左邊緣之距離</param>
        /// <param name="top">距上緣之距離</param>
        public static void FormLocation(Form form, int left, int top) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormDInt(FormLocation), new object[] { form, left, top });
            else form.Location = new Point(left, top);
        }

        private delegate void DlgFormPoint(Form form, Point point);

        /// <summary>調用 <see cref="Form"/> 以更改位置，預設錨點(Anchor)為左上角</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="loc">欲調整之位置</param>
        public static void FormLocation(Form form, Point loc) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormPoint(FormLocation), new object[] { form, loc });
            else form.Location = loc;
        }

        private delegate void DlgFormSize(Form form, Size size);
        /// <summary>調用 <see cref="Form"/> 以更改介面大小</summary>
        /// <param name="form">欲調用之表單</param>
        /// <param name="size">欲調整之大小</param>
        public static void FormSize(Form form, Size size) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgFormSize(FormSize), new object[] { form, size });
            else form.Size = size;
        }

        private delegate void DlgForm(Form form);
        /// <summary>調用 <see cref="Form"/> 以取得焦點</summary>
        /// <param name="form">欲調用之表單</param>
        public static void FormFocus(Form form) {
            if (form.InvokeRequired)
                form.BeginInvoke(new DlgForm(FormFocus), new object[] { form });
            else form.Focus();
        }
        #endregion

        #region GroupBox Operations

        private delegate void DlgGroupBoxBool(GroupBox groupbox, bool value);

        /// <summary>調用 <see cref="GroupBox"/> 啟用選項(Enabled)</summary>
        /// <param name="groupbox">欲調用之GroupBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void GroupBoxEnable(GroupBox groupbox, bool enabled) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxBool(GroupBoxEnable), new object[] { groupbox, enabled });
            else groupbox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="GroupBox"/> 可視選項(Visible)</summary>
        /// <param name="groupbox">欲調用之GroupBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void GroupBoxVisible(GroupBox groupbox, bool visible) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxBool(GroupBoxVisible), new object[] { groupbox, visible });
            else groupbox.Visible = visible;
        }

        private delegate void DlgGroupBoxText(GroupBox groupbox, string text);

        /// <summary>調用 <see cref="GroupBox"/> 文字</summary>
        /// <param name="groupbox">欲調用之GroupBox</param>
        /// <param name="text">欲顯示之文字</param>
        public static void GroupBoxText(GroupBox groupbox, string text) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxText(GroupBoxText), new object[] { groupbox, text });
            else groupbox.Text = text;
        }

        private delegate void DlgGroupBoxInt(GroupBox groupbox, int value);

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項寬度</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="width">欲調整之寬度</param>
        public static void GroupBoxWidth(GroupBox groupbox, int width) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxInt(GroupBoxWidth), new object[] { groupbox, width });
            else groupbox.Width = width;
        }

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項高度</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="height">欲調整之高度</param>
        public static void GroupBoxHeight(GroupBox groupbox, int height) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxInt(GroupBoxHeight), new object[] { groupbox, height });
            else groupbox.Height = height;
        }

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="left">欲調整之位置</param>
        public static void GroupBoxLeft(GroupBox groupbox, int left) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxInt(GroupBoxLeft), new object[] { groupbox, left });
            else groupbox.Left = left;
        }

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="top">欲調整之位置</param>
        public static void GroupBoxTop(GroupBox groupbox, int top) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxInt(GroupBoxTop), new object[] { groupbox, top });
            else groupbox.Top = top;
        }

        private delegate void DlgGroupBoxDInt(GroupBox groupbox, int val1, int val2);

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項大小</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void GroupBoxSize(GroupBox groupbox, int width, int height) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxDInt(GroupBoxSize), new object[] { groupbox, width, height });
            else groupbox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void GroupBoxLocation(GroupBox groupbox, int left, int top) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxDInt(GroupBoxLocation), new object[] { groupbox, left, top });
            else groupbox.Location = new Point(left, top);
        }

        private delegate void DlgGroupBoxSize(GroupBox groupbox, Size size);

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項大小</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="size">欲調整之大小</param>
        public static void GroupBoxSize(GroupBox groupbox, Size size) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxSize(GroupBoxSize), new object[] { groupbox, size });
            else groupbox.Size = size;
        }

        private delegate void DlgGroupBoxPoint(GroupBox groupbox, Point point);

        /// <summary>調用 <see cref="GroupBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        /// <param name="point">欲調整之位置</param>
        public static void GroupBoxLocation(GroupBox groupbox, Point point) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBoxPoint(GroupBoxLocation), new object[] { groupbox, point });
            else groupbox.Location = point;
        }

        private delegate void DlgGroupBox(GroupBox groupbox);

        /// <summary>調用 <see cref="GroupBox"/> 以取得焦點</summary>
        /// <param name="groupbox">欲調用之 GroupBox</param>
        public static void GroupBoxFocus(GroupBox groupbox) {
            if (groupbox.InvokeRequired)
                groupbox.BeginInvoke(new DlgGroupBox(GroupBoxFocus), new object[] { groupbox });
            else groupbox.Focus();
        }
        #endregion

        #region Label Operations

        private delegate void DlgLabelBool(Label label, bool value);

        /// <summary>調用 <see cref="Label"/> 啟用選項(Enabled)</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void LabelEnable(Label label, bool enabled) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelBool(LabelEnable), new object[] { label, enabled });
            else label.Enabled = enabled;
        }

        /// <summary>調用 <see cref="Label"/> 可視選項(Visible)</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void LabelVisible(Label label, bool visible) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelBool(LabelVisible), new object[] { label, visible });
            else label.Visible = visible;
        }

        private delegate void DlgLabelText(Label label, string text);

        /// <summary>調用 <see cref="Label"/> 文字</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="text">欲顯示之文字</param>
        public static void LabelText(Label label, string text) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelText(LabelText), new object[] { label, text });
            else label.Text = text;
        }

        private delegate void DlgLabelTextList(Label label, IEnumerable<string> text);

        /// <summary>調用 <see cref="Label"/> 多行文字</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="text">欲顯示之文字</param>
        public static void LabelText(Label label, IEnumerable<string> text) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelTextList(LabelText), new object[] { label, text });
            else {
                label.Text = string.Join(CtConst.NewLine, text.ToArray());
            }
        }

        private delegate void DlgLabelColor(Label label, Color color);

        /// <summary>調用 <see cref="Label"/> 背景顏色</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="color">欲套用之顏色</param>
        public static void LabelBackColor(Label label, Color color) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelColor(LabelBackColor), new object[] { label, color });
            else label.BackColor = color;
        }

        /// <summary>調用 <see cref="Label"/> 前景顏色(Enabled)</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="color">欲套用之顏色</param>
        public static void LabelForeColor(Label label, Color color) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelColor(LabelForeColor), new object[] { label, color });
            else label.ForeColor = color;
        }

        private delegate void DlgLabelTag(Label label, object tag);

        /// <summary>調用 <see cref="Label"/> 標籤</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="tag">標籤 (任意物件)</param>

        public static void LabelTag(Label label, object tag) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelTag(LabelTag), new object[] { label, tag });
            else label.Tag = tag;
        }

        private delegate void DlgLabelFont(Label label, Font font);

        /// <summary>調用 <see cref="Label"/> 字體</summary>
        /// <param name="label">欲調用之Label</param>
        /// <param name="font">字體</param>
        public static void LabelFont(Label label, Font font) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelFont(LabelFont), new object[] { label, font });
            else label.Font = font;
        }

        private delegate void DlgLabelFocus(Label label);

        /// <summary>調用 <see cref="Label"/> 以取得該物件焦點</summary>
        /// <param name="label">欲調用之 Label</param>
        public static void LabelFocus(Label label) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelFocus(LabelFocus), new object[] { label });
            else label.Focus();
        }

        private delegate void DlgLabelInt(Label label, int value);

        /// <summary>調用 <see cref="Label"/> 以調整控制項寬度</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void LabelWidth(Label label, int width) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelInt(LabelWidth), new object[] { label, width });
            else label.Width = width;
        }

        /// <summary>調用 <see cref="Label"/> 以調整控制項高度</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="height">欲調整之高度</param>
        public static void LabelHeight(Label label, int height) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelInt(LabelHeight), new object[] { label, height });
            else label.Height = height;
        }

        /// <summary>調用 <see cref="Label"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="left">欲調整之位置</param>
        public static void LabelLeft(Label label, int left) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelInt(LabelLeft), new object[] { label, left });
            else label.Left = left;
        }

        /// <summary>調用 <see cref="Label"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="top">欲調整之位置</param>
        public static void LabelTop(Label label, int top) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelInt(LabelTop), new object[] { label, top });
            else label.Top = top;
        }

        private delegate void DlgLabelDInt(Label label, int val1, int val2);

        /// <summary>調用 <see cref="Label"/> 以調整控制項大小</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void LabelSize(Label label, int width, int height) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelDInt(LabelSize), new object[] { label, width, height });
            else label.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="Label"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void LabelLocation(Label label, int left, int top) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelDInt(LabelLocation), new object[] { label, left, top });
            else label.Location = new Point(left, top);
        }

        private delegate void DlgLabelSize(Label label, Size size);

        /// <summary>調用 <see cref="Label"/> 以調整控制項大小</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="size">欲調整之大小</param>
        public static void LabelSize(Label label, Size size) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelSize(LabelSize), new object[] { label, size });
            else label.Size = size;
        }

        private delegate void DlgLabelPoint(Label label, Point point);

        /// <summary>調用 <see cref="Label"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="point">欲調整之位置</param>
        public static void LabelLocation(Label label, Point point) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelPoint(LabelLocation), new object[] { label, point });
            else label.Location = point;
        }

        private delegate void DlgLabelCursor(Label label, Cursor cursor);

        /// <summary>調用 <see cref="Label"/> 以調整滑鼠指標樣式</summary>
        /// <param name="label">欲調用之 文字標籤(Label)</param>
        /// <param name="cursor">欲套用之滑鼠指標</param>
        public static void LabelCursor(Label label, Cursor cursor) {
            if (label.InvokeRequired)
                label.BeginInvoke(new DlgLabelCursor(LabelCursor), new object[] { label, cursor });
            else label.Cursor = cursor;
        }
        #endregion

        #region ListBox Operations

        private delegate void DlgListBoxBool(ListBox listbox, bool value);

        /// <summary>調用 <see cref="ListBox"/> 啟用選項(Enabled)</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ListBoxEnable(ListBox listbox, bool enabled) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxBool(ListBoxEnable), new object[] { listbox, enabled });
            else listbox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ListBox"/> 可視選項(Visible)</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ListBoxVisible(ListBox listbox, bool visible) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxBool(ListBoxVisible), new object[] { listbox, visible });
            else listbox.Visible = visible;
        }

        private delegate void DlgListBoxIns(ListBox listbox, int index, object value);

        /// <summary>調用 <see cref="ListBox"/> 以插入選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="index">欲插入之位置索引</param>
        /// <param name="value">欲插入之物件</param>
        public static void ListBoxInsert(ListBox listbox, int index, object value) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxIns(ListBoxInsert), new object[] { listbox, index, value });
            else listbox.Items.Insert(index, value);
        }

        private delegate void DlgListBoxObject(ListBox listbox, object value);

        /// <summary>調用 <see cref="ListBox"/> 以增加選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="value">欲新增之物件</param>
        public static void ListBoxAdd(ListBox listbox, object value) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxObject(ListBoxAdd), new object[] { listbox, value });
            else listbox.Items.Add(value);
        }

        private delegate void DlgListBoxAddList<TObj>(ListBox listbox, IEnumerable<TObj> value);

        /// <summary>調用 <see cref="ListBox"/> 以增加選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="value">欲新增之物件</param>
        public static void ListBoxAdd<TValue>(ListBox listbox, IEnumerable<TValue> value) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxAddList<TValue>(ListBoxAdd), new object[] { listbox, value });
            else if (value is string) listbox.Items.Add(value);
            else listbox.Items.AddRange(value.Cast<object>().ToArray());
        }

        /// <summary>調用 <see cref="ListBox"/> 以刪除選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="value">欲刪除之物件</param>
        public static void ListBoxRemove(ListBox listbox, object value) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxObject(ListBoxRemove), new object[] { listbox, value });
            else listbox.Items.Remove(value);
        }

        private delegate void DlgListBoxRemoveAt(ListBox listbox, int index);
        /// <summary>調用 <see cref="ListBox"/> 以刪除特定索引選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        /// <param name="index">欲刪除之索引</param>
        public static void ListBoxRemove(ListBox listbox, int index) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxRemoveAt(ListBoxRemove), new object[] { listbox, index });
            else listbox.Items.RemoveAt(index);
        }

        private delegate void DlgListBoxClear(ListBox listbox);
        /// <summary>調用 <see cref="ListBox"/> 以清空所有選項</summary>
        /// <param name="listbox">欲調用之ListBox</param>
        public static void ListBoxClear(ListBox listbox) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxClear(ListBoxClear), new object[] { listbox });
            else listbox.Items.Clear();
        }

        private delegate void DlgListBoxInt(ListBox listbox, int value);

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項寬度</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="width">欲調整之寬度</param>
        public static void ListBoxWidth(ListBox listbox, int width) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxInt(ListBoxWidth), new object[] { listbox, width });
            else listbox.Width = width;
        }

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項高度</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="height">欲調整之高度</param>
        public static void ListBoxHeight(ListBox listbox, int height) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxInt(ListBoxHeight), new object[] { listbox, height });
            else listbox.Height = height;
        }

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="left">欲調整之位置</param>
        public static void ListBoxLeft(ListBox listbox, int left) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxInt(ListBoxLeft), new object[] { listbox, left });
            else listbox.Left = left;
        }

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="top">欲調整之位置</param>
        public static void ListBoxTop(ListBox listbox, int top) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxInt(ListBoxTop), new object[] { listbox, top });
            else listbox.Top = top;
        }

        /// <summary>調用 <see cref="ListBox"/> 以調整選取選項索引</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="index">欲調整之索引</param>
        public static void ListBoxSelectedIndex(ListBox listbox, int index) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxInt(ListBoxTop), new object[] { listbox, index });
            else if (listbox.Items.Count > 0) listbox.SelectedIndex = index;
        }

        private delegate void DlgListBoxDInt(ListBox listbox, int val1, int val2);

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項大小</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void ListBoxSize(ListBox listbox, int width, int height) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxDInt(ListBoxSize), new object[] { listbox, width, height });
            else listbox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void ListBoxLocation(ListBox listbox, int left, int top) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxDInt(ListBoxLocation), new object[] { listbox, left, top });
            else listbox.Location = new Point(left, top);
        }

        private delegate void DlgListBoxSize(ListBox listbox, Size size);

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項大小</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="size">欲調整之大小</param>
        public static void ListBoxSize(ListBox listbox, Size size) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxSize(ListBoxSize), new object[] { listbox, size });
            else listbox.Size = size;
        }

        private delegate void DlgListBoxPoint(ListBox listbox, Point point);

        /// <summary>調用 <see cref="ListBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="listbox">欲調用之 ListBox</param>
        /// <param name="point">欲調整之位置</param>
        public static void ListBoxLocation(ListBox listbox, Point point) {
            if (listbox.InvokeRequired)
                listbox.BeginInvoke(new DlgListBoxPoint(ListBoxLocation), new object[] { listbox, point });
            else listbox.Location = point;
        }
        #endregion

        #region ListView Operations

        private delegate void DlgListViewBool(ListView listview, bool value);
        /// <summary>調用 <see cref="ListView"/> 啟用選項(Enabled)</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ListViewEnable(ListView listview, bool enabled) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewBool(ListViewEnable), new object[] { listview, enabled });
            else listview.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ListView"/> 可視選項(Visible)</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>

        public static void ListViewVisible(ListView listview, bool visible) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewBool(ListViewVisible), new object[] { listview, visible });
            else listview.Visible = visible;
        }

        private delegate void DlgListViewText(ListView listview, string text);
        /// <summary>調用 <see cref="ListView"/> 文字</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="text">欲顯示之文字</param>
        public static void ListViewText(ListView listview, string text) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewText(ListViewText), new object[] { listview, text });
            else listview.Text = text;
        }

        private delegate void DlgListViewIns(ListView listview, int index, ListViewItem value);
        /// <summary>調用 <see cref="ListView"/> 以插入項目</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="index">欲插入之物件索引</param>
        /// <param name="value">欲插入之物件</param>
        public static void ListViewInsert(ListView listview, int index, ListViewItem value) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewIns(ListViewInsert), new object[] { listview, index, value });
            else listview.Items.Insert(index, value);
        }

        private delegate void DlgListViewItem(ListView listview, ListViewItem value);
        /// <summary>調用 <see cref="ListView"/> 以增加項目</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="value">欲新增之物件</param>
        public static void ListViewAdd(ListView listview, ListViewItem value) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewItem(ListViewAdd), new object[] { listview, value });
            else listview.Items.Add(value);
        }

        /// <summary>調用 <see cref="ListView"/> 以移除項目</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="value">欲移除之物件</param>
        public static void ListViewRemove(ListView listview, ListViewItem value) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewItem(ListViewRemove), new object[] { listview, value });
            else listview.Items.Remove(value);
        }

        private delegate void DlgListViewRemoveAt(ListView listview, int index);
        /// <summary>調用 <see cref="ListView"/> 以移除項目</summary>
        /// <param name="listview">欲調用之ListView</param>
        /// <param name="index">欲移除之物件索引</param>
        public static void ListViewRemove(ListView listview, int index) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewRemoveAt(ListViewRemove), new object[] { listview, index });
            else listview.Items.RemoveAt(index);
        }

        private delegate void DlgListViewSort(ListView listview);

        /// <summary>調用 <see cref="ListView"/> 以排序項目</summary>
        /// <param name="listview">欲調用之ListView</param>
        public static void ListViewSort(ListView listview) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewSort(ListViewSort), new object[] { listview });
            else listview.Sort();
        }

        private delegate void DlgListViewInt(ListView listview, int value);

        /// <summary>調用 <see cref="ListView"/> 以調整控制項寬度</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="width">欲調整之寬度</param>
        public static void ListViewWidth(ListView listview, int width) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewInt(ListViewWidth), new object[] { listview, width });
            else listview.Width = width;
        }

        /// <summary>調用 <see cref="ListView"/> 以調整控制項高度</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="height">欲調整之高度</param>
        public static void ListViewHeight(ListView listview, int height) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewInt(ListViewHeight), new object[] { listview, height });
            else listview.Height = height;
        }

        /// <summary>調用 <see cref="ListView"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="left">欲調整之位置</param>
        public static void ListViewLeft(ListView listview, int left) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewInt(ListViewLeft), new object[] { listview, left });
            else listview.Left = left;
        }

        /// <summary>調用 <see cref="ListView"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="top">欲調整之位置</param>
        public static void ListViewTop(ListView listview, int top) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewInt(ListViewTop), new object[] { listview, top });
            else listview.Top = top;
        }

        private delegate void DlgListViewDInt(ListView listview, int val1, int val2);

        /// <summary>調用 <see cref="ListView"/> 以調整控制項大小</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void ListViewSize(ListView listview, int width, int height) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewDInt(ListViewSize), new object[] { listview, width, height });
            else listview.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="ListView"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void ListViewLocation(ListView listview, int left, int top) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewDInt(ListViewLocation), new object[] { listview, left, top });
            else listview.Location = new Point(left, top);
        }

        private delegate void DlgListViewSize(ListView listview, Size size);

        /// <summary>調用 <see cref="ListView"/> 以調整控制項大小</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="size">欲調整之大小</param>
        public static void ListViewSize(ListView listview, Size size) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewSize(ListViewSize), new object[] { listview, size });
            else listview.Size = size;
        }

        private delegate void DlgListViewPoint(ListView listview, Point point);

        /// <summary>調用 <see cref="ListView"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="listview">欲調用之 ListView</param>
        /// <param name="point">欲調整之位置</param>
        public static void ListViewLocation(ListView listview, Point point) {
            if (listview.InvokeRequired)
                listview.BeginInvoke(new DlgListViewPoint(ListViewLocation), new object[] { listview, point });
            else listview.Location = point;
        }
        #endregion

        #region NumericUpDown Operations

        private delegate void DlgNumericUpDownBool(NumericUpDown numeric, bool value);

        /// <summary>調用 <see cref="NumericUpDown"/> 啟用選項(Enabled)</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void NumericUpDownEnable(NumericUpDown numeric, bool enabled) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownBool(NumericUpDownEnable), new object[] { numeric, enabled });
            else numeric.Enabled = enabled;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 可視選項(Visible)</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void NumericUpDownVisible(NumericUpDown numeric, bool visible) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownBool(NumericUpDownVisible), new object[] { numeric, visible });
            else numeric.Visible = visible;
        }

        private delegate void DlgNumericUpDownColor(NumericUpDown numeric, Color color);

        /// <summary>調用 <see cref="NumericUpDown"/> 前景顏色</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="color">欲套用之顏色</param>
        public static void NumericUpDownForeColor(NumericUpDown numeric, Color color) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownColor(NumericUpDownForeColor), new object[] { numeric, color });
            else numeric.ForeColor = color;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以更改背景顏色</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="color">欲更改之背景顏色</param>

        public static void NumericUpDownBackColor(NumericUpDown numeric, Color color) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownColor(NumericUpDownBackColor), new object[] { numeric, color });
            else numeric.BackColor = color;
        }

        private delegate void DlgNumericUpDownValue(NumericUpDown numeric, decimal value);

        /// <summary>調用 <see cref="NumericUpDown"/> 以更改當前數值</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="value">欲更改之數值</param>
        public static void NumericUpDownValue(NumericUpDown numeric, decimal value) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownValue(NumericUpDownValue), new object[] { numeric, value });
            else numeric.Value = value;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以更改最大範圍</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="maximum">欲更改之數值</param>
        public static void NumericUpDownMaximum(NumericUpDown numeric, decimal maximum) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownValue(NumericUpDownMaximum), new object[] { numeric, maximum });
            else numeric.Maximum = maximum;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以更改最小範圍</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="minimum">欲更改之數值</param>
        public static void NumericUpDownMinimum(NumericUpDown numeric, decimal minimum) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownValue(NumericUpDownMinimum), new object[] { numeric, minimum });
            else numeric.Minimum = minimum;
        }

        private delegate void DlgNumericUpDownFont(NumericUpDown numeric, Font font);

        /// <summary>調用 <see cref="NumericUpDown"/> 以更改字型</summary>
        /// <param name="numeric">欲調用之NumericUpDown</param>
        /// <param name="font">欲更改之字型</param>
        public static void NumericUpDownFont(NumericUpDown numeric, Font font) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownFont(NumericUpDownFont), new object[] { numeric, font });
            else numeric.Font = font;
        }

        private delegate void DlgNumericUpDownInt(NumericUpDown numeric, int value);

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項寬度</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="width">欲調整之寬度</param>
        public static void NumericUpDownWidth(NumericUpDown numeric, int width) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownInt(NumericUpDownWidth), new object[] { numeric, width });
            else numeric.Width = width;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項高度</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="height">欲調整之高度</param>
        public static void NumericUpDownHeight(NumericUpDown numeric, int height) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownInt(NumericUpDownHeight), new object[] { numeric, height });
            else numeric.Height = height;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="left">欲調整之位置</param>
        public static void NumericUpDownLeft(NumericUpDown numeric, int left) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownInt(NumericUpDownLeft), new object[] { numeric, left });
            else numeric.Left = left;
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="top">欲調整之位置</param>
        public static void NumericUpDownTop(NumericUpDown numeric, int top) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownInt(NumericUpDownTop), new object[] { numeric, top });
            else numeric.Top = top;
        }

        private delegate void DlgNumericUpDownDInt(NumericUpDown numeric, int val1, int val2);

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項大小</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void NumericUpDownSize(NumericUpDown numeric, int width, int height) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownDInt(NumericUpDownSize), new object[] { numeric, width, height });
            else numeric.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void NumericUpDownLocation(NumericUpDown numeric, int left, int top) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownDInt(NumericUpDownLocation), new object[] { numeric, left, top });
            else numeric.Location = new Point(left, top);
        }

        private delegate void DlgNumericUpDownSize(NumericUpDown numeric, Size size);

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項大小</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="size">欲調整之大小</param>
        public static void NumericUpDownSize(NumericUpDown numeric, Size size) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownSize(NumericUpDownSize), new object[] { numeric, size });
            else numeric.Size = size;
        }

        private delegate void DlgNumericUpDownPoint(NumericUpDown numeric, Point point);

        /// <summary>調用 <see cref="NumericUpDown"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="numeric">欲調用之 NumericUpDown</param>
        /// <param name="point">欲調整之位置</param>
        public static void NumericUpDownLocation(NumericUpDown numeric, Point point) {
            if (numeric.InvokeRequired)
                numeric.BeginInvoke(new DlgNumericUpDownPoint(NumericUpDownLocation), new object[] { numeric, point });
            else numeric.Location = point;
        }

        #endregion

        #region Panel Operations

        private delegate void DlgPanelBool(Panel panel, bool value);

        /// <summary>調用 <see cref="Panel"/> 啟用選項(Enabled)</summary>
        /// <param name="panel">欲調用之Panel</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void PanelEnable(Panel panel, bool enabled) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelBool(PanelEnable), new object[] { panel, enabled });
            else panel.Enabled = enabled;
        }

        /// <summary>調用 <see cref="Panel"/> 可視選項(Visible)</summary>
        /// <param name="panel">欲調用之Panel</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void PanelVisible(Panel panel, bool visible) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelBool(PanelVisible), new object[] { panel, visible });
            else panel.Visible = visible;
        }

        private delegate void DlgPanelInt(Panel panel, int value);

        /// <summary>調用 <see cref="Panel"/> 以調整控制項寬度</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="width">欲調整之寬度</param>
        public static void PanelWidth(Panel panel, int width) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelInt(PanelWidth), new object[] { panel, width });
            else panel.Width = width;
        }

        /// <summary>調用 <see cref="Panel"/> 以調整控制項高度</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="height">欲調整之高度</param>
        public static void PanelHeight(Panel panel, int height) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelInt(PanelHeight), new object[] { panel, height });
            else panel.Height = height;
        }

        /// <summary>調用 <see cref="Panel"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="left">欲調整之位置</param>
        public static void PanelLeft(Panel panel, int left) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelInt(PanelLeft), new object[] { panel, left });
            else panel.Left = left;
        }

        /// <summary>調用 <see cref="Panel"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="top">欲調整之位置</param>
        public static void PanelTop(Panel panel, int top) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelInt(PanelTop), new object[] { panel, top });
            else panel.Top = top;
        }

        private delegate void DlgPanelDInt(Panel panel, int val1, int val2);

        /// <summary>調用 <see cref="Panel"/> 以調整控制項大小</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void PanelSize(Panel panel, int width, int height) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelDInt(PanelSize), new object[] { panel, width, height });
            else panel.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="Panel"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void PanelLocation(Panel panel, int left, int top) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelDInt(PanelLocation), new object[] { panel, left, top });
            else panel.Location = new Point(left, top);
        }

        private delegate void DlgPanelSize(Panel panel, Size size);

        /// <summary>調用 <see cref="Panel"/> 以調整控制項大小</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="size">欲調整之大小</param>
        public static void PanelSize(Panel panel, Size size) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelSize(PanelSize), new object[] { panel, size });
            else panel.Size = size;
        }

        private delegate void DlgPanelPoint(Panel panel, Point point);

        /// <summary>調用 <see cref="Panel"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="point">欲調整之位置</param>
        public static void PanelLocation(Panel panel, Point point) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelPoint(PanelLocation), new object[] { panel, point });
            else panel.Location = point;
        }

        private delegate void DlgPanelForm(Panel panel, Control form, DockStyle dock, bool show);

        /// <summary>調用 <see cref="Panel"/> 以加入新的介面，屆時將會依附在 Panel 裡</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="form">欲依附的表單或控制項</param>
        /// <param name="dock">停駐方式</param>
        /// <param name="show">依附後是否直接顯示</param>
        public static void PanelAddControl(Panel panel, Control form, DockStyle dock = DockStyle.Fill, bool show = true) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelForm(PanelAddControl), new object[] { panel, form, dock, show });
            else {
                panel.Controls.Add(form);
                form.Dock = dock;
                if (show) form.Show();
            }
        }

        private delegate void DlgPanelColor(Panel panel, Color color);

        /// <summary>調用 <see cref="Panel"/> 以更改背景顏色</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="color">欲更改之顏色</param>
        public static void PanelBackColor(Panel panel, Color color) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelColor(PanelBackColor), new object[] { panel, color });
            else panel.BackColor = color;
        }

        /// <summary>調用 <see cref="Panel"/> 以更改前景顏色</summary>
        /// <param name="panel">欲調用之 Panel</param>
        /// <param name="color">欲更改之顏色</param>
        public static void PanelForeColor(Panel panel, Color color) {
            if (panel.InvokeRequired)
                panel.BeginInvoke(new DlgPanelColor(PanelForeColor), new object[] { panel, color });
            else panel.ForeColor = color;
        }
        #endregion

        #region PictureBox Operations

        private delegate void DlgPictureBoxBool(PictureBox picbox, bool value);

        /// <summary>調用 <see cref="PictureBox"/> 啟用選項(Enabled)</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void PictureBoxEnable(PictureBox picbox, bool enabled) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxBool(PictureBoxEnable), new object[] { picbox, enabled });
            else picbox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="PictureBox"/> 可視選項(Visible)</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void PictureBoxVisible(PictureBox picbox, bool visible) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxBool(PictureBoxVisible), new object[] { picbox, visible });
            else picbox.Visible = visible;
        }

        private delegate void DlgPictureBoxImage(PictureBox picbox, Image img);

        /// <summary>調用 <see cref="PictureBox"/> 以更改其顯示圖片</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="img">欲更換之圖片</param>
        public static void PictureBoxImage(PictureBox picbox, Image img) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxImage(PictureBoxImage), new object[] { picbox, img });
            else picbox.Image = img;
        }

        private delegate void DlgPictureBoxColor(PictureBox picbox, Color color);
        /// <summary>調用 <see cref="PictureBox"/> 以更改背景顏色</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="color">欲套用之顏色</param>
        public static void PictureBoxBackColor(PictureBox picbox, Color color) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxColor(PictureBoxBackColor), new object[] { picbox, color });
            else picbox.BackColor = color;
        }

        /// <summary>調用 <see cref="PictureBox"/> 以刷為特定顏色</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="color">欲套用之顏色</param>
        public static void PictureBoxBackDraw(PictureBox picbox, Color color) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxColor(PictureBoxBackDraw), new object[] { picbox, color });
            else {
                Graphics g = picbox.CreateGraphics();
                g.Clear(color);
                Application.DoEvents();
            }
        }

        private delegate void DlgPictureBoxDrawString(PictureBox picbox, string text, Font font, Brush brush, float x, float y);

        /// <summary>調用 <see cref="PictureBox"/> 以繪出文字</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="text">欲顯示之文字</param>
        /// <param name="font">顯示文字之字體</param>
        /// <param name="brush">繪畫文字之筆刷樣式</param>
        /// <param name="x">繪出文字之起始X座標</param>
        /// <param name="y">繪出文字之起始Y座標</param>
        public static void PictureBoxBackDraw(PictureBox picbox, string text, Font font, Brush brush, float x, float y) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxDrawString(PictureBoxBackDraw), new object[] { picbox, text, font, brush, x, y });
            else {
                Graphics g = picbox.CreateGraphics();
                g.DrawString(text, font, brush, x, y);
                Application.DoEvents();
            }
        }

        private delegate void DlgPictureBoxTag(PictureBox picbox, object tag);

        /// <summary>調用 <see cref="PictureBox"/> 以更改其Tag</summary>
        /// <param name="picbox">欲調用之PictureBox</param>
        /// <param name="tag">欲更換之Tag</param>
        public static void PictureBoxTag(PictureBox picbox, object tag) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxTag(PictureBoxTag), new object[] { picbox, tag });
            else picbox.Tag = tag;
        }

        private delegate void DlgPictureBoxInt(PictureBox picbox, int value);

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項寬度</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void PictureBoxWidth(PictureBox picbox, int width) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxInt(PictureBoxWidth), new object[] { picbox, width });
            else picbox.Width = width;
        }

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項高度</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="height">欲調整之高度</param>
        public static void PictureBoxHeight(PictureBox picbox, int height) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxInt(PictureBoxHeight), new object[] { picbox, height });
            else picbox.Height = height;
        }

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="left">欲調整之位置</param>
        public static void PictureBoxLeft(PictureBox picbox, int left) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxInt(PictureBoxLeft), new object[] { picbox, left });
            else picbox.Left = left;
        }

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="top">欲調整之位置</param>
        public static void PictureBoxTop(PictureBox picbox, int top) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxInt(PictureBoxTop), new object[] { picbox, top });
            else picbox.Top = top;
        }

        private delegate void DlgPictureBoxDInt(PictureBox picbox, int val1, int val2);

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項大小</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void PictureBoxSize(PictureBox picbox, int width, int height) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxDInt(PictureBoxSize), new object[] { picbox, width, height });
            else picbox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void PictureBoxLocation(PictureBox picbox, int left, int top) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxDInt(PictureBoxLocation), new object[] { picbox, left, top });
            else picbox.Location = new Point(left, top);
        }

        private delegate void DlgPictureBoxSize(PictureBox picbox, Size size);

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項大小</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="size">欲調整之大小</param>
        public static void PictureBoxSize(PictureBox picbox, Size size) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxSize(PictureBoxSize), new object[] { picbox, size });
            else picbox.Size = size;
        }

        private delegate void DlgPictureBoxPoint(PictureBox picbox, Point point);

        /// <summary>調用 <see cref="PictureBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="picbox">欲調用之 圖片盒(PictureBox)</param>
        /// <param name="point">欲調整之位置</param>
        public static void PictureBoxLocation(PictureBox picbox, Point point) {
            if (picbox.InvokeRequired)
                picbox.BeginInvoke(new DlgPictureBoxPoint(PictureBoxLocation), new object[] { picbox, point });
            else picbox.Location = point;
        }
        #endregion

        #region ProgressBar Operations

        private delegate void DlgProgressBarBool(ProgressBar progress, bool value);

        /// <summary>調用 <see cref="ProgressBar"/> 啟用選項(Enabled)</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ProgressBarEnable(ProgressBar progress, bool enabled) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarBool(ProgressBarEnable), new object[] { progress, enabled });
            else progress.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 可視選項(Visible)</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ProgressBarVisible(ProgressBar progress, bool visible) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarBool(ProgressBarVisible), new object[] { progress, visible });
            else progress.Visible = visible;
        }

        private delegate void DlgProgressBarBgColor(ProgressBar progress, Color color);

        /// <summary>調用 <see cref="ProgressBar"/> 以更改背景顏色</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="color">欲更改之背景顏色</param>
        public static void ProgressBarBackColor(ProgressBar progress, Color color) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarBgColor(ProgressBarBackColor), new object[] { progress, color });
            else progress.BackColor = color;
        }

        private delegate void DlgProgressBarValue(ProgressBar progress, int value);

        /// <summary>調用 <see cref="ProgressBar"/> 以更改當前數值</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="value">欲更改之數值</param>
        public static void ProgressBarValue(ProgressBar progress, int value) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarValue(ProgressBarValue), new object[] { progress, value });
            else progress.Value = value;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以更改數值最大範圍</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="max">欲更改之數值</param>
        public static void ProgressBarMaximum(ProgressBar progress, int max) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarValue(ProgressBarMaximum), new object[] { progress, max });
            else progress.Maximum = max;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以更改最大與最小值範圍</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static void ProgressBarRange(ProgressBar progress, int min, int max) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarDInt(ProgressBarRange), new object[] { progress, min, max });
            else {
                progress.Minimum = min;
                progress.Maximum = max;
            }
        }

        private delegate void DlgProgressBarStyle(ProgressBar progress, System.Windows.Forms.ProgressBarStyle style);

        /// <summary>調用 <see cref="ProgressBar"/> 以更改當前樣式</summary>
        /// <param name="progress">欲調用之ProgressBar</param>
        /// <param name="style">欲更改之樣式</param>
        public static void ProgressBarStyle(ProgressBar progress, System.Windows.Forms.ProgressBarStyle style) {
            if (progress.InvokeRequired)
                progress.BeginInvoke(new DlgProgressBarStyle(ProgressBarStyle), new object[] { progress, style });
            else progress.Style = style;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項寬度</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void ProgressBarWidth(ProgressBar progbar, int width) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarValue(ProgressBarWidth), new object[] { progbar, width });
            else progbar.Width = width;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項高度</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="height">欲調整之高度</param>
        public static void ProgressBarHeight(ProgressBar progbar, int height) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarValue(ProgressBarHeight), new object[] { progbar, height });
            else progbar.Height = height;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="left">欲調整之位置</param>
        public static void ProgressBarLeft(ProgressBar progbar, int left) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarValue(ProgressBarLeft), new object[] { progbar, left });
            else progbar.Left = left;
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="top">欲調整之位置</param>
        public static void ProgressBarTop(ProgressBar progbar, int top) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarValue(ProgressBarTop), new object[] { progbar, top });
            else progbar.Top = top;
        }

        private delegate void DlgProgressBarDInt(ProgressBar progbar, int val1, int val2);

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項大小</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void ProgressBarSize(ProgressBar progbar, int width, int height) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarDInt(ProgressBarSize), new object[] { progbar, width, height });
            else progbar.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void ProgressBarLocation(ProgressBar progbar, int left, int top) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarDInt(ProgressBarLocation), new object[] { progbar, left, top });
            else progbar.Location = new Point(left, top);
        }

        private delegate void DlgProgressBarSize(ProgressBar progbar, Size size);

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項大小</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="size">欲調整之大小</param>
        public static void ProgressBarSize(ProgressBar progbar, Size size) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarSize(ProgressBarSize), new object[] { progbar, size });
            else progbar.Size = size;
        }

        private delegate void DlgProgressBarPoint(ProgressBar progbar, Point point);

        /// <summary>調用 <see cref="ProgressBar"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="progbar">欲調用之 進度條(ProgressBar)</param>
        /// <param name="point">欲調整之位置</param>
        public static void ProgressBarLocation(ProgressBar progbar, Point point) {
            if (progbar.InvokeRequired)
                progbar.BeginInvoke(new DlgProgressBarPoint(ProgressBarLocation), new object[] { progbar, point });
            else progbar.Location = point;
        }
        #endregion

        #region RadioButton Operations

        private delegate void DlgRadioButtonBool(RadioButton radio, bool value);

        /// <summary>調用 <see cref="RadioButton"/> 啟用選項(Enabled)</summary>
        /// <param name="radio">欲調用之 RadioButton</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>

        public static void RadioButtonEnable(RadioButton radio, bool enabled) {
            if (radio.InvokeRequired)
                radio.BeginInvoke(new DlgRadioButtonBool(RadioButtonEnable), new object[] { radio, enabled });
            else radio.Enabled = enabled;
        }

        /// <summary>調用 <see cref="RadioButton"/> 可視選項</summary>
        /// <param name="radio">欲調用之 RadioButton</param>
        /// <param name="visible">可視?  (True)可見 (False)不可見</param>
        public static void RadioButtonVisible(RadioButton radio, bool visible) {
            if (radio.InvokeRequired)
                radio.BeginInvoke(new DlgRadioButtonBool(RadioButtonVisible), new object[] { radio, visible });
            else radio.Visible = visible;
        }

        /// <summary>調用 <see cref="RadioButton"/> 勾選狀態</summary>
        /// <param name="radio">欲調用之RadioButton</param>
        /// <param name="check">勾選?  (True)勾選 (False)取消勾選</param>
        public static void RadioButtonChecked(RadioButton radio, bool check) {
            if (radio.InvokeRequired)
                radio.BeginInvoke(new DlgRadioButtonBool(RadioButtonChecked), new object[] { radio, check });
            else radio.Checked = check;
        }

        private delegate void DlgRadioButtonInt(RadioButton radioBtn, int value);

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項寬度</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="width">欲調整之寬度</param>
        public static void RadioButtonWidth(RadioButton radioBtn, int width) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonInt(RadioButtonWidth), new object[] { radioBtn, width });
            else radioBtn.Width = width;
        }

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項高度</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="height">欲調整之高度</param>
        public static void RadioButtonHeight(RadioButton radioBtn, int height) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonInt(RadioButtonHeight), new object[] { radioBtn, height });
            else radioBtn.Height = height;
        }

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="left">欲調整之位置</param>
        public static void RadioButtonLeft(RadioButton radioBtn, int left) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonInt(RadioButtonLeft), new object[] { radioBtn, left });
            else radioBtn.Left = left;
        }

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="top">欲調整之位置</param>
        public static void RadioButtonTop(RadioButton radioBtn, int top) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonInt(RadioButtonTop), new object[] { radioBtn, top });
            else radioBtn.Top = top;
        }

        private delegate void DlgRadioButtonDInt(RadioButton radioBtn, int val1, int val2);

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項大小</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void RadioButtonSize(RadioButton radioBtn, int width, int height) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonDInt(RadioButtonSize), new object[] { radioBtn, width, height });
            else radioBtn.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void RadioButtonLocation(RadioButton radioBtn, int left, int top) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonDInt(RadioButtonLocation), new object[] { radioBtn, left, top });
            else radioBtn.Location = new Point(left, top);
        }

        private delegate void DlgRadioButtonSize(RadioButton radioBtn, Size size);

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項大小</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="size">欲調整之大小</param>
        public static void RadioButtonSize(RadioButton radioBtn, Size size) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonSize(RadioButtonSize), new object[] { radioBtn, size });
            else radioBtn.Size = size;
        }

        private delegate void DlgRadioButtonPoint(RadioButton radioBtn, Point point);

        /// <summary>調用 <see cref="RadioButton"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="radioBtn">欲調用之 RadioButton</param>
        /// <param name="point">欲調整之位置</param>
        public static void RadioButtonLocation(RadioButton radioBtn, Point point) {
            if (radioBtn.InvokeRequired)
                radioBtn.BeginInvoke(new DlgRadioButtonPoint(RadioButtonLocation), new object[] { radioBtn, point });
            else radioBtn.Location = point;
        }

        #endregion

        #region RichTextBox Operations

        private delegate void DlgRichTextBoxClean(RichTextBox richTextBox);

        /// <summary>調用 <see cref="RichTextBox"/> 清除文字</summary>
        /// <param name="richTextBox">欲調用之RichTextBox</param>
        public static void RichTextBoxClear(RichTextBox richTextBox) {
            if (richTextBox.InvokeRequired)
                richTextBox.BeginInvoke(new DlgRichTextBoxClean(RichTextBoxClear), new object[] { richTextBox });
            else richTextBox.Text = "";
        }

        private delegate void DlgRichTextBoxText(RichTextBox richTextBox, string text, Color color, bool top = false);

        /// <summary>調用 <see cref="RichTextBox"/> 多行文字</summary>
        /// <param name="richTextBox">欲調用之RichTextBox</param>
        /// <param name="text">欲顯示之文字</param>
        /// <param name="color">欲顯示之文字顏色</param>
        /// <param name="top">是否放於最上方?  (True)上方 (False)下方</param>
        public static void RichTextBoxText(RichTextBox richTextBox, string text, Color color, bool top = false) {
            if (richTextBox.InvokeRequired)
                richTextBox.BeginInvoke(new DlgRichTextBoxText(RichTextBoxText), new object[] { richTextBox, text, color, top });
            else {
                richTextBox.SelectionStart = (top) ? 0 : richTextBox.Text.Length;
                richTextBox.SelectionColor = color;
                richTextBox.SelectedText = text + Environment.NewLine;
            }
        }

        private delegate void DlgRichTextBoxInt(RichTextBox richTxt, int value);

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項寬度</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="width">欲調整之寬度</param>
        public static void RichTextBoxWidth(RichTextBox richTxt, int width) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxInt(RichTextBoxWidth), new object[] { richTxt, width });
            else richTxt.Width = width;
        }

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項高度</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="height">欲調整之高度</param>
        public static void RichTextBoxHeight(RichTextBox richTxt, int height) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxInt(RichTextBoxHeight), new object[] { richTxt, height });
            else richTxt.Height = height;
        }

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="left">欲調整之位置</param>
        public static void RichTextBoxLeft(RichTextBox richTxt, int left) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxInt(RichTextBoxLeft), new object[] { richTxt, left });
            else richTxt.Left = left;
        }

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="top">欲調整之位置</param>
        public static void RichTextBoxTop(RichTextBox richTxt, int top) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxInt(RichTextBoxTop), new object[] { richTxt, top });
            else richTxt.Top = top;
        }

        private delegate void DlgRichTextBoxDInt(RichTextBox richTxt, int val1, int val2);

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項大小</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void RichTextBoxSize(RichTextBox richTxt, int width, int height) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxDInt(RichTextBoxSize), new object[] { richTxt, width, height });
            else richTxt.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void RichTextBoxLocation(RichTextBox richTxt, int left, int top) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxDInt(RichTextBoxLocation), new object[] { richTxt, left, top });
            else richTxt.Location = new Point(left, top);
        }

        private delegate void DlgRichTextBoxSize(RichTextBox richTxt, Size size);

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項大小</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="size">欲調整之大小</param>
        public static void RichTextBoxSize(RichTextBox richTxt, Size size) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxSize(RichTextBoxSize), new object[] { richTxt, size });
            else richTxt.Size = size;
        }

        private delegate void DlgRichTextBoxPoint(RichTextBox richTxt, Point point);

        /// <summary>調用 <see cref="RichTextBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="richTxt">欲調用之 RichTextBox</param>
        /// <param name="point">欲調整之位置</param>
        public static void RichTextBoxLocation(RichTextBox richTxt, Point point) {
            if (richTxt.InvokeRequired)
                richTxt.BeginInvoke(new DlgRichTextBoxPoint(RichTextBoxLocation), new object[] { richTxt, point });
            else richTxt.Location = point;
        }
        #endregion

        #region TabControl Operations

        private delegate void DlgTabControlBool(TabControl tab, bool value);
        /// <summary>調用 <see cref="TabControl"/> 啟用選項(Enabled)</summary>
        /// <param name="tab">欲調用之TabControl</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void TabControlEnable(TabControl tab, bool enabled) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlBool(TabControlEnable), new object[] { tab, enabled });
            else tab.Enabled = enabled;
        }

        /// <summary>調用 <see cref="TabControl"/> 可視選項(Visible)</summary>
        /// <param name="tab">欲調用之TabControl</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void TabControlVisible(TabControl tab, bool visible) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlBool(TabControlVisible), new object[] { tab, visible });
            else tab.Visible = visible;
        }

        private delegate void DlgTabControlBgColor(TabControl tab, Color color);
        /// <summary>調用 <see cref="TabControl"/> 以更改背景顏色</summary>
        /// <param name="tab">欲調用之TabControl</param>
        /// <param name="color">欲更改之背景顏色</param>
        public static void TabControlBackColor(TabControl tab, Color color) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlBgColor(TabControlBackColor), new object[] { tab, color });
            else tab.BackColor = color;
        }

        private delegate void DlgTabControlValue(TabControl tab, int value);
        /// <summary>調用 <see cref="TabControl"/> 以更改選擇頁面</summary>
        /// <param name="tab">欲調用之TabControl</param>
        /// <param name="value">欲更改之頁面索引</param>
        public static void TabControlSelectedTab(TabControl tab, int value) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlValue(TabControlSelectedTab), new object[] { tab, value });
            else tab.SelectedIndex = value;
        }

        private delegate void DlgTabControlInt(TabControl tab, int value);

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項寬度</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void TabControlWidth(TabControl tab, int width) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlInt(TabControlWidth), new object[] { tab, width });
            else tab.Width = width;
        }

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項高度</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="height">欲調整之高度</param>
        public static void TabControlHeight(TabControl tab, int height) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlInt(TabControlHeight), new object[] { tab, height });
            else tab.Height = height;
        }

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="left">欲調整之位置</param>
        public static void TabControlLeft(TabControl tab, int left) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlInt(TabControlLeft), new object[] { tab, left });
            else tab.Left = left;
        }

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="top">欲調整之位置</param>
        public static void TabControlTop(TabControl tab, int top) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlInt(TabControlTop), new object[] { tab, top });
            else tab.Top = top;
        }

        private delegate void DlgTabControlDInt(TabControl tab, int val1, int val2);

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項大小</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void TabControlSize(TabControl tab, int width, int height) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlDInt(TabControlSize), new object[] { tab, width, height });
            else tab.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void TabControlLocation(TabControl tab, int left, int top) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlDInt(TabControlLocation), new object[] { tab, left, top });
            else tab.Location = new Point(left, top);
        }

        private delegate void DlgTabControlSize(TabControl tab, Size size);

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項大小</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="size">欲調整之大小</param>
        public static void TabControlSize(TabControl tab, Size size) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlSize(TabControlSize), new object[] { tab, size });
            else tab.Size = size;
        }

        private delegate void DlgTabControlPoint(TabControl tab, Point point);

        /// <summary>調用 <see cref="TabControl"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="tab">欲調用之 分頁(TabControl)</param>
        /// <param name="point">欲調整之位置</param>
        public static void TabControlLocation(TabControl tab, Point point) {
            if (tab.InvokeRequired)
                tab.BeginInvoke(new DlgTabControlPoint(TabControlLocation), new object[] { tab, point });
            else tab.Location = point;
        }
        #endregion

        #region TextBox Operations

        private delegate void DlgTextBoxBool(TextBox textbox, bool value);

        /// <summary>調用 <see cref="TextBox"/> 啟用選項(Enabled)</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void TextBoxEnable(TextBox textbox, bool enabled) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxBool(TextBoxEnable), new object[] { textbox, enabled });
            else textbox.Enabled = enabled;
        }

        /// <summary>調用 <see cref="TextBox"/> 可視選項(Visible)</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void TextBoxVisible(TextBox textbox, bool visible) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxBool(TextBoxVisible), new object[] { textbox, visible });
            else textbox.Visible = visible;
        }

        private delegate void DlgTextBoxText(TextBox textbox, string text, bool append, bool top);

        /// <summary>調用 <see cref="TextBox"/> 文字</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="text">欲顯示之文字</param>
        /// <param name="append">是否附加?  (True)使用附加而非覆蓋 (False)捨棄舊有文字直接覆蓋</param>
        /// <param name="top">是否加入於最上方?  (True)加入的文字位於最前方 (False)新增文字加於最後方</param>
        public static void TextBoxText(TextBox textbox, string text, bool append = false, bool top = false) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxText(TextBoxText), new object[] { textbox, text, append, top });
            else {
                string strTemp = "";

                if (!append) strTemp = text;
                else if (top) strTemp = text + textbox.Text;
                else strTemp = textbox.Text + text;

                if (strTemp.Length > 2048) strTemp = strTemp.Substring(0, 2047);
                textbox.Text = strTemp;
            }
        }

        private delegate void DlgTextBoxTextList(TextBox textbox, IEnumerable<string> text, bool append, bool top);

        /// <summary>調用 <see cref="TextBox"/> 多行文字</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="text">欲顯示之文字</param>
        /// <param name="append">是否附加?  (True)使用附加而非覆蓋 (False)捨棄舊有文字直接覆蓋</param>
        /// <param name="top">是否加入於最上方?  (True)加入的文字位於最前方 (False)新增文字加於最後方</param>
        public static void TextBoxText(TextBox textbox, IEnumerable<string> text, bool append = false, bool top = false) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxTextList(TextBoxText), new object[] { textbox, text, append, top });
            else {
                string strMsg = "", strTemp = "";

                strMsg = string.Join(CtConst.NewLine, text.ToArray());
                textbox.Multiline = true;

                if (!append) strTemp = strMsg;
                else if (top) strTemp = strMsg + textbox.Text;
                else strTemp = textbox.Text + strMsg;

                if (strTemp.Length > 2048) strTemp = strTemp.Substring(0, 2047);
                textbox.Text = strTemp;
            }
        }

        private delegate void DlgTextBoxColor(TextBox textbox, Color color);

        /// <summary>調用 <see cref="TextBox"/> 背景顏色</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="color">欲套用之顏色</param>
        public static void TextBoxBackColor(TextBox textbox, Color color) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxColor(TextBoxBackColor), new object[] { textbox, color });
            else textbox.BackColor = color;
        }

        /// <summary>調用 <see cref="TextBox"/> 前景顏色(Enabled)</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="color">欲套用之顏色</param>
        public static void TextBoxForeColor(TextBox textbox, Color color) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxColor(TextBoxForeColor), new object[] { textbox, color });
            else textbox.ForeColor = color;
        }

        private delegate void DlgTextBoxTag(TextBox textbox, object tag);

        /// <summary>調用 <see cref="TextBox"/> 標籤</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="tag">標籤 (任意物件)</param>
        public static void TextBoxTag(TextBox textbox, object tag) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxTag(TextBoxTag), new object[] { textbox, tag });
            else textbox.Tag = tag;
        }

        private delegate void DlgTextBoxFont(TextBox textbox, Font font);
        /// <summary>調用 <see cref="TextBox"/> 字體</summary>
        /// <param name="textbox">欲調用之TextBox</param>
        /// <param name="font">字體</param>
        public static void TextBoxFont(TextBox textbox, Font font) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxFont(TextBoxFont), new object[] { textbox, font });
            else textbox.Font = font;
        }

        private delegate void DlgTextBoxInt(TextBox textbox, int value);

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項寬度</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void TextBoxWidth(TextBox textbox, int width) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxInt(TextBoxWidth), new object[] { textbox, width });
            else textbox.Width = width;
        }

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項高度</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="height">欲調整之高度</param>
        public static void TextBoxHeight(TextBox textbox, int height) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxInt(TextBoxHeight), new object[] { textbox, height });
            else textbox.Height = height;
        }

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="left">欲調整之位置</param>
        public static void TextBoxLeft(TextBox textbox, int left) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxInt(TextBoxLeft), new object[] { textbox, left });
            else textbox.Left = left;
        }

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="top">欲調整之位置</param>
        public static void TextBoxTop(TextBox textbox, int top) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxInt(TextBoxTop), new object[] { textbox, top });
            else textbox.Top = top;
        }

        private delegate void DlgTextBoxDInt(TextBox textbox, int val1, int val2);

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項大小</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void TextBoxSize(TextBox textbox, int width, int height) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxDInt(TextBoxSize), new object[] { textbox, width, height });
            else textbox.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void TextBoxLocation(TextBox textbox, int left, int top) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxDInt(TextBoxLocation), new object[] { textbox, left, top });
            else textbox.Location = new Point(left, top);
        }

        private delegate void DlgTextBoxSize(TextBox textbox, Size size);

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項大小</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="size">欲調整之大小</param>
        public static void TextBoxSize(TextBox textbox, Size size) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxSize(TextBoxSize), new object[] { textbox, size });
            else textbox.Size = size;
        }

        private delegate void DlgTextBoxPoint(TextBox textbox, Point point);

        /// <summary>調用 <see cref="TextBox"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="point">欲調整之位置</param>
        public static void TextBoxLocation(TextBox textbox, Point point) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxPoint(TextBoxLocation), new object[] { textbox, point });
            else textbox.Location = point;
        }

        private delegate void DlgTextBoxCursor(TextBox textbox, Cursor cursor);

        /// <summary>調用 <see cref="TextBox"/> 以調整滑鼠指標樣式</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        /// <param name="cursor">欲套用之滑鼠指標</param>
        public static void TextBoxCursor(TextBox textbox, Cursor cursor) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBoxCursor(TextBoxCursor), new object[] { textbox, cursor });
            else textbox.Cursor = cursor;
        }

        private delegate void DlgTextBox(TextBox textbox);

        /// <summary>調用 <see cref="TextBox"/> 以取得焦點</summary>
        /// <param name="textbox">欲調用之 文字盒(TextBox)</param>
        public static void TextBoxFocus(TextBox textbox) {
            if (textbox.InvokeRequired)
                textbox.BeginInvoke(new DlgTextBox(TextBoxFocus), new object[] { textbox });
            else textbox.Focus();
        }
        #endregion

        #region ToolStrip & ToolStripMenuItem Operations

        /* MenuStrip → 系統選單(子類別僅有選單，即 ToolStripMenuItem)，如一般程式上的檔案、檢視、幫助等
           ToolStrip → 工具欄，有眾多子元件，如Button、Spliter、TextBox等，也就是一般程式上面可移動、可更改內容的工具列
           MenuStrip 是繼承於 ToolStrip，故 MenuStrip 可以直接丟入以下的 ToolStrip Function 
           其子元件均是繼承於 ToolStripItem，故不管是 ToolStripMenuItem、ToolStripButton都可以用以下的 ToolStripItem Function */

        private delegate void DlgToolStripBool(ToolStrip strip, bool value);
        /// <summary>調用 <see cref="ToolStrip"/> 啟用選項(Enabled)</summary>
        /// <param name="strip">欲調用之ToolStrip</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ToolStripEnable(ToolStrip strip, bool enabled) {
            if (strip.InvokeRequired)
                strip.BeginInvoke(new DlgToolStripBool(ToolStripEnable), new object[] { strip, enabled });
            else strip.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ToolStrip"/> 可視選項(Visible)</summary>
        /// <param name="strip">欲調用之ToolStrip</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ToolStripVisible(ToolStrip strip, bool visible) {
            if (strip.InvokeRequired)
                strip.BeginInvoke(new DlgToolStripBool(ToolStripVisible), new object[] { strip, visible });
            else strip.Visible = visible;
        }

        private delegate void DlgToolStripBgColor(ToolStrip strip, Color color);
        /// <summary>調用 <see cref="ToolStrip"/> 以更改背景顏色</summary>
        /// <param name="strip">欲調用之ToolStrip</param>
        /// <param name="color">欲更改之背景顏色</param>
        public static void ToolStripBackColor(ToolStrip strip, Color color) {
            if (strip.InvokeRequired)
                strip.BeginInvoke(new DlgToolStripBgColor(ToolStripBackColor), new object[] { strip, color });
            else strip.BackColor = color;
        }

        private delegate void DlgToolStripItemBool(ToolStripItem item, bool value);
        /// <summary>調用 <see cref="ToolStripItem"/> 啟用選項(Enabled)</summary>
        /// <param name="item">欲調用之ToolStripItem</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void ToolStripItemEnable(ToolStripItem item, bool enabled) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripItemBool(ToolStripItemEnable), new object[] { item, enabled });
            else item.Enabled = enabled;
        }

        /// <summary>調用 <see cref="ToolStripItem"/> 可視選項(Visible)</summary>
        /// <param name="item">欲調用之ToolStripItem</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void ToolStripItemVisible(ToolStripItem item, bool visible) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripItemBool(ToolStripItemVisible), new object[] { item, visible });
            else item.Visible = visible;
        }

        private delegate void DlgToolStripMenuItemBool(ToolStripMenuItem item, bool value);
        /// <summary>調用 <see cref="ToolStripMenuItem"/> 並更改勾選狀態 (此功能僅支援 ToolStripMenuItem，其他如ToolStripItem、ToolStripButton等並不支援)</summary>
        /// <param name="item">欲調用之ToolStripMenuItem</param>
        /// <param name="check">是否勾選?  (True)勾選 (False)取消勾選</param>
        public static void ToolStripItemChecked(ToolStripMenuItem item, bool check) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripMenuItemBool(ToolStripItemChecked), new object[] { item, check });
            else item.Checked = check;
        }

        private delegate void DlgToolStripItemText(ToolStripItem item, string text);
        /// <summary>調用 <see cref="ToolStripItem"/>Item 以更改顯示文字</summary>
        /// <param name="item">欲調用之ToolStripItem</param>
        /// <param name="text">欲更換之文字</param>
        public static void ToolStripItemText(ToolStripItem item, string text) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripItemText(ToolStripItemText), new object[] { item, text });
            else item.Text = text;
        }

        private delegate void DlgToolStripItemTag(ToolStripItem item, object tag);
        /// <summary>調用 <see cref="ToolStripItem"/> 以更改顯示文字</summary>
        /// <param name="item">欲調用之ToolStripItem</param>
        /// <param name="tag">欲更換之文字</param>
        public static void ToolStripItemTag(ToolStripItem item, object tag) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripItemTag(ToolStripItemTag), new object[] { item, tag });
            else item.Tag = tag;
        }

        private delegate void DlgToolStripItemImage(ToolStripItem item, Image img);
        /// <summary>調用 <see cref="ToolStripItem"/> 以更改顯示文字</summary>
        /// <param name="item">欲調用之ToolStripItem</param>
        /// <param name="img">欲更換之文字</param>
        public static void ToolStripItemImage(ToolStripItem item, Image img) {
            if (item.GetCurrentParent().InvokeRequired)
                item.GetCurrentParent().BeginInvoke(new DlgToolStripItemImage(ToolStripItemImage), new object[] { item, img });
            else item.Image = img;
        }

        #endregion

        #region TrackBar Operations

        private delegate void DlgTrackBarBool(TrackBar track, bool value);
        /// <summary>調用 <see cref="TrackBar"/> 啟用選項(Enabled)</summary>
        /// <param name="track">欲調用之TrackBar</param>
        /// <param name="enabled">啟用?  (True)啟用 (False)禁用(變灰色)</param>
        public static void TrackBarEnable(TrackBar track, bool enabled) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarBool(TrackBarEnable), new object[] { track, enabled });
            else track.Enabled = enabled;
        }

        /// <summary>調用 <see cref="TrackBar"/> 可視選項(Visible)</summary>
        /// <param name="track">欲調用之TrackBar</param>
        /// <param name="visible">可見?  (True)可見 (False)不可見</param>
        public static void TrackBarVisible(TrackBar track, bool visible) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarBool(TrackBarVisible), new object[] { track, visible });
            else track.Visible = visible;
        }

        private delegate void DlgTrackBarBgColor(TrackBar track, Color color);
        /// <summary>調用 <see cref="TrackBar"/> 以更改背景顏色</summary>
        /// <param name="track">欲調用之TrackBar</param>
        /// <param name="color">欲更改之背景顏色</param>
        public static void TrackBarBackColor(TrackBar track, Color color) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarBgColor(TrackBarBackColor), new object[] { track, color });
            else track.BackColor = color;
        }

        private delegate void DlgTrackBarValue(TrackBar track, int value);
        /// <summary>調用 <see cref="TrackBar"/> 以更改當前數值</summary>
        /// <param name="track">欲調用之TrackBar</param>
        /// <param name="value">欲更改之數值</param>
        public static void TrackBarValue(TrackBar track, int value) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarValue(TrackBarValue), new object[] { track, value });
            else track.Value = value;
        }

        private delegate void DlgTrackBarInt(TrackBar track, int value);

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項寬度</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="width">欲調整之寬度</param>
        public static void TrackBarWidth(TrackBar track, int width) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarInt(TrackBarWidth), new object[] { track, width });
            else track.Width = width;
        }

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項高度</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="height">欲調整之高度</param>
        public static void TrackBarHeight(TrackBar track, int height) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarInt(TrackBarHeight), new object[] { track, height });
            else track.Height = height;
        }

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項錨點(Anchor)距離父元件(Parent)左邊緣相對距離</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="left">欲調整之位置</param>
        public static void TrackBarLeft(TrackBar track, int left) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarInt(TrackBarLeft), new object[] { track, left });
            else track.Left = left;
        }

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項錨點(Anchor)距離父元件(Parent)上緣相對距離</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="top">欲調整之位置</param>
        public static void TrackBarTop(TrackBar track, int top) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarInt(TrackBarTop), new object[] { track, top });
            else track.Top = top;
        }

        private delegate void DlgTrackBarDInt(TrackBar track, int val1, int val2);

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項大小</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="width">欲調整之寬度</param>
        /// <param name="height">欲調整之高度</param>
        public static void TrackBarSize(TrackBar track, int width, int height) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarDInt(TrackBarSize), new object[] { track, width, height });
            else track.Size = new Size(width, height);
        }

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="left">欲調整之寬度</param>
        /// <param name="top">欲調整之高度</param>
        public static void TrackBarLocation(TrackBar track, int left, int top) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarDInt(TrackBarLocation), new object[] { track, left, top });
            else track.Location = new Point(left, top);
        }

        private delegate void DlgTrackBarSize(TrackBar track, Size size);

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項大小</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="size">欲調整之大小</param>
        public static void TrackBarSize(TrackBar track, Size size) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarSize(TrackBarSize), new object[] { track, size });
            else track.Size = size;
        }

        private delegate void DlgTrackBarPoint(TrackBar track, Point point);

        /// <summary>調用 <see cref="TrackBar"/> 以調整控制項錨點於父元件(Parent)之相對位置。預設錨點(Anchor)為左上角</summary>
        /// <param name="track">欲調用之 滑桿條(TrackBar)</param>
        /// <param name="point">欲調整之位置</param>
        public static void TrackBarLocation(TrackBar track, Point point) {
            if (track.InvokeRequired)
                track.BeginInvoke(new DlgTrackBarPoint(TrackBarLocation), new object[] { track, point });
            else track.Location = point;
        }
        #endregion

        #endregion

    }
}
