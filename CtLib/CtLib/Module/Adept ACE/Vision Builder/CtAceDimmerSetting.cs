using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ace.HSVision.Server.Tools;

using CtLib.Library;
using CtLib.Forms;
using CtLib.Module.Dimmer;
using CtLib.Module.SerialPort;
using CtLib.Module.Utility;
using CtLib.Module.XML;

namespace CtLib.Module.Adept {
	/// <summary>提供使用者介面以調試調光器組別與其參數</summary>
	public partial class CtAceDimmerSetting : Form {

		#region Fields
		private string mCurSelc = string.Empty;
		private bool mFlag_Modified = false;
		private bool mFlag_Editing = false;
		private UILanguage mLang = UILanguage.TraditionalChinese;
		private List<DimmerPack> rLights;
		private List<DimmerPack> mLights;
		private List<DimmerPack> mDimAdd = new List<DimmerPack>();
		private List<string> mDimDel = new List<string>();
		private Dictionary<DimmerBrand, Dictionary<UILanguage, string>> mBrandDef = new Dictionary<DimmerBrand, Dictionary<UILanguage, string>> {
			{
				DimmerBrand.HJ_CH5I700RS232,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Hong Ji [5CH 700mA RS232]" },
					{ UILanguage.SimplifiedChinese, "弘积 [5CH 700mA RS232]" },
					{ UILanguage.TraditionalChinese, "弘積 [5CH 700mA RS232]" }
				}
			},
			{
				DimmerBrand.QUAN_DA,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Quan Da [2/4/5CH 500mA RS232]" },
					{ UILanguage.SimplifiedChinese, "荃达 [2/4/5CH 500mA RS232]" },
					{ UILanguage.TraditionalChinese, "荃達 [2/4/5CH 500mA RS232]" }
				}
			},
			{
				DimmerBrand.HJ_CH4I700RS485,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Hong Ji [4CH 700mA RS485]" },
					{ UILanguage.SimplifiedChinese, "弘积 [4CH 700mA RS485]" },
					{ UILanguage.TraditionalChinese, "弘積 [4CH 700mA RS485]" }
				}
			}
		};
		private Dictionary<LightStyle, Dictionary<UILanguage, string>> mStyleDef = new Dictionary<LightStyle, Dictionary<UILanguage, string>> {
			{
				LightStyle.Circle,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Circle" },
					{ UILanguage.SimplifiedChinese, "环形光" },
					{ UILanguage.TraditionalChinese, "環形光" }
				}
			},
			{
				LightStyle.Coaxial,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Coaxial" },
					{ UILanguage.SimplifiedChinese, "同轴光" },
					{ UILanguage.TraditionalChinese, "同軸光" }
				}
			},
			{
				LightStyle.Fluorescent,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Fluorescent" },
					{ UILanguage.SimplifiedChinese, "日光灯" },
					{ UILanguage.TraditionalChinese, "日光燈" }
				}
			},
			{
				LightStyle.Strip,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Strip" },
					{ UILanguage.SimplifiedChinese, "条状光" },
					{ UILanguage.TraditionalChinese, "條狀光" }
				}
			},
			{
				LightStyle.Undefined,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Empty" },
					{ UILanguage.SimplifiedChinese, "無" },
					{ UILanguage.TraditionalChinese, "無" }
				}
			}
		};

		private Dictionary<LightColor, Dictionary<UILanguage, string>> mColorDef = new Dictionary<LightColor, Dictionary<UILanguage, string>> {
			{
				LightColor.Blue,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Blue" },
					{ UILanguage.SimplifiedChinese, "蓝" },
					{ UILanguage.TraditionalChinese, "藍" }
				}
			},
			{
				LightColor.Red,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Red" },
					{ UILanguage.SimplifiedChinese, "红" },
					{ UILanguage.TraditionalChinese, "紅" }
				}
			},
			{
				LightColor.White,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "White" },
					{ UILanguage.SimplifiedChinese, "白" },
					{ UILanguage.TraditionalChinese, "白" }
				}
			},
			{
				LightColor.Undefined,
				new Dictionary<UILanguage, string> {
					{ UILanguage.English, "Unknown" },
					{ UILanguage.SimplifiedChinese, "無" },
					{ UILanguage.TraditionalChinese, "無" }
				}
			}
		};

		private Dictionary<DimmerBrand, int> mChDef = new Dictionary<DimmerBrand, int> {
			{ DimmerBrand.HJ_CH4I700RS485, 4 }, { DimmerBrand.HJ_CH5I700RS232, 5 }, { DimmerBrand.QUAN_DA, 4 }
		};
		#endregion

		#region Constructors
		/// <summary>建構設定視窗</summary>
		/// <param name="lang">欲顯示的語言</param>
		/// <param name="lights">當前所載入的調光器集合</param>
		public CtAceDimmerSetting(UILanguage lang, List<DimmerPack> lights) {
			InitializeComponent();

			mLang = lang;
			rLights = lights;
			mLights = lights.ConvertAll(val => val.Clone());

			AddComboBox(lang);
			LoadDimmers(mLights);

			lbDimmerName.Text = string.Empty;
		}
		#endregion

		#region Private Methods
		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private string GetMultiLangText(string key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", mLang, key)[key];
		}

		/// <summary>取得特定關鍵字的文化語系，使用當前的 <see cref="System.Threading.Thread.CurrentUICulture"/></summary>
		/// <param name="key">欲比對的關鍵字</param>
		/// <returns>ID 之文字</returns>
		private Dictionary<string, string> GetMultiLangText(params string[] key) {
			return CtLanguage.GetMultiLangXmlText("Language.xml", mLang, key);
		}

		private void AddComboBox(UILanguage lang) {
			dgvChannel.InvokeIfNecessary(
				() => {
					dgvChannel.Rows.Clear();

					colStyle.Items.Clear();
					foreach (LightStyle style in Enum.GetValues(typeof(LightStyle))) {
						if (mStyleDef.ContainsKey(style)) {
							colStyle.Items.Add(mStyleDef[style][lang]);
						}
					}

					colColor.Items.Clear();
					foreach (LightColor color in Enum.GetValues(typeof(LightColor))) {
						if (mColorDef.ContainsKey(color)) {
							colColor.Items.Add(mColorDef[color][lang]);
						}
					}

					colCh.Items.Clear();
					foreach (string name in Enum.GetNames(typeof(Channels))) {
						colCh.Items.Add(name);
					}
				}
			);
		}

		private void LoadDimmers(List<DimmerPack> lights) {
			mCurSelc = string.Empty;
			CtInvoke.DataGridViewClear(dgvChannel);
			CtInvoke.ControlText(lbDimmerName, string.Empty);
			List<DataGridViewRow> rowColl = lights.ConvertAll(pack => CreateDimmerRow(dgvDimmer, pack));
			dgvDimmer.InvokeIfNecessary(
				() => {
					dgvDimmer.Rows.Clear();
					dgvDimmer.Rows.AddRange(rowColl.ToArray());
				}
			);
			if (lights.Count > 0) CtInvoke.ControlVisible(btnExport, true);
		}

		private void LoadLights(DimmerPack pack) {
			mCurSelc = pack.Name;
			CtInvoke.ControlText(lbDimmerName, mCurSelc);
			List<DataGridViewRow> chColl = pack.Channels.ConvertAll(light => CreateLightRow(dgvChannel, light));
			dgvChannel.InvokeIfNecessary(
				() => {
					dgvChannel.Rows.Clear();
					dgvChannel.Rows.AddRange(chColl.ToArray());
				}
			);
			mFlag_Modified = false;
			tsk_ChEditMode(false);
		}

		private void tsk_ChEditMode(bool edit) {
			mFlag_Editing = edit;
			CtInvoke.ControlVisible(btnEdit, !edit && !string.IsNullOrEmpty(mCurSelc));
			CtInvoke.ControlVisible(btnEditDim, !edit);
			CtInvoke.ControlVisible(btnAdd, edit);
			CtInvoke.ControlVisible(btnDelete, edit);
			CtInvoke.ControlVisible(btnSave, edit);
			CtInvoke.ControlVisible(btnLeave, edit);
			dgvChannel.InvokeIfNecessary(
				() => {
					colStyle.DisplayStyle = edit ? DataGridViewComboBoxDisplayStyle.DropDownButton : DataGridViewComboBoxDisplayStyle.Nothing;
					colColor.DisplayStyle = edit ? DataGridViewComboBoxDisplayStyle.DropDownButton : DataGridViewComboBoxDisplayStyle.Nothing;
					colStyle.ReadOnly = !edit;
					colColor.ReadOnly = !edit;
					colCmt.ReadOnly = !edit;
				}
			);
		}

		private void tsk_DimEditMode(bool edit) {
			mFlag_Editing = edit;
			CtInvoke.ControlVisible(btnEditDim, !edit);
			CtInvoke.ControlVisible(btnEdit, false);
			CtInvoke.ControlVisible(btnAddDim, edit);
			CtInvoke.ControlVisible(btnDeleteDim, edit);
			CtInvoke.ControlVisible(btnSaveDim, edit);
			CtInvoke.ControlVisible(btnLeaveDim, edit);
		}

		private DataGridViewRow CreateDimmerRow(DataGridView dgv, DimmerPack ch) {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv);
			row.Cells[colName.Index].Value = ch.Name;
			row.Cells[colBrand.Index].Value = mBrandDef[ch.Brand][mLang];
			row.Cells[colPort.Index].Value = ch.Port;
			return row;
		}

		private DataGridViewRow CreateLightRow(DataGridView dgv, DimmerChannel ch) {
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(dgv);
			row.Cells[colStyle.Index].Value = mStyleDef[ch.LightShape][mLang];
			row.Cells[colColor.Index].Value = mColorDef[ch.EmitColor][mLang];
			row.Cells[colCh.Index].Value = ch.Channel.ToString();
			row.Cells[colCmt.Index].Value = ch.Comment;
			return row;
		}

		private DimmerBrand GetDimmerBrand(string str) {
			return mBrandDef.FirstOrDefault(brdKvp => brdKvp.Value.ContainsValue(str)).Key;
		}

		private LightStyle GetLightStyle(string str) {
			return mStyleDef.FirstOrDefault(stlKvp => stlKvp.Value.ContainsValue(str)).Key;
		}

		private LightColor GetLightColor(string str) {
			return mColorDef.FirstOrDefault(clrKvp => clrKvp.Value.ContainsValue(str)).Key;
		}

		private Channels GetLightChannel(string str) {
			return (Channels)Enum.Parse(typeof(Channels), str);
		}

		private void SaveLights() {
			/* 把 DataGridViewRow 轉回 DimmerChannel */
			DimmerPack pack = mLights.Find(val => val.Name == mCurSelc);
			if (pack != null) {
				pack.ClearChannels();
				foreach (DataGridViewRow row in dgvChannel.Rows) {
					DimmerChannel ch = new DimmerChannel();
					ch.Channel = GetLightChannel(row.Cells[colCh.Index].Value.ToString());
					ch.EmitColor = GetLightColor(row.Cells[colColor.Index].Value.ToString());
					ch.LightShape = GetLightStyle(row.Cells[colStyle.Index].Value.ToString());
					ch.Comment = row.Cells[colCmt.Index].Value.ToString();
					ch.Port = pack.Port;
					pack.AddChannels(ch);
				}

				/* 寫回全域 */
				DimmerPack refPack = rLights.Find(val => val.Name == mCurSelc);
				if (refPack != null) {
					refPack.ClearChannels();
					refPack.AssignChannels(pack.Channels);
				}
			}

			mFlag_Modified = false;
			tsk_ChEditMode(false);

			CtInvoke.ControlVisible(btnExport, true);
		}

		private void SaveDimmers() {
			if (mDimAdd.Count > 0) {
				mLights.AddRange(mDimAdd);
			}

			if (mDimDel.Count > 0) {
				mLights.RemoveAll(val => mDimDel.Contains(val.Name));
			}

			rLights.Clear();
			rLights.AddRange(mLights.ConvertAll(val => val.Clone()));

			mFlag_Modified = false;
			tsk_DimEditMode(false);

			mDimAdd.Clear();
			mDimDel.Clear();

			CtInvoke.ControlVisible(btnExport, true);
		}

		#endregion

		#region Interface Events

		private void btnEdit_Click(object sender, EventArgs e) {
			if (!string.IsNullOrEmpty(mCurSelc)) {
				tsk_ChEditMode(true);
			}
		}

		private void btnLeave_Click(object sender, EventArgs e) {
			if (mFlag_Modified) {
				Dictionary<string, string> msg = GetMultiLangText("ExitTit", "ExitMsg");
				MsgBoxBtn btn = CtMsgBox.Show(
					msg["ExitTit"],
					msg["ExitMsg"],
					MsgBoxBtn.YesNo | MsgBoxBtn.Cancel,
					MsgBoxStyle.Question
				);
				if (btn == MsgBoxBtn.Yes) SaveLights();
				else if (btn == MsgBoxBtn.Cancel) return;
			}
			LoadDimmers(mLights);
			mFlag_Modified = false;
			tsk_ChEditMode(false);
		}

		private void btnAdd_Click(object sender, EventArgs e) {
			string chStr = string.Empty;
			string[] chColl = Enum.GetNames(typeof(Channels));
			Dictionary<string, string> msg = GetMultiLangText("DimChTit", "DimChMsg", "DimChInv", "DimStyTit", "DimStyMsg", "DimClrTit", "DimClrMsg");
			Stat stt = CtInput.ComboBoxList(out chStr, msg["DimChTit"], msg["DimChMsg"], chColl, chColl.ElementAt(0));
			if (stt != Stat.SUCCESS) return;
			bool repeat = false;
			foreach (DataGridViewRow row in dgvChannel.Rows) {
				if (row.Cells[colCh.Index].Value.ToString() == chStr) {
					repeat = true;
					break;
				}
			}
			if (repeat) {
				CtMsgBox.Show(msg["DimChTit"], msg["DimChInv"], MsgBoxBtn.OK, MsgBoxStyle.Error);
				return;
			}

			string styleStr = string.Empty;
			IEnumerable<string> styleColl = mStyleDef.Values.Select(kvp => kvp[mLang]);
			stt = CtInput.ComboBoxList(out styleStr, msg["DimStyTit"], msg["DimStyMsg"], styleColl, styleColl.ElementAt(0));
			if (stt != Stat.SUCCESS) return;
			
			string colorStr = string.Empty;
			IEnumerable<string> colorColl = mColorDef.Values.Select(kvp => kvp[mLang]);
			stt = CtInput.ComboBoxList(out colorStr, msg["DimClrTit"], msg["DimClrMsg"], colorColl, colorColl.ElementAt(0));
			if (stt != Stat.SUCCESS) return;
			
			DimmerPack pack = mLights.Find(dim => dim.Name == mCurSelc);
			DimmerChannel ch = new DimmerChannel() {
				Channel = GetLightChannel(chStr),
				Port = pack.Port,
				EmitColor = GetLightColor(colorStr),
				LightShape = GetLightStyle(styleStr)
			};

			dgvChannel.BeginInvokeIfNecessary(() => dgvChannel.Rows.Add(CreateLightRow(dgvChannel, ch)));
		}

		private void btnDelete_Click(object sender, EventArgs e) {
			if (dgvChannel.SelectedCells.Count > 0) {
				int rowIdx = dgvChannel.SelectedCells[0].RowIndex;
				DataGridViewRow row = dgvChannel.Rows[rowIdx];

				DimmerPack pack = mLights.Find(dim => dim.Name == mCurSelc);
				DimmerChannel ch = pack.Channels.Find(
					val =>
						mStyleDef[val.LightShape][mLang] == row.Cells[colStyle.Index].Value.ToString() &&
						mColorDef[val.EmitColor][mLang] == row.Cells[colColor.Index].Value.ToString() &&
						val.Channel.ToString() == row.Cells[colCh.Index].Value.ToString()
				);

				if (ch != null) {
					Dictionary<string, string> msg = GetMultiLangText("DimRmTit", "DimRmMsg");
					MsgBoxBtn btn = CtMsgBox.Show(
						msg["DimRmTit"],
						string.Format(msg["DimRmMsg"], ch.ToString()),
						MsgBoxBtn.YesNo,
						MsgBoxStyle.Question
					);
					if (btn == MsgBoxBtn.Yes) {
						dgvChannel.BeginInvokeIfNecessary(() => dgvChannel.Rows.RemoveAt(rowIdx));
					}
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e) {
			SaveLights();
		}

		private void btnExit_Click(object sender, EventArgs e) {
			if (mFlag_Modified) {
				Dictionary<string, string> msg = GetMultiLangText("ExitTit", "ExitMsg");
				MsgBoxBtn btn = CtMsgBox.Show(
					msg["ExitTit"],
					msg["ExitMsg"],
					MsgBoxBtn.YesNo | MsgBoxBtn.Cancel,
					MsgBoxStyle.Question
				);
				if (btn == MsgBoxBtn.Yes) {
					SaveDimmers();
					SaveLights();
				} else if (btn == MsgBoxBtn.Cancel) return;
			}
			CtInvoke.FormClose(this);
		}

		private void dgvLight_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
			mFlag_Modified = true;
		}

		private void btnAddDim_Click(object sender, EventArgs e) {
			string nameStr;
			string defName = "DIMMER " + CtConvert.ToHex(DateTime.Now.ToBinary());
			Dictionary<string, string> msg = GetMultiLangText("DimNamTit", "DimNamMsg", "DimBrndTit", "DimBrndMsg", "DimPortTit", "DimPortMsg", "ValEditInv", "StrValInv");
			Stat stt = CtInput.Text(out nameStr, msg["DimNamTit"], msg["DimNamMsg"], defName);
			if (stt != Stat.SUCCESS) return;
			if (string.IsNullOrEmpty(nameStr)) {
				CtMsgBox.Show(msg["ValEditInv"], msg["StrValInv"], MsgBoxBtn.OK, MsgBoxStyle.Error);
				return;
			}

			string brandStr = string.Empty;
			IEnumerable<string> brandColl = mBrandDef.Values.Select(kvp => kvp[mLang]);
			stt = CtInput.ComboBoxList(out brandStr, msg["DimBrndTit"], msg["DimBrndMsg"], brandColl, mBrandDef[DimmerBrand.QUAN_DA][mLang]);
			if (stt != Stat.SUCCESS) return;

			string portStr = string.Empty;
			List<string> portColl = CtSerial.GetPortNames();
			stt = CtInput.ComboBoxList(out portStr, msg["DimPortTit"], msg["DimPortMsg"], portColl, "COM1", true);
			if (stt != Stat.SUCCESS) return;
			if (string.IsNullOrEmpty(nameStr)) {
				CtMsgBox.Show(msg["ValEditInv"], msg["StrValInv"], MsgBoxBtn.OK, MsgBoxStyle.Error);
				return;
			}

			List<DimmerChannel> chColl = new List<DimmerChannel>();
			for (int i = 0; i < mChDef[GetDimmerBrand(brandStr)]; i++) {
				DimmerChannel ch = new DimmerChannel();
				ch.Channel = (Channels)i;
				ch.Port = portStr;
				chColl.Add(ch);
			}

			DimmerPack pack = new DimmerPack(nameStr, GetDimmerBrand(brandStr), portStr, chColl);
			mDimAdd.Add(pack);
			dgvDimmer.BeginInvokeIfNecessary(() => dgvDimmer.Rows.Add(CreateDimmerRow(dgvDimmer, pack)));
			mFlag_Modified = true;
		}

		private void dgvDimmer_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if (!mFlag_Editing && dgvDimmer.SelectedCells.Count > 0) {
				if (mFlag_Modified) {
					Dictionary<string, string> msg = GetMultiLangText("ExitTit", "ExitMsg");
					MsgBoxBtn btn = CtMsgBox.Show(
						msg["ExitTit"],
						msg["ExitMsg"],
						MsgBoxBtn.YesNo | MsgBoxBtn.Cancel,
						MsgBoxStyle.Question
					);
					if (btn == MsgBoxBtn.YesNo) SaveLights();
					else if (btn == MsgBoxBtn.Cancel) return;
				}
				string name = dgvDimmer.Rows[dgvDimmer.SelectedCells[0].RowIndex].Cells[colName.Index].Value.ToString();
				DimmerPack pack = mLights.Find(val => val.Name == name);
				LoadLights(pack);
			}
		}

		private void btnDeleteDim_Click(object sender, EventArgs e) {
			if (dgvDimmer.SelectedCells.Count > 0) {
				int rowIdx = dgvDimmer.SelectedCells[0].RowIndex;
				string name = dgvDimmer.Rows[rowIdx].Cells[0].Value.ToString();
				Dictionary<string, string> msg = GetMultiLangText("DimDelTit", "DimDelMsg");
				MsgBoxBtn btn = CtMsgBox.Show(
					msg["DimDelTit"],
					string.Format(msg["DimDelMsg"].Replace("\\r\\n","\r\n"), name),
					MsgBoxBtn.YesNo,
					MsgBoxStyle.Question
				);
				if (btn == MsgBoxBtn.Yes) {
					mDimDel.Add(name);
					dgvDimmer.BeginInvokeIfNecessary(() => dgvDimmer.Rows.RemoveAt(rowIdx));
				}
				mFlag_Modified = true;
			}
		}
		
		private void btnEditDim_Click(object sender, EventArgs e) {
			tsk_DimEditMode(true);
		}

		private void btnLeaveDim_Click(object sender, EventArgs e) {
			if (mFlag_Modified) {
				Dictionary<string, string> msg = GetMultiLangText("ExitTit", "ExitMsg");
				MsgBoxBtn btn = CtMsgBox.Show(
					msg["ExitTit"],
					msg["ExitMsg"],
					MsgBoxBtn.YesNo | MsgBoxBtn.Cancel,
					MsgBoxStyle.Question
				);
				if (btn == MsgBoxBtn.Yes) SaveDimmers();
				else if (btn == MsgBoxBtn.Cancel) return;
			}
			mFlag_Modified = false;
			tsk_DimEditMode(false);
			LoadDimmers(mLights);
			CtInvoke.ControlText(lbDimmerName, string.Empty);
			CtInvoke.DataGridViewClear(dgvChannel);
		}

		private void btnSaveDim_Click(object sender, EventArgs e) {
			SaveDimmers();
		}

		private void CtAceDimmerSetting_Paint(object sender, PaintEventArgs e) {
			int x = (dgvDimmer.Right + dgvChannel.Left) / 2;
			int y1 = btnEdit.Top - 5;
			int y2 = dgvChannel.Bottom + 5;

			e.Graphics.DrawLine(new Pen(Color.LightGray, 2), x, y1, x, y2);
		}

		private void btnExport_Click(object sender, EventArgs e) {
			int idx = 0;
			List<XmlElmt> dataColl = mLights.ConvertAll(pack => pack.CreateXmlData($"DIM_{idx}"));
			using (SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.Filter = "可延伸標記式語言 | *.xml";
				if (dialog.ShowDialog() == DialogResult.OK) {
					XmlElmt xmlData = new XmlElmt(
						"Dimmer",
						dataColl
					);
					CtXML.Save(xmlData, dialog.FileName);
					CtMsgBox.Show("匯出", $"已匯出至 {dialog.FileName}");
				}
			}
		}

		private void btnImport_Click(object sender, EventArgs e) {
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = "可延伸標記式語言 | *.xml";
				if (dialog.ShowDialog() == DialogResult.OK) {
					XmlElmt dimXml = CtXML.Load(dialog.FileName);
					List<XmlElmt> dataColl = dimXml.Elements();

					mLights.Clear();
					mLights.AddRange(dataColl.ConvertAll(data => new DimmerPack(data)));
					LoadDimmers(mLights);
					rLights.Clear();
					rLights.AddRange(mLights.ConvertAll(light => light.Clone()));
				}
			}
		}

		#endregion
	}
}
