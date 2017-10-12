using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Net;
using CtLib.Module.Utility;
using CtLib.Module.Utility.Renderer;
using CtLib.Module.Utility.Drawing;
using System.Runtime.InteropServices;

namespace CtLib.Forms.TestPlatform {

	/// <summary>[測試介面] Socket</summary>
	public partial class Test_Socket : Form {

		#region Declaration - Fields
		private CtSocket mSocket;
		private TransDataFormats mDataType = TransDataFormats.String;
		private NumericFormats mDataFormat = NumericFormats.Decimal;
		private Dictionary<string, bool> mMouseIn = new Dictionary<string, bool>();
		private Dictionary<string, Dictionary<UILanguage, string>> mCult = new Dictionary<string, Dictionary<UILanguage, string>> {
			{ "connect", 
				new Dictionary<UILanguage, string> { 
					{UILanguage.English, "Connect" },
					{UILanguage.SimplifiedChinese, "联机" },
					{UILanguage.TraditionalChinese, "連線" }
				}
			},
			{ "disconnect",
				new Dictionary<UILanguage, string> {
					{UILanguage.English, "Disconnect" },
					{UILanguage.SimplifiedChinese, "中断联机" },
					{UILanguage.TraditionalChinese, "中斷連線" }
				}
			},
			{ "start",
				new Dictionary<UILanguage, string> {
					{UILanguage.English, "Start" },
					{UILanguage.SimplifiedChinese, "开始" },
					{UILanguage.TraditionalChinese, "開始" }
				}
			},
			{ "stop",
				new Dictionary<UILanguage, string> {
					{UILanguage.English, "Stop" },
					{UILanguage.SimplifiedChinese, "停止" },
					{UILanguage.TraditionalChinese, "停止" }
				}
			},
			{ "qty",
				new Dictionary<UILanguage, string> {
					{UILanguage.English, "Connected:" },
					{UILanguage.SimplifiedChinese, "已连接: " },
					{UILanguage.TraditionalChinese, "已連接: " }
				}
			}
		};
		#endregion

		#region Function - Constructors
		/// <summary>開啟 Socket 測試介面</summary>
		public Test_Socket() {
			InitializeComponent();

			InitializeControls();
			InitializeMouseState();
			SetStyles();
			AdjustControlLocations();
		}
		#endregion

		#region Function - Styles
		private void InitializeControls() {
			cbMode.SelectedIndex = 0;
			cbEndLine.SelectedIndex = 0;
			cbDataType.SelectedIndex = 0;
			misProtList.SelectedIndex = SocketSetting.Default.Protocol;
			misEncList.SelectedIndex = SocketSetting.Default.Encoding;
			misAutoClear.Checked = SocketSetting.Default.AutoClear;
			misAutoClose.Checked = SocketSetting.Default.AutoClose;
			misShowSend.Checked = SocketSetting.Default.ShowSend;
			misManRx.Checked = SocketSetting.Default.ManualReceive;
			misEnter.Checked = SocketSetting.Default.PressEnter;
		}

		private void FindControls(Dictionary<Type, List<Control>> dic, Control ctrl) {
			if (!dic.ContainsKey(ctrl.GetType())) dic.Add(ctrl.GetType(), new List<Control> { ctrl });
			else dic[ctrl.GetType()].Add(ctrl);

			if (ctrl.HasChildren) {
				foreach (Control item in ctrl.Controls) {
					FindControls(dic, item);
				}
			}
		}

		private void InitializeMouseState() {
			/* Search all controls */
			var ctrls = new Dictionary<Type, List<Control>>();
			foreach (Control ctrl in this.Controls) {
				FindControls(ctrls, ctrl);
			}

			foreach (var kvp in ctrls) {
				kvp.Value.ForEach(
					ctrl => mMouseIn.Add(ctrl.Name, false)
				);
			}
		}

		private void SetStyles() {
			var frmBgClr = Color.FromArgb(30, 30, 30);
			var ctrlBgClr = Color.FromArgb(45, 45, 45);
			var txtClr = Color.FromArgb(220, 220, 220);

			/* Search all controls */
			var ctrls = new Dictionary<Type, List<Control>>();
			foreach (Control ctrl in this.Controls) {
				FindControls(ctrls, ctrl);
			}

			/* Form */
			this.BackColor = frmBgClr;
			this.ForeColor = txtClr;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw, true);

