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
using CtLib.Module.Utility;

namespace CtLib.Module.Adept {
	/// <summary>提供使用者介面供調整調光器電流數值，以 <see cref="CtAceDimmerSetting"/> 為基礎</summary>
	public partial class CtAceLightSetting : Form {

		#region Fields
		private string mCurSelc = string.Empty;
		private bool mFlag_Modified = false;
		private UILanguage mLang = UILanguage.TraditionalChinese;
		private List<DimmerPack> rLights;
		private List<DimmerPack> mLights;
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
		#endregion

		#region Constructors
		/// <summary>建構設定視窗</summary>
		/// <param name="lang">欲顯示的語言</param>
		/// <param name="lights">已載入的光源集合</param>
		public CtAceLightSetting(UILanguage lang, List<DimmerPack> lights) {
			InitializeComponent();

			mLang = lang;
			rLights = lights;
			mLights = lights.ConvertAll(val => val.Clone());

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

		private void LoadDimmers(List<DimmerPack> lights) {
			mCurSelc = string.Empty;
			CtInvoke.DataGridViewClear(dgvChannel);
			List<DataGridViewRow> rowColl = lights.ConvertAll(pack => CreateDimmerRow(dgvDimmer, pack));
			dgvDimmer.InvokeIfNecessary(
				() => {
					dgvDimmer.Rows.Clear();
					dgvDimmer.Rows.AddRange(rowColl.ToArray());
				}
			);
		}

		private void tsk_EditMode(bool edit) {
			CtInvoke.ControlVisible(btnEdit, !edit);
			CtInvoke.ControlVisible(btnSave, edit);
			CtInvoke.ControlVisible(btnLeave, edit);
			dgvChannel.InvokeIfNecessary(
				() => {
					colValue.DefaultCellStyle.BackColor = edit ? Color.LightSkyBlue : SystemColors.Window;
					colValue.ReadOnly = !edit;
				}
			) ;
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
			row.Cells[colValue.Index].Value = ch.CurrentValue;
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
			pack.ClearChannels();
			foreach (DataGridViewRow row in dgvChannel.Rows) {
				DimmerChannel ch = new DimmerChannel();
				ch.Channel = GetLightChannel(row.Cells[colCh.Index].Value.ToString());
				ch.EmitColor = GetLightColor(row.Cells[colColor.Index].Value.ToString());
				ch.LightShape = GetLightStyle(row.Cells[colStyle.Index].Value.ToString());
				ch.Comment = row.Cells[colCmt.Index].Value.ToString();
				ch.Port = pack.Port;
				ch.CurrentValue = int.Parse(row.Cells[colValue.Index].Value.ToString());
				pack.AddChannels(ch);
			}

			/* 寫回全域 */
			DimmerPack refPack = rLights.Find(val => val.Name == mCurSelc);
			refPack.ClearChannels();
			refPack.AssignChannels(pack.Channels);

			mFlag_Modified = false;
			tsk_EditMode(false);
		}
		#endregion

		#region Interface Events

		private void btnEdit_Click(object sender, EventArgs e) {
			if (!string.IsNullOrEmpty(mCurSelc)) {
				tsk_EditMode(true);
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
			mFlag_Modified = false;
			tsk_EditMode(false);
			LoadDimmers(mLights);
			CtInvoke.ControlText(lbDimmerName, string.Empty);
			CtInvoke.DataGridViewClear(dgvChannel);
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
				if (btn == MsgBoxBtn.Yes) SaveLights();
				else if (btn == MsgBoxBtn.Cancel) return;
			}
			CtInvoke.FormClose(this);
		}

		private void dgvLight_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
			mFlag_Modified = true;
		}

		private void dgvDimmer_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			if (dgvDimmer.SelectedCells.Count > 0) {
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
				mFlag_Modified = false;
				tsk_EditMode(false);
				mCurSelc = dgvDimmer.Rows[dgvDimmer.SelectedCells[0].RowIndex].Cells[colName.Index].Value.ToString();
				CtInvoke.ControlText(lbDimmerName, mCurSelc);
				List<DataGridViewRow> chColl = mLights.Find(val => val.Name == mCurSelc).Channels.ConvertAll(light => CreateLightRow(dgvChannel, light));
				dgvChannel.InvokeIfNecessary(
					() => {
						dgvChannel.Rows.Clear();
						dgvChannel.Rows.AddRange(chColl.ToArray());
					}
				);
			}
		}

		#endregion
	}
}
