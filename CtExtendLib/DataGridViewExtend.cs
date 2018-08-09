using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;

namespace CtExtendLib
{
	public static class DataGridViewExtendTool
	{
		public static void DoubleMode(this DataGridView dgv, bool mode)
		{
			Type dgvType = dgv.GetType();
			PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dgv, mode, null);
		}
		public static string RtfToString(this string rtf)
		{
			switch (rtf.StartsWith(@"{\rtf"))
			{
				case true:
					return (new RichTextBox() { Rtf = rtf }).Text;
				default:
					return rtf;
			}
		}
	}

	public class RichTextBoxColumn : DataGridViewColumn
	{
		public RichTextBoxColumn() : base(new RichTextBoxCell()) { }

		public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !value.GetType().IsAssignableFrom(typeof(RichTextBoxCell)))
				{
					throw new InvalidCastException("Must be a CalendarCell");
				}
				base.CellTemplate = value;
			}
		}

	}

	public class RichTextBoxCell : DataGridViewTextBoxCell
	{
		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
			RichTexBoxEditingContoler ctl = DataGridView.EditingControl as RichTexBoxEditingContoler;
			if (initialFormattedValue.ToString().StartsWith(@"{\rtf")) ctl.Rtf = initialFormattedValue.ToString();
			else ctl.Text = initialFormattedValue.ToString();
		}

		public override Type EditType => typeof(RichTexBoxEditingContoler);
		public override Type ValueType { get => typeof(string); set => base.ValueType = value; }
		public override Type FormattedValueType => typeof(string);
		public override object DefaultNewRowValue => string.Empty;

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, paintParts);
			if (value == null) return;
			Size cellSize = cellBounds.Size;
			RichTextBox ctl = new RichTextBox() { Size = cellSize };
			if (value.ToString().StartsWith(@"{\rtf")) ctl.Rtf = value.ToString();
			else ctl.Text = value.ToString();
			ctl.BorderStyle = BorderStyle.None;
			ctl.Margin = new Padding(1, 0, 1, 0);
			ctl.BackColor = base.Selected ? DataGridView.DefaultCellStyle.SelectionBackColor : DataGridView.DefaultCellStyle.BackColor;
			ctl.ForeColor = base.Selected ? DataGridView.DefaultCellStyle.SelectionForeColor : DataGridView.DefaultCellStyle.ForeColor;

			Image img = RichTextBoxPrinter.Print(ctl, ctl.Width - 2, ctl.Height);


			graphics.DrawImage(img, cellBounds.Left + 1, cellBounds.Top);
		}
	}

	public class RichTexBoxEditingContoler : RichTextBox, IDataGridViewEditingControl
	{
		DataGridView dgv;
		private bool valueChange = false;
		int rowIndex;

		public RichTexBoxEditingContoler()
		{
			this.BorderStyle = BorderStyle.None;
			this.Multiline = false;
			this.Margin = new Padding(1, 0, 1, 0);
		}

		public DataGridView EditingControlDataGridView { get => dgv; set => dgv = value; }
		public object EditingControlFormattedValue
		{
			get => this.Text;
			set
			{
				if (value is string)
				{
					if (value.ToString().StartsWith(@"{\rtf")) this.Rtf = value.ToString();
					else this.Text = value.ToString();
				}
				else this.Text = "";
			}
		}
		public int EditingControlRowIndex { get => rowIndex; set => rowIndex = value; }
		public bool EditingControlValueChanged { get => valueChange; set => valueChange = value; }

		public Cursor EditingPanelCursor => base.Cursor;

		public bool RepositionEditingControlOnValueChange => false;

		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			this.Font = dataGridViewCellStyle.Font;
			this.ForeColor = dataGridViewCellStyle.ForeColor;
			this.BackColor = dataGridViewCellStyle.BackColor;
		}

		public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
				case Keys.PageDown:
				case Keys.PageUp:
					return true;
				default:
					return !dataGridViewWantsInputKey;
			}
		}

		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return EditingControlFormattedValue;
		}

		public void PrepareEditingControlForEdit(bool selectAll)
		{

		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			valueChange = true;
			this.EditingControlDataGridView.NotifyCurrentCellDirty(true);	
		}
	}

	/// <summary>
	/// RichText欄位控制項
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
	/// RichText儲存格控制項
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
				ctl.Margin = new Padding(1, 0, 1, 0);
				SetRichTextBoxText(ctl, Convert.ToString(value));
			}

			if (ctl != null)
			{
				// Print the content of RichTextBox to an image.
				Size imgSize = new Size(cellSize.Width , cellSize.Height );
				Image rtfImg = null;

				if (selected)
				{
					// Selected cell state
					ctl.BackColor = DataGridView.DefaultCellStyle.SelectionBackColor;
					ctl.ForeColor = DataGridView.DefaultCellStyle.SelectionForeColor;

					// Print image
					rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width-5, imgSize.Height);

					// Restore RichTextBox
					ctl.BackColor = DataGridView.DefaultCellStyle.BackColor;
					ctl.ForeColor = DataGridView.DefaultCellStyle.ForeColor;
				}
				else
				{
					rtfImg = RichTextBoxPrinter.Print(ctl, imgSize.Width-5, imgSize.Height);
				}

				return rtfImg;
			}

			return null;
		}

		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

			if (DataGridView.EditingControl is RichTextBox ctl)
			{
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

	public class DataGridViewRichTextBoxEditingControl : RichTextBox, IDataGridViewEditingControl
	{
		private DataGridView _dataGridView;
		private int _rowIndex;
		private bool _valueChanged;

		public DataGridViewRichTextBoxEditingControl()
		{
			this.BorderStyle = BorderStyle.None;
			this.Multiline = false;
			this.Margin = new Padding(1, 0, 1, 0);
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
	/// http://support.microsoft.com/default.aspx?scid=kb;en-us;812425
	/// The RichTextBox control does not provide any method to print the content of the RichTextBox. 
	/// You can extend the RichTextBox class to use EM_FORMATRANGE message 
	/// to send the content of a RichTextBox control to an output device such as printer.
	/// </summary>
	public class RichTextBoxPrinter
	{
		//Convert the unit used by the .NET framework (1/100 inch) 
		//and the unit used by Win32 API calls (twips 1/1440 inch)
		private const double anInch = 14.4;

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct CHARRANGE
		{
			public int cpMin;         //First character of range (0 for start of doc)
			public int cpMax;           //Last character of range (-1 for end of doc)
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct FORMATRANGE
		{
			public IntPtr hdc;             //Actual DC to draw on
			public IntPtr hdcTarget;       //Target DC for determining text formatting
			public RECT rc;                //Region of the DC to draw to (in twips)
			public RECT rcPage;            //Region of the whole DC (page size) (in twips)
			public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
		}

		private const int WM_USER = 0x0400;
		private const int EM_FORMATRANGE = WM_USER + 57;

		[DllImport("USER32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		// Render the contents of the RichTextBox for printing
		//	Return the last character printed + 1 (printing start from this point for next page)
		public static int Print(IntPtr richTextBoxHandle, int charFrom, int charTo, PrintPageEventArgs e)
		{
			//Calculate the area to render and print
			RECT rectToPrint;
			rectToPrint.Top = (int)(e.MarginBounds.Top * anInch);
			rectToPrint.Bottom = (int)(e.MarginBounds.Bottom * anInch);
			rectToPrint.Left = (int)(e.MarginBounds.Left * anInch);
			rectToPrint.Right = (int)(e.MarginBounds.Right * anInch);

			//Calculate the size of the page
			RECT rectPage;
			rectPage.Top = (int)(e.PageBounds.Top * anInch);
			rectPage.Bottom = (int)(e.PageBounds.Bottom * anInch);
			rectPage.Left = (int)(e.PageBounds.Left * anInch);
			rectPage.Right = (int)(e.PageBounds.Right * anInch);

			IntPtr hdc = e.Graphics.GetHdc();

			FORMATRANGE fmtRange;
			fmtRange.chrg.cpMax = charTo;               //Indicate character from to character to 
			fmtRange.chrg.cpMin = charFrom;
			fmtRange.hdc = hdc;                    //Use the same DC for measuring and rendering
			fmtRange.hdcTarget = hdc;              //Point at printer hDC
			fmtRange.rc = rectToPrint;             //Indicate the area on page to print
			fmtRange.rcPage = rectPage;            //Indicate size of page

			IntPtr res = IntPtr.Zero;

			IntPtr wparam = IntPtr.Zero;
			wparam = new IntPtr(1);

			//Get the pointer to the FORMATRANGE structure in memory
			IntPtr lparam = IntPtr.Zero;
			lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
			Marshal.StructureToPtr(fmtRange, lparam, false);

			//Send the rendered data for printing 
			res = SendMessage(richTextBoxHandle, EM_FORMATRANGE, wparam, lparam);

			//Free the block of memory allocated
			Marshal.FreeCoTaskMem(lparam);

			//Release the device context handle obtained by a previous call
			e.Graphics.ReleaseHdc(hdc);

			// Release and cached info
			SendMessage(richTextBoxHandle, EM_FORMATRANGE, (IntPtr)0, (IntPtr)0);

			//Return last + 1 character printer
			return res.ToInt32();
		}

		public static Image Print(RichTextBox ctl, int width, int height)
		{
			Image img = new Bitmap(width, height);
			float scale;

			using (Graphics g = Graphics.FromImage(img))
			{
				// --- Begin code addition D_Kondrad

				// HorizontalResolution is measured in pix/inch         
				scale = (float)(width * 100) / img.HorizontalResolution;
				width = (int)scale;

				// VerticalResolution is measured in pix/inch
				scale = (float)(height * 100) / img.VerticalResolution;
				height = (int)scale;

				// --- End code addition D_Kondrad

				Rectangle marginBounds = new Rectangle(0, 0, width, height);
				Rectangle pageBounds = new Rectangle(0, 0, width, height);
				PrintPageEventArgs args = new PrintPageEventArgs(g, marginBounds, pageBounds, null);

				Print(ctl.Handle, 0, ctl.Text.Length, args);
			}

			return img;
		}

	}
}