			/* TextBox */
			ctrls[typeof(TextBox)].ForEach(
				ctrl => {
					var txt = ctrl as TextBox;
					txt.BorderStyle = BorderStyle.None;
					txt.BackColor = ctrlBgClr;
					txt.ForeColor = txtClr;
				}
			);

			/* ListBox */
			ctrls[typeof(ListBox)].ForEach(
				ctrl => {
					var lst = ctrl as ListBox;
					lst.BorderStyle = BorderStyle.None;
					lst.BackColor = frmBgClr;
					lst.ForeColor = txtClr;
				}
			);

			/* ComboBox */
			ctrls[typeof(ComboBox)].ForEach(
				ctrl => {
					var cb = ctrl as ComboBox;
					cb.BackColor = ctrlBgClr;
					cb.ForeColor = txtClr;
				}
			);

			/* Labels */
			ctrls[typeof(Label)].ForEach(
				ctrl => {
					var lbl = ctrl as Label;
					lbl.ForeColor = txtClr;
				}
			);

			/* MenuStrip */
			menuStrip.Renderer = new ToolStripDeepStyleRenderer();
			misEncList.BackColor = ctrlBgClr;
			misEncList.ForeColor = txtClr;
			misProtList.BackColor = ctrlBgClr;
			misProtList.ForeColor = txtClr;

			/* ContextMenu */
			cntxMenu.Renderer = new ToolStripDeepStyleRenderer();
		}

		private void AdjustControlLocations() {
			int gap = 3;
			int w = CtInvoke.ControlWidth(gbSend);
			if (SocketSetting.Default.ManualReceive) {
				CtInvoke.ControlVisible(btnReceive, true);
				CtInvoke.ControlLocation(btnReceive, new Point(w - gap - btnReceive.Width - 2, 25));
				CtInvoke.ControlLocation(btnSend, new Point(btnReceive.Left - gap - btnSend.Width, 25));
				CtInvoke.ControlLocation(cbEndLine, new Point(btnSend.Left - gap - cbEndLine.Width, 25));
				CtInvoke.ControlWidth(txtSend, cbEndLine.Left - gap - txtSend.Left);
			} else {
				CtInvoke.ControlVisible(btnReceive, false);
				CtInvoke.ControlLocation(btnSend, new Point(w - gap - btnSend.Width - 2, 25));
				CtInvoke.ControlLocation(cbEndLine, new Point(btnSend.Left - gap - cbEndLine.Width, 25));
				CtInvoke.ControlWidth(txtSend, cbEndLine.Left - gap - txtSend.Left);
			}

			if (SocketSetting.Default.Protocol == 1 /*&& CtInvoke.ComboBoxSelectedIndex(cbMode) == 1*/) {
				CtInvoke.ControlVisible(lbUdpIp, true);
				CtInvoke.ControlVisible(lbUdpPort, true);
				CtInvoke.ControlVisible(txtUdpIp, true);
				CtInvoke.ControlVisible(txtUdpPort, true);
				CtInvoke.ControlHeight(gbSend, txtUdpIp.Bottom + 5);
			} else {
				CtInvoke.ControlVisible(lbUdpIp, false);
				CtInvoke.ControlVisible(lbUdpPort, false);
				CtInvoke.ControlVisible(txtUdpIp, false);
				CtInvoke.ControlVisible(txtUdpPort, false);
				CtInvoke.ControlHeight(gbSend, txtSend.Bottom + 15);
			}

			var rxTop = gbSend.Bottom + 6 - gbReceive.Top;
			CtInvoke.ControlTop(gbReceive, gbReceive.Top + rxTop);
			CtInvoke.ControlHeight(gbReceive, gbReceive.Height - rxTop);
			
			this.Refresh();
		}
		#endregion

		#region Function - Methods
		private void ShowMessage(string data) {
			string strTemp = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.ff"), data);
			CtInvoke.ListBoxInsert(lsbxMsg, 0, strTemp, true);
		}

