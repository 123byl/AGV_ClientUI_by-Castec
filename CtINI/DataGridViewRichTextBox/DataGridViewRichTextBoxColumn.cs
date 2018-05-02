using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;

namespace DataGridViewRichTextBox
{
    /// <summary>
    /// �r��˦��w�q���c
    /// </summary>
    public interface IExFont {

        /// <summary>
        /// �r��
        /// </summary>
        Font Font { get; set; }
        /// <summary>
        /// �r���W��
        /// </summary>
        string Name { get; }
        /// <summary>
        /// �r���˦�
        /// </summary>
        FontStyle Style { get; }
        /// <summary>
        /// �r���C��
        /// </summary>
        Color ForeColor { get; set; }
        /// <summary>
        /// �r�驳��
        /// </summary>
        Color BackColor { get; set; }

    }
    /// <summary>
    /// RichText��챱�
    /// </summary>
    public class DataGridViewRichTextBoxColumn : DataGridViewColumn
    {
        public DataGridViewRichTextBoxColumn()
            : base(new DataGridViewRichTextBoxCell())
        { 
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (!(value is DataGridViewRichTextBoxCell))
                    throw new InvalidCastException("CellTemplate must be a DataGridViewRichTextBoxCell");

                base.CellTemplate = value;  
            }
        }
    }

    /// <summary>
    /// RichText�x�s�汱�
    /// </summary>
    public class DataGridViewRichTextBoxCell : DataGridViewImageCell
    {
        private static readonly RichTextBox _editingControl = new RichTextBox();

        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewRichTextBoxEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
            set
            {
                base.ValueType = value;
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return typeof(string);
            }
        }

        private static void SetRichTextBoxText(RichTextBox ctl, string text)
        {
            try
            {
                ctl.Rtf = text;
            }
            catch (ArgumentException)
            {
                ctl.Text = text;
            }
        }

        private Image GetRtfImage(int rowIndex, object value, bool selected)
        {
            Size cellSize = GetSize(rowIndex);

            if (cellSize.Width < 1 || cellSize.Height < 1)
                return null;

            RichTextBox ctl = null;

            if (ctl == null)
            {
                ctl = _editingControl;
                ctl.Size = GetSize(rowIndex);
                SetRichTextBoxText(ctl, Convert.ToString(value));
            }

            if (ctl != null)
            {
                // Print the content of RichTextBox to an image.
                Size imgSize = new Size(cellSize.Width - 1, cellSize.Height - 1);
                Image rtfImg = null;

                if (selected)
                {
                    // Selected cell state
                    ctl.BackColor = DataGridView.DefaultCellStyle.SelectionBackColor;
                    ctl.ForeColor = DataGridView.DefaultCellStyle.SelectionForeColor;

                    // Print image
                    rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height);

                    // Restore RichTextBox
                    ctl.BackColor = DataGridView.DefaultCellStyle.BackColor;
                    ctl.ForeColor = DataGridView.DefaultCellStyle.ForeColor;
                }
                else
                {
                    rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width, imgSize.Height);
                }

                return rtfImg;
            }

            return null;
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is RichTextBox ctl) {
                SetRichTextBoxText(ctl, Convert.ToString(initialFormattedValue));
            }
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            return value;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);

            Image img = GetRtfImage(rowIndex, value, base.Selected);

            if (img != null)
                graphics.DrawImage(img, cellBounds.Left, cellBounds.Top);
        }

        #region Handlers of edit events, copyied from DataGridViewTextBoxCell

        private byte flagsState;

        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            base.OnEnter(rowIndex, throughMouseClick);

            if ((base.DataGridView != null) && throughMouseClick)
            {
                this.flagsState = (byte)(this.flagsState | 1);
            }
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            base.OnLeave(rowIndex, throughMouseClick);

            if (base.DataGridView != null)
            {
                this.flagsState = (byte)(this.flagsState & -2);
            }
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (base.DataGridView != null)
            {
                Point currentCellAddress = base.DataGridView.CurrentCellAddress;

                if (((currentCellAddress.X == e.ColumnIndex) && (currentCellAddress.Y == e.RowIndex)) && (e.Button == MouseButtons.Left))
                {
                    if ((this.flagsState & 1) != 0)
                    {
                        this.flagsState = (byte)(this.flagsState & -2);
                    }
                    else if (base.DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
                    {
                        base.DataGridView.BeginEdit(false);
                    }
                }
            }
        }

        public override bool KeyEntersEditMode(KeyEventArgs e)
        {
            return (((((char.IsLetterOrDigit((char)((ushort)e.KeyCode)) && ((e.KeyCode < Keys.F1) || (e.KeyCode > Keys.F24))) || ((e.KeyCode >= Keys.NumPad0) && (e.KeyCode <= Keys.Divide))) || (((e.KeyCode >= Keys.OemSemicolon) && (e.KeyCode <= Keys.OemBackslash)) || ((e.KeyCode == Keys.Space) && !e.Shift))) && (!e.Alt && !e.Control)) || base.KeyEntersEditMode(e));
        }

        #endregion
    }

    public class  DataGridViewRichTextBoxEditingControl : RichTextBox, IDataGridViewEditingControl
    {
        private DataGridView _dataGridView;
        private int _rowIndex;
        private bool _valueChanged;

        public DataGridViewRichTextBoxEditingControl()
        {
            this.BorderStyle = BorderStyle.None;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            _valueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keys = keyData & Keys.KeyCode;
            if (keys == Keys.Return)
            {
                return this.Multiline;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    // Control + B = Bold
                    case Keys.B:
                        if (this.SelectionFont.Bold)
                        {
                            this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold & this.Font.Style);
                        }
                        else
                            this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold | this.Font.Style);
                        break;
                    // Control + U = Underline
                    case Keys.U:
                        if (this.SelectionFont.Underline)
                        {
                            this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, ~FontStyle.Underline & this.Font.Style);
                        }
                        else
                            this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Underline | this.Font.Style);
                        break;
                    // Control + I = Italic
                    // Conflicts with the default shortcut
                    //case Keys.I:
                    //    if (this.SelectionFont.Italic)
                    //    {
                    //        this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, ~FontStyle.Italic & this.Font.Style);
                    //    }
                    //    else
                    //        this.SelectionFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Italic | this.Font.Style);
                    //    break;
                    default:
                        break;
                }
            }
        }

        #region IDataGridViewEditingControl Members

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        public DataGridView EditingControlDataGridView
        {
            get
            {
                return _dataGridView;
            }
            set
            {
                _dataGridView = value;
            }
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return this.Rtf;
            }
            set
            {
                if (value is string)
                    this.Text = value as string;
            }
        }

        public int EditingControlRowIndex
        {
            get
            {
                return _rowIndex;
            }
            set
            {
                _rowIndex = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return _valueChanged;
            }
            set
            {
                _valueChanged = value;
            }
        }
        
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch ((keyData & Keys.KeyCode))
            {
                case Keys.Return:
                    if ((((keyData & (Keys.Alt | Keys.Control | Keys.Shift)) == Keys.Shift) && this.Multiline))
                    {
                        return true;
                    }
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }

            return !dataGridViewWantsInputKey;
        }

        public Cursor EditingPanelCursor
        {
            get { return this.Cursor; }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Rtf;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        #endregion
    }

    /// <summary>
    /// �s�X����
    /// </summary>
    /// <remarks>
    /// �~��Encoding���O��@��H��k�B�X�iToHexs��k��CharSet�ݩ�
    /// </remarks>
    public abstract class BaseEncoding : Encoding {
        public override int CodePage { get { return mCodePage; } }

        public abstract int CharSet { get; }
        protected abstract int mCodePage { get; set; }

        public override int GetByteCount(char[] chars, int index, int count) {
            return GetEncoding(CodePage).GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) {
            return GetEncoding(CodePage).GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count) {
            return GetEncoding(CodePage).GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
            return GetEncoding(CodePage).GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override int GetMaxByteCount(int charCount) {
            return GetEncoding(CodePage).GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount) {
            return GetEncoding(CodePage).GetMaxCharCount(byteCount);
        }

        public string ToHexs(string data) {
            return GetBytes(data).ToHex();
        }
    }

    /// <summary>
    /// �s�X�w�A���ѦU�ؽs�X�榡
    /// </summary>
    public class CtEncoding : BaseEncoding {
        private static CtEncoding mANSI = null;
        private static CtEncoding mShiftJIS = null;
        private static CtEncoding mGB2312 = null;
        private static CtEncoding mBig5 = null;

        public static CtEncoding ANSI {
            get {
                if (mANSI == null) mANSI = new CtEncoding(1252, 0);
                return mANSI;
            }
        }
        public static CtEncoding ShiftJIS {
            get {
                if (mShiftJIS == null) mShiftJIS = new CtEncoding(932, 128);
                return mShiftJIS;
            }
        }

        public static CtEncoding GB2312 {
            get {
                if (mGB2312 == null) mGB2312 = new CtEncoding(936, 134);
                return mGB2312;
            }
        }

        public static CtEncoding Big5 {
            get {
                if (mBig5 == null) mBig5 = new CtEncoding(950, 136);
                return mBig5;
            }
        }

        public static CtEncoding GetEncoding(int page, int set) {
            return new CtEncoding(page, set);
        }

        public override int CharSet { get; } = 0;
        protected override int mCodePage { get; set; }
        private CtEncoding(int page, int set) {
            mCodePage = page;
            CharSet = set;
        }

    }

    /// <summary>
    /// ��нX�w�q
    /// </summary>
    public static class LocaleCode {
        /// <summary>
        /// �x�W�c��
        /// </summary>
        public const int ZhTw = 1028;
        /// <summary>
        /// ����²��
        /// </summary>
        public const int ZhCn = 2052;
        /// <summary>
        /// ����
        /// </summary>
        public const int EnUs = 1033;
    }

    /// <summary>
    /// RTF�����X�R��k
    /// </summary>
    public static class RtfExtension {

        /// <summary>
        /// �N<see cref="Color"/>�ഫ��RTF�榡�r��
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToRTF(this Color color) {
            return "\\red" + color.R.ToString() + "\\green" + color.G.ToString() + "\\blue" + color.B.ToString();
        }

        /// <summary>
        /// �ഫ�榡��\\'H1\\'H2\\'H3.......
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes) {
            char[] c = new char[bytes.Length * 4];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx) {
                c[cx] = '\\';

                c[++cx] = '\'';

                b = ((byte)(bytes[bx] >> 4));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

    }
    
    /// <summary>
    /// �r��˦��w�q���c
    /// </summary>
    public class ExFont :IExFont{

        /// <summary>
        /// �r��
        /// </summary>
        public Font Font { get; set; }

        public string Name { get { return Font.Name; } }

        public FontStyle Style { get { return Font.Style; } }

        /// <summary>
        /// �r���C��
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// �r�驳��
        /// </summary>
        public Color BackColor { get; set; }

        public ExFont (Font font,Color foreColor,Color backColor) {
            this.Font = font;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
        }

        public ExFont(string fontName,float fontSize,FontStyle fontStyle,Color foreColor,Color backColor) {
            this.Font = new Font(fontName,fontSize,fontStyle);
            this.ForeColor = foreColor;
            this.BackColor = backColor;   
        }
        
    }

    /// <summary>
    /// RTF�ഫ�u��
    /// </summary>
    public class RtfConvert {

        #region Declaration - Fields

        /// <summary>
        /// �r����ܵ���
        /// </summary>
        private FontDialog mFdlg = new FontDialog();

        /// <summary>
        /// �C���ܵ���
        /// </summary>
        private ColorDialog mCdlg = new ColorDialog();

        /// <summary>
        /// �O�_���ݩʳQ�ק�L
        /// </summary>
        private bool mIsModified = true;
        
        /// <summary>
        /// �@���r�榡
        /// </summary>
        private IExFont mRegular = new ExFont(new Font("�L�n������",12),Color.Black,Color.Empty);

        /// <summary>
        /// �аO��r�榡
        /// </summary>
        private IExFont mHighlight = new ExFont(new Font("�L�n������",12,FontStyle.Bold | FontStyle .Italic),Color.Blue,Color.Yellow);

        /// <summary>
        /// �s�X�榡
        /// </summary>
        private CtEncoding mEncoding = CtEncoding.GB2312;

        /// <summary>
        /// RTF���Y
        /// </summary>
        private StringBuilder mHeader = new StringBuilder();

        /// <summary>
        /// ����RTF
        /// </summary>
        private StringBuilder mFullRTF = new StringBuilder();

        /// <summary>
        /// �t�Φr���������
        /// </summary>
        private HybridDictionary rtfFontFamily;

        /// <summary>
        /// �r���w�q��
        /// </summary>
        private byte mFontCount = 0;

        private string mKeyWord = null;

        #endregion Declaration - Fields

        #region Declaration - Const

        /// <summary>
        /// �r�������w�q
        /// </summary>
        private struct RtfFontFamilyDef {
            public const string Unknown = @"\fnil";
            public const string Roman = @"\froman";
            public const string Swiss = @"\fswiss";
            public const string Modern = @"\fmodern";
            public const string Script = @"\fscript";
            public const string Decor = @"\fdecor";
            public const string Technical = @"\ftech";
            public const string BiDirect = @"\fbidi";
        }

        /// <summary>
        /// �����r��
        /// </summary>
        private const string FF_UNKNOWN = "UNKNOWN";

        #endregion Declaration - Const

        #region Declaration - Properties

        /// <summary>
        /// �@���r�˦��w�q
        /// </summary>
        public IExFont Regular {
            get {
                return mRegular;
            }set{
                if (value != null) {
                    mIsModified = true;
                    mRegular = value;
                }
            }
        }
        /// <summary>
        /// ����r�˦��w�q
        /// </summary>
        public IExFont Highlight {
            get {
                return mHighlight;
            }set {
                if (value != null) {
                    mIsModified = true;
                    mHighlight = value;
                }
            }
        }
        /// <summary>
        /// �s�X�榡
        /// </summary>
        public CtEncoding Encoding {
            get {
                return mEncoding;
            }set {
                mIsModified = true;
                mEncoding = value;
            }
        }
        public string KeyWord {
            get {
                return mKeyWord;
            }
            set {
                mKeyWord = value;
            }
        }
        #endregion Declaration - Properties

        #region Function - Construcotrs

        public RtfConvert() {
            /*-- ���o�r����Ӹ�� --*/
            rtfFontFamily = new HybridDictionary {
                { System.Drawing.FontFamily.GenericMonospace.Name, RtfFontFamilyDef.Modern },
                { System.Drawing.FontFamily.GenericSansSerif, RtfFontFamilyDef.Swiss },
                { System.Drawing.FontFamily.GenericSerif, RtfFontFamilyDef.Roman },
                { FF_UNKNOWN, RtfFontFamilyDef.Unknown }
            };
        }

        #endregion Function - Constructors

        #region Funciton - Public Methods

        public bool ShowColorDialog(out Color color) {
            color = Color.Empty;
            if (mCdlg.ShowDialog() == DialogResult.OK) {
                color = mCdlg.Color;
                return true;
            }
            return false;
        }

        public bool ShowFontDialog(out Font font) {
            font = null;
            if (mFdlg.ShowDialog() == DialogResult.OK) {
                font = mFdlg.Font;
                return true;
            }
            return false;            
        }

        /// <summary>
        /// �N��l�r���ഫ��RTF�榡�r��
        /// </summary>
        /// <param name="data">��l�r��</param>
        /// <param name="keyWord">�аO�r��</param>
        /// <returns></returns>
        public string ToRTF(string data) {
            return ToRTF(data, mRegular);
        }

        public string ToRTF(string data,IExFont rgFont = null,bool highlight = true) {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            if (mIsModified || rgFont != null) {
                mHeader.Clear();
                ConstructorRtfHeader(rgFont ?? mRegular,mHighlight );
            }
            mFullRTF.Clear();
            mFullRTF.Append(mHeader);
            string keyWord = highlight ? mKeyWord : null;
            RtfContent(data,  keyWord, rgFont, mHighlight);

            return mFullRTF.ToString();

        }

        #endregion Funciotn - Public Mehtods

        #region Funciton - Private Mehtods

        /// <summary>
        /// ���s����RTF���Y��r
        /// </summary>
        private void ConstructorRtfHeader(IExFont rgFont,IExFont hlFont) {
            mHeader.Clear();
            mHeader.Append(@"{\rtf1\ansi\ansicpg" + mEncoding.CodePage.ToString() + @"\deff0");//�]�w�s�X��
            DefineTable(rgFont,hlFont);
        }

        /// <summary>
        /// RTF�ѼƩw�q
        /// </summary>
        private void DefineTable(IExFont rgFont,IExFont hlFont) {
            /*-- �C��w�q --*/
            mHeader.Append(@"{\colortbl;");
            mHeader.Append(rgFont.ForeColor.ToRTF());//�@���r�C��
            mHeader.Append(";");
            mHeader.Append(rgFont.BackColor.ToRTF());//�@���r����
            mHeader.Append(";");
            mHeader.Append(hlFont.ForeColor.ToRTF());//�аO��r�C��
            mHeader.Append(";");
            mHeader.Append(hlFont.BackColor.ToRTF());//�ФΤ�r����
            mHeader.Append(";}");

            /*-- �r���w�q --*/
            mFontCount = 0;//�r���`���k�s
            mHeader.Append(@"{\fonttbl");
            FontFamily(rgFont.Font);//�@���r�r��
            FontFamily(hlFont.Font);//�аO��r�r��
            mHeader.Append("}");
        }

        /// <summary>
        /// �r�Ŷ� & �r���]�w
        /// </summary>
        /// <param name="font">�]�w�r��</param>
        private void FontFamily(Font font) {
            mHeader.Append(@"{\f");
            mHeader.Append(mFontCount.ToString());//�r���ƽs��
            /*-- �r������ --*/
            if (rtfFontFamily.Contains(font.FontFamily.Name)) {
                mHeader.Append(rtfFontFamily[font.FontFamily.Name]);
            } else {
                mHeader.Append(rtfFontFamily[FF_UNKNOWN]);
            }
            mHeader.Append(@"\fcharset");
            mHeader.Append(mEncoding.CharSet.ToString());//�r�Ŷ�
            mHeader.Append(" ");
            mHeader.Append(mEncoding.ToHexs(font.Name));//�r���W��
            mHeader.Append(";}");
            mFontCount += 1;
        }
        
        /// <summary>
        /// �r���˦��ഫ��RTF�榡
        /// </summary>
        /// <param name="font">�]�w�r��</param>
        /// <returns></returns>
        private string RtfFontStyle(Font font) {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(@"\fs" + Math.Round((font.Size * 2),0).ToString());//�r��j�p
            if (font.Bold) tmp.Append(@"\b");
            if (font.Italic) tmp.Append(@"\i");
            if (font.Strikeout) tmp.Append(@"\strike");
            if (font.Underline) tmp.Append(@"\ul");
            return tmp.ToString();
        }

        /// <summary>
        /// �N��l��r�ഫ��RTF�榡�[�J<see cref="mFullRTF"/>
        /// </summary>
        /// <param name="data">��l��r</param>
        /// <param name="keyWord">�аO��r</param>
        private void RtfContent(string data,string keyWord,IExFont rgFont,IExFont hlFont) {
            mFullRTF.Append(@"\f0");//�@��r��
            if (rgFont.ForeColor != Color.Empty) mFullRTF.Append(@"\cf1");//�@��r���C��N��
            if (rgFont.BackColor != Color.Empty) mFullRTF.Append(@"\highlight2");//�@��r�驳��N��
            mFullRTF.Append(RtfFontStyle(rgFont.Font));//�@���r�˦�
            string hData = mEncoding.ToHexs(data);
            /*--  �аO��r --*/
            if (!string.IsNullOrEmpty(keyWord)) {
                string hKeyWord = mEncoding.ToHexs(keyWord);
                if (hData.Contains(hKeyWord)) {
                    StringBuilder highlight = new StringBuilder();
                    highlight.Append(@"{\f1");//�аO�r��
                    if (hlFont.ForeColor != null) highlight.Append(@"\cf3");//�аO�r���C��N��
                    if (hlFont.BackColor != null) highlight.Append(@"\highlight4");//�аO��r����N��
                    highlight.Append(RtfFontStyle(mHighlight.Font));//�аO��r�˦�
                    highlight.Append(hKeyWord);
                    highlight.Append("}");
                    hData = hData.Replace(hKeyWord, highlight.ToString());//�N����r�˦����N
                }
            }
            mFullRTF.Append(hData);
            mFullRTF.Append("}");
        }

        #endregion Funciton - Private Methods

    }

    /// <summary>
    /// �t�Φr��Ū����
    /// </summary>
    public static class CtFontFamily {

        /// <summary>
        /// �t�Φr���������
        /// </summary>
        private static HybridDictionary rtfFontFamily = new HybridDictionary() {
            { FontFamily.GenericMonospace.Name, RtfFontFamilyDef.Modern },
            { FontFamily.GenericSansSerif, RtfFontFamilyDef.Swiss },
            {FontFamily.GenericSerif, RtfFontFamilyDef.Roman },
            {FF_UNKNOWN, RtfFontFamilyDef.Unknown }
        };
        
        /// <summary>
        /// �r�������w�q
        /// </summary>
        private struct RtfFontFamilyDef {
            public const string Unknown = @"\fnil";
            public const string Roman = @"\froman";
            public const string Swiss = @"\fswiss";
            public const string Modern = @"\fmodern";
            public const string Script = @"\fscript";
            public const string Decor = @"\fdecor";
            public const string Technical = @"\ftech";
            public const string BiDirect = @"\fbidi";
        }

        /// <summary>
        /// �����r��
        /// </summary>
        private const string FF_UNKNOWN = "UNKNOWN";

        public static object Mapping(string fontFamilyName) {
            return rtfFontFamily.Contains(fontFamilyName) ?
                rtfFontFamily[fontFamilyName] :
                rtfFontFamily[FF_UNKNOWN];            
        }
    }

    public class Factory {

        public DataGridViewColumn GetTextColumn(string colName, Font font) {
            return ConfigColumn(new DataGridViewTextBoxColumn(), colName, font);
        }

        public DataGridViewColumn GetRichTextColumn(string colName) {
            return ConfigColumn(new DataGridViewRichTextBoxColumn(), colName);
        }

        private DataGridViewColumn ConfigColumn(DataGridViewColumn column, string colName, Font font = null) {
            column.Name = colName;
            column.DataPropertyName = colName;
            column.HeaderText = colName;
            if (font != null) column.DefaultCellStyle.Font = font;
            return column;
        }
    }

}