		private void ShowMessage(DateTime time, string data) {
			string strTemp = string.Format("[{0}] {1}", time.ToString("HH:mm:ss.ff"), data);
			CtInvoke.ListBoxInsert(lsbxMsg, 0, strTemp, true);
		}

		private string GetCult(string keyWord) {
			var cult = CtLanguage.GetUiLangByCult();
			return mCult[keyWord][cult];
		}
		#endregion

		#region Function - CtAsyncSocket Events
		void mSocket_OnSocketEvents(object sender, SocketEventArgs e) {
			switch (e.Event) {
				case SocketEvents.ConnectionWithServer:
					SocketConnection infoClient = e.Value as SocketConnection;
					ShowMessage(infoClient.Time, (infoClient.Status) ? $"已連上 Server @ {infoClient.EndPoint.ToString()}" : "已中斷連線");

					CtInvoke.ControlText(btnConnect, (infoClient.Status) ? GetCult("disconnect") : GetCult("connect"));
					CtInvoke.ControlTag(btnConnect, infoClient.Status);
					CtInvoke.PictureBoxImage(pbStt, (infoClient.Status) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
					CtInvoke.ControlEnabled(cbMode, !infoClient.Status);

					CtInvoke.ControlEnabled(gbSend, infoClient.Status | CtInvoke.ToolStripItemChecked(misAutoClose));
					CtInvoke.ControlEnabled(cbDataType, !infoClient.Status);
					CtInvoke.ToolStripItemEnable(misManRx, !infoClient.Status);
					CtInvoke.ToolStripItemEnable(misProt, !infoClient.Status);
					break;

				case SocketEvents.ClientConnection:
					SocketConnection infoSrv = e.Value as SocketConnection;
					ShowMessage(infoSrv.Time, (infoSrv.Status) ? $"有 Client 連接 @ {infoSrv.EndPoint.ToString()}" : $"Client 中斷連線 @ {infoSrv.EndPoint.ToString()}");

					if (mSocket.Protocol == NetworkProtocol.Tcp) {
						var tcpSock = mSocket as CtTcpSocket;
						CtInvoke.ControlText(lbSrvCount, $"{GetCult("qty")} {tcpSock.ClientCount.ToString()}");
						CtInvoke.ControlEnabled(gbSend, tcpSock.ClientCount > 0);
					}
					break;

				case SocketEvents.Exception:
					ShowMessage(e.Value.ToString());
					break;

				case SocketEvents.DataReceived:
					SocketRxData data = e.Value as SocketRxData;
					if (mSocket.Role == SocketRole.Client)
						if (mDataType == TransDataFormats.String) ShowMessage(data.Time, $"[RX] {data.Data.ToString()}");
						else ShowMessage(data.Time, $"[RX] {CtConvert.CStr(data.Data as List<byte>, mDataFormat)}");
					else
						if (mDataType == TransDataFormats.String) ShowMessage(data.Time, $"[From {data.EndPoint.ToString()}] {data.Data}");
					else ShowMessage(data.Time, $"[From {data.EndPoint.ToString()}] {CtConvert.CStr(data.Data as List<byte>, mDataFormat)}");
					break;

				case SocketEvents.Listen:
					SocketConnection infoListen = e.Value as SocketConnection;
					ShowMessage(infoListen.Time, (infoListen.Status) ? "開始監聽" : "停止監聽");

					CtInvoke.ControlText(btnConnect, (infoListen.Status) ? GetCult("stop") : GetCult("start"));
					CtInvoke.ControlTag(btnConnect, infoListen.Status);
					CtInvoke.PictureBoxImage(pbStt, (infoListen.Status) ? Properties.Resources.Green_Ball : Properties.Resources.Grey_Ball);
					CtInvoke.ControlEnabled(cbMode, !infoListen.Status);
					CtInvoke.ToolStripItemEnable(misManRx, !infoListen.Status);
					CtInvoke.ToolStripItemEnable(misProt, !infoListen.Status);

					if (mSocket.Protocol == NetworkProtocol.Tcp) {
						var tcpSock = mSocket as CtTcpSocket;
						CtInvoke.ControlText(lbSrvCount, $"{GetCult("qty")} {tcpSock.ClientCount.ToString()}");
					}
					break;

				case SocketEvents.DataSend:
					SocketTxData txData = e.Value as SocketTxData;
					if (mSocket.Role == SocketRole.Client)
						if (mDataType == TransDataFormats.String) ShowMessage(txData.Time, $"[TX] {txData.Data.ToString()}");
						else ShowMessage(txData.Time, $"[TX] {CtConvert.CStr(txData.Data as byte[], mDataFormat)}");
					else {
						if (mDataType == TransDataFormats.String)
							ShowMessage(txData.Time, $"[To {txData.EndPoint.ToString()}] " + txData.Data);
						else
							ShowMessage(txData.Time, $"[To {txData.EndPoint.ToString()}] {CtConvert.CStr(txData.Data as byte[], mDataFormat)}");
					}
					break;
				default:
					break;
			}
		}
		#endregion

		#region Function - Interface Events
		private void cbMode_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbMode.SelectedIndex == 0) {
				CtInvoke.ControlEnabled(txtIP, true);
				//CtInvoke.ControlVisible(lbSrvCount, false);
				CtInvoke.ControlText(lbSrvCount, string.Empty);
				CtInvoke.ControlText(btnConnect, GetCult("connect"));
				CtInvoke.ControlTag(btnConnect, false);
				CtInvoke.ToolStripItemEnable(misAutoClose, true);
			} else {
				CtInvoke.ControlEnabled(txtIP, false);
				CtInvoke.ControlText(txtIP, "127.0.0.1");
				//CtInvoke.ControlVisible(lbSrvCount, true);
				if (CtInvoke.ToolStripItemText(misProtList) == "TCP") CtInvoke.ControlText(lbSrvCount, $"{GetCult("qty")} 0");
				CtInvoke.ControlText(btnConnect, GetCult("start"));
				CtInvoke.ControlTag(btnConnect, false);
				CtInvoke.ToolStripItemEnable(misAutoClose, false);
			}
			AdjustControlLocations();
		}

		private void btnConnect_Click(object sender, EventArgs e) {

			try {
				if (CtConvert.CBool(btnConnect.Tag)) {
					if (cbMode.SelectedIndex == 0) {
						mSocket.ClientDisconnect();
					} else {
						mSocket.ServerStopListen();
					}
					mSocket.OnSocketEvents -= mSocket_OnSocketEvents;
					CtInvoke.ControlEnabled(cbDataType, true);
					if (mSocket.Protocol == NetworkProtocol.Udp) {
						CtInvoke.ToolStripItemEnable(misEnc, true);
					}
				} else {
					if (SocketSetting.Default.Protocol == 0) {
						mSocket = new CtTcpSocket(mDataType, !SocketSetting.Default.ManualReceive) {
							CodePage = CtInvoke.ToolStripItemText(misEncList).ToEnum<CodePages>(),
							SubscribeSendEvent = SocketSetting.Default.ShowSend,
						};
					} else {
						mSocket = new CtUdpSocket(mDataType, !SocketSetting.Default.ManualReceive) {
							CodePage = CtInvoke.ToolStripItemText(misEncList).ToEnum<CodePages>(),
							SubscribeSendEvent = SocketSetting.Default.ShowSend,
						};
					}
					mSocket.OnSocketEvents += mSocket_OnSocketEvents;

					if (cbMode.SelectedIndex == 0) {
						mSocket.ClientBeginConnect(CtInvoke.ControlText(txtIP), int.Parse(CtInvoke.ControlText(txtPort)));
					} else {
						mSocket.ServerBeginListen(int.Parse(CtInvoke.ControlText(txtPort)));
						if (mSocket.Protocol == NetworkProtocol.Udp) {  //UDP 不會有 Client 連線資訊
							CtInvoke.ControlEnabled(gbSend, true);
							CtInvoke.ToolStripItemEnable(misEnc, false);
						}
					}
					CtInvoke.ControlEnabled(cbDataType, false);
				}
			} catch (Exception ex) {
				ShowMessage(ex.Message);
			}
		}

		private void btnSend_Click(object sender, EventArgs e) {

			try {
				string sendStr = string.Empty;
				List<byte> sendByte = null;
				if (mDataType == TransDataFormats.String) {
					sendStr = txtSend.Text;

					switch (cbEndLine.SelectedIndex) {
						case 1:
							sendStr += CtConst.NewLine;
							break;
						case 2:
							sendStr += CtConst.Cr;
							break;
						case 3:
							sendStr += CtConst.Lf;
							break;
					}
				} else {
					List<string> strSplit = txtSend.Text.Split(CtConst.CHR_SEPERATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
					if (strSplit != null && strSplit.Count > 0) {
						sendByte = strSplit.ConvertAll(val => Convert.ToByte(val, (int) mDataFormat));
					}
				}

				EndPointSlim eps = null;
				bool udp = SocketSetting.Default.Protocol == 1;
				if (udp) {
					eps = new EndPointSlim(
						CtInvoke.ControlText(txtUdpIp),
						int.Parse(CtInvoke.ControlText(txtUdpPort))
					);
				}

				if (SocketSetting.Default.AutoClose && cbMode.SelectedIndex == 0) {
					if (SocketSetting.Default.Protocol == 0) {
						mSocket = new CtTcpSocket(mDataType, !SocketSetting.Default.ManualReceive) {
							CodePage = CtInvoke.ToolStripItemText(misEncList).ToEnum<CodePages>(),
							SubscribeSendEvent = SocketSetting.Default.ShowSend,
						};
					} else {
						mSocket = new CtUdpSocket(mDataType, !SocketSetting.Default.ManualReceive) {
							CodePage = CtInvoke.ToolStripItemText(misEncList).ToEnum<CodePages>(),
							SubscribeSendEvent = SocketSetting.Default.ShowSend,
						};
					}
					mSocket.OnSocketEvents += mSocket_OnSocketEvents;

					mSocket.ClientConnect(txtIP.Text, int.Parse(txtPort.Text));

					if (udp) (mSocket as CtUdpSocket).TargetEndPoint = eps;

					if (mDataType == TransDataFormats.String) mSocket.Send(sendStr);
					else mSocket.Send(sendByte);
					mSocket.ClientDisconnect();
				} else if (mDataType == TransDataFormats.String) {
					if (udp) (mSocket as CtUdpSocket).TargetEndPoint = eps;
					mSocket.BeginSend(sendStr, CtInvoke.ToolStripItemText(misEncList).ToEnum<CodePages>());
				} else if (mDataType == TransDataFormats.EnumerableOfByte) {
					if (udp) (mSocket as CtUdpSocket).TargetEndPoint = eps;
					mSocket.BeginSend(sendByte);
				}

				if (SocketSetting.Default.AutoClear) {
					CtInvoke.ControlText(txtSend, string.Empty);
				}
			} catch (Exception ex) {
				ShowMessage(ex.Message);
			}
		}

		private void cbDataType_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbDataType.SelectedIndex > -1) {
				mDataType = (TransDataFormats) cbDataType.SelectedIndex;

				if (mDataType == TransDataFormats.String) {
					cbEndLine.InvokeIfNecessary(
						() => {
							cbEndLine.Items.Clear();
							cbEndLine.Items.AddRange(new object[] { "None", "CrLf", "Cr", "Lf" });
						}
					);
					CtInvoke.ToolStripItemEnable(misEnc, true);
				} else {
					cbEndLine.InvokeIfNecessary(
						() => {
							cbEndLine.Items.Clear();
							cbEndLine.Items.AddRange(new object[] { "Hex", "Dec" });
						}
					);
					CtInvoke.ToolStripItemEnable(misEnc, false);
				}
				CtInvoke.ComboBoxSelectedIndex(cbEndLine, 0);
			}
		}

		private void cbEndLine_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbEndLine.SelectedIndex == 0) mDataFormat = NumericFormats.Hexadecimal;
			else mDataFormat = NumericFormats.Decimal;
		}

		private void miSaveLog_Click(object sender, EventArgs e) {
			IEnumerable<string> content = lsbxMsg
											.InvokeIfNecessary(() => lsbxMsg.Items).
											Cast<object>().
											Select(obj => obj.ToString());

			if (content != null && content.Any()) {
				using (SaveFileDialog dialog = new SaveFileDialog()) {
					dialog.Filter = "紀錄檔 | *.log";
					if (dialog.ShowDialog() == DialogResult.OK) {
						CtFile.WriteFile(dialog.FileName, content);
					}
				}
			}
		}

		private void miClrMsg_Click(object sender, EventArgs e) {
			CtInvoke.ListBoxClear(lsbxMsg);
		}

		private void txtSend_KeyPress(object sender, KeyPressEventArgs e) {
			if (SocketSetting.Default.PressEnter && e.KeyChar == '\r') {
				e.Handled = true;
				btnSend.PerformClick();
			}
		}

		private void btnReceive_Click(object sender, EventArgs e) {
			if (mSocket.Role == SocketRole.Client) {
				try {
					var cltRx = mSocket.Receive();
					if (mSocket.DataFormat == TransDataFormats.String) {
						ShowMessage($"[RX] {cltRx.Data.ToString()}");
					} else {
						var rxByte = cltRx.Data as List<byte>;
						ShowMessage($"[RX] {string.Join(" ", rxByte.ConvertAll(b => CtConvert.ToHex(b, 2)))}");
					}
				} catch (Exception ex) {
					ShowMessage(ex.Message);
				}
			} else if (mSocket.Protocol == NetworkProtocol.Tcp) {
				var tcpSock = mSocket as CtTcpSocket;
				List<SocketRxData> srvRx;
				tcpSock.Receive(out srvRx);
				srvRx.ForEach(
					rxData => {
						if (tcpSock.DataFormat == TransDataFormats.String) {
							ShowMessage($"[From {rxData.EndPoint.ToString()}] {rxData.Data.ToString()}");
						} else {
							var rxByte = rxData.Data as List<byte>;
							ShowMessage($"[From {rxData.EndPoint.ToString()}] {string.Join(" ", rxByte.ConvertAll(b => CtConvert.ToHex(b, 2)))}");
						}
					}
				);
			}
		}

		private void misEncList_DropDownClosed(object sender, EventArgs e) {
			CodePages cp = (CodePages) Enum.Parse(typeof(CodePages), CtInvoke.ToolStripItemText(misEncList));
			if (!object.Equals(mSocket, null)) {
				mSocket.CodePage = cp;
				if (cp == CodePages.ASCII || cp == CodePages.MACINTOSH) {
					txtSend.InvokeIfNecessary(() => txtSend.ImeMode = ImeMode.Disable);
				} else {
					txtSend.InvokeIfNecessary(() => txtSend.ImeMode = ImeMode.NoControl);
				}
			}

			SocketSetting.Default.Encoding = misEncList.SelectedIndex;
			SocketSetting.Default.Save();
		}

		private void GroupBoxPaint(object sender, PaintEventArgs e) {
			var bgClr = Color.FromArgb(30, 30, 30);
			var bdClr = Color.FromArgb(70, 70, 70);
			var txtClr = Color.FromArgb(220, 220, 220);
			var grp = sender as GroupBox;
			var g = e.Graphics;

			//取得標題大小
			var txtSz = g.MeasureString(grp.Text, grp.Font);
			//計算邊框位置
			var bdRect = e.ClipRectangle;
			var halfTxtY = Math.Ceiling(txtSz.Height / 2);
			bdRect.Y += (int) halfTxtY;
			bdRect.Height -= (int) halfTxtY;
			var path = CtDraw.GetRoundRectanglePath(bdRect);

			//畫畫
			g.Clear(bgClr);
			g.DrawPath(bdClr.GetPen(1), path);
			g.FillRectangle(bgClr.GetSolidBrush(), 8, 0, txtSz.Width + 2, txtSz.Height);
			g.DrawString(grp.Text, grp.Font, txtClr.GetSolidBrush(), 10, 0);
		}

		private void ButtonPaint(object sender, PaintEventArgs e) {
			var bgClr = Color.FromArgb(45, 45, 45);
			var bdClr = Color.FromArgb(70, 70, 70);
			var txtClr = Color.FromArgb(220, 220, 220);
			var disClr = Color.FromArgb(30, 30, 30);
			var btn = sender as Button;
			var g = e.Graphics;

			//取得文字座標
			var txtSz = g.MeasureString(btn.Text, btn.Font);
			var txtPt = new PointF(
				(e.ClipRectangle.Width - txtSz.Width) / 2,
				(e.ClipRectangle.Height - txtSz.Height) / 2
			);

			//畫畫
			g.Clear(mMouseIn[btn.Name] ? bdClr : bgClr);
			var brsh = (btn.Enabled) ? txtClr.GetSolidBrush() : disClr.GetSolidBrush();
			g.DrawString(btn.Text, btn.Font, brsh, txtPt);
		}

		private void MouseIn(object sender, EventArgs e) {
			var ctrl = sender as Control;
			mMouseIn[ctrl.Name] = true;
		}

		private void MouseOut(object sender, EventArgs e) {
			var ctrl = sender as Control;
			mMouseIn[ctrl.Name] = false;
		}

		private void misAutoClose_Click(object sender, EventArgs e) {
			SocketSetting.Default.AutoClose = !SocketSetting.Default.AutoClose;
			SocketSetting.Default.Save();
			CtInvoke.ToolStripItemChecked(misAutoClose, SocketSetting.Default.AutoClose);
		}

		private void misEnter_Click(object sender, EventArgs e) {
			SocketSetting.Default.PressEnter = !SocketSetting.Default.PressEnter;
			SocketSetting.Default.Save();
			CtInvoke.ToolStripItemChecked(misEnter, SocketSetting.Default.PressEnter);
		}

		private void misAutoClear_Click(object sender, EventArgs e) {
			SocketSetting.Default.AutoClear = !SocketSetting.Default.AutoClear;
			SocketSetting.Default.Save();
			CtInvoke.ToolStripItemChecked(misAutoClear, SocketSetting.Default.AutoClear);
		}

		private void misShowSend_Click(object sender, EventArgs e) {
			SocketSetting.Default.ShowSend = !SocketSetting.Default.ShowSend;
			SocketSetting.Default.Save();
			CtInvoke.ToolStripItemChecked(misShowSend, SocketSetting.Default.ShowSend);
			if (mSocket != null) {
				mSocket.SubscribeSendEvent = SocketSetting.Default.ShowSend;
			}
		}

		private void misManRx_Click(object sender, EventArgs e) {
			SocketSetting.Default.ManualReceive = !SocketSetting.Default.ManualReceive;
			SocketSetting.Default.Save();
			CtInvoke.ToolStripItemChecked(misManRx, SocketSetting.Default.ManualReceive);
			AdjustControlLocations();
			this.Refresh();
		}

		private void misExit_Click(object sender, EventArgs e) {
			if (mSocket != null) mSocket.Dispose();
			CtInvoke.FormClose(this);
		}

		private void Test_Socket_Paint(object sender, PaintEventArgs e) {
			var pen = Color.FromArgb(0, 122, 204).GetPen(1);
			e.Graphics.DrawRectangle(pen, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
		}

		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HT_CAPTION = 0x2;

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		private void menuStrip_MouseDown(object sender, MouseEventArgs e) {
			ReleaseCapture();
			SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
		}

		/// <summary>處理 Windows 訊息，當滑鼠指向右下角時，顯示 Moving 並允許改變大小</summary>
		/// <param name="m">Windows 訊息</param>
		protected override void WndProc(ref Message m) {
			if (0x84.Equals(m.Msg)) {
				var lparam = m.LParam.ToInt32();
				var pos = new Point(lparam & 0xFFFF, lparam >> 16);
				pos = this.PointToClient(pos);
				if (pos.X >= this.ClientSize.Width - 10 && pos.Y >= this.ClientSize.Height - 10) {
					m.Result = (IntPtr) 17;
					return;
				}
			}
			base.WndProc(ref m);
		}

		private void misCbProtocol_DropDownClosed(object sender, EventArgs e) {
			SocketSetting.Default.Protocol = misProtList.SelectedIndex;
			SocketSetting.Default.Save();
			AdjustControlLocations();
		}

		private void misCopy_Click(object sender, EventArgs e) {
			if (lsbxMsg.SelectedIndex > -1) {
				var sltStr = lsbxMsg.SelectedItem.ToString();
				Clipboard.SetText(sltStr);
			}
		}

		#endregion
	}
}
