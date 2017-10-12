using System;
using System.ComponentModel;
using System.Windows.Forms;

using CtLib.Library;
using CtLib.Module.Utility;

using Ace.Core.Client;
using Ace.HSVision.Client.ImageDisplay;
using Ace.HSVision.Server.Tools;
using Ace.HSVision.Server.Integration;

namespace CtLib.Module.Adept {
	/// <summary>顯示單一 VisionTool 畫面</summary>
	/// <example><code language="C#">
	/// /*-- Create and connect Adept ACE --*/
	/// CtAce mAce = new CtAce();
	/// mAce.Connect(ControllerType.SmartController);
	/// 
	/// /*-- Create an object --*/
	/// CtAceSingleVision singleVision = new CtAceSingleVision();   //New a form or drag it into form from Visualize Utility
	/// 
	/// /*-- Select the tool that you want to show --*/
	/// string visPath = @"/Equipment/Vision/Device/Virtual Camera";
	/// 
	/// /*-- Show it --*/
	/// singleVision.Connect(mAce, visPath);
	/// </code></example>
	public partial class CtAceSingleVision : UserControl {

		#region Version

		/// <summary>CtAce 版本訊息</summary>
		public static readonly CtVersion @Version = new CtVersion(1, 1, 0, "2016/11/09", "Ahern Kuo");

		/*---------- Version Note ----------
         * 
         * 1.0.0  <Ahern> [2014/09/10]
         *      + 建立Connect模組
		 *      
		 * 1.1.0  <Ahern> [2016/11/09]
		 *		\ 使用 IVisionImageSource 與 IVisionTool 進行判斷，省去填入 ToolType
		 *		\ 修正尺標等顯示屬性，可於 VS 屬性分頁調整
         *      + 新增 Disconnect 方法
		 * 
         *----------------------------------*/

		#endregion

		#region Declaration - Properties

		/// <summary>是否顯示尺標</summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool ShowRuler { get; set; } = false;

		/// <summary>是否顯示滑條</summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool ShowScrollBar { get; set; } = false;

		/// <summary>是否顯示工作列</summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool ShowToolBar { get; set; } = false;

		/// <summary>是否顯示狀態列</summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool ShowStatusBar { get; set; } = false;

		/// <summary>是否顯示執行時間</summary>
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool ShowExecutionTime { get; set; } = false;

		/// <summary>是否顯示自動清除</summary>
		[DefaultValue(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool AutoClearEnabled { get; set; } = true;

		#endregion

		#region Declaration - Fields
		private IImageDisplayControl mImgDisp;
		#endregion

		#region Function - Core
		/// <summary>建構一單影像之使用者控制項</summary>
		public CtAceSingleVision() {
			InitializeComponent();
		}

		/// <summary>與透過CtAce連接影像工具，並顯示在畫面上</summary>
		/// <param name="ace">CtAce元件，用於連接並取的影像</param>
		/// <param name="path">影像工具之路徑，如 @"/Equipment/Vision/Device/Virtual Camera"</param>
		/// <param name="showExecption">指出當 <see cref="Exception"/> 發生時，是否顯示對話視窗以提醒使用者</param>
		/// <returns>Status Code</returns>
		public Stat Connect(CtAce ace, string path, bool showExecption = true) {
			Stat stt = Stat.SUCCESS;

			try {
				var tool = ace.FindObject(path) as IVisionToolBase;
				if (tool == null) {
					stt = Stat.ER_SYS_INVARG;
					throw new System.IO.FileNotFoundException("Can not find specified vision tool", "VisionToolPath");
				}

				IImageBuffer imgBuf = null;
				if (tool is IVisionImageSource) imgBuf = (tool as IVisionImageSource).Buffer;
				else if (tool is IVisionTool) imgBuf = (tool as IVisionTool).ImageSource.Buffer;

				/*-- 如果有正確取得IImageBuffer，建立Control並顯示 --*/
				if (imgBuf != null) {

					/*-- 在IAceClient建立DisplayControl --*/
					IAceClient client = ace.GetClient();

					mImgDisp = client.CreateObject(typeof(IImageDisplayControl)) as IImageDisplayControl;

					/*-- 設定DisplayControl --*/
					mImgDisp.Client = client;
					mImgDisp.Buffer = imgBuf;
					mImgDisp.AutomaticRendering = true;
					mImgDisp.RulersVisible = ShowRuler;
					mImgDisp.ScrollBarsVisible = ShowScrollBar;
					mImgDisp.ToolBarVisible = ShowToolBar;
					mImgDisp.StatusBarVisible = ShowStatusBar;
					mImgDisp.ExecutionTimeVisible = ShowExecutionTime;
					mImgDisp.AutoClearGraphics = AutoClearEnabled;

					/*-- 轉為Windows之Control --*/
					Control ctrl = mImgDisp as Control;
					ctrl.Dock = DockStyle.Fill;
					ctrl.Name = imgBuf.Name;
					ctrl.Visible = false;
					ctrl.Visible = true;

					/*-- Assign --*/
					this.Controls.Add(ctrl);
				}
			} catch (Exception ex) {
				stt = Stat.ER3_ACE;
				CtStatus.Report(stt, ex, showExecption);
			}

			return stt;
		}

		/// <summary>停止影像視窗，此方法僅將 <see cref="IImageDisplayControl"/> 視窗釋放，而非釋放整個 <see cref="CtAceSingleVision"/></summary>
		/// <returns>Status Code</returns>
		public Stat Disconnect() {
			Stat stt = Stat.SUCCESS;
			try {
				if (mImgDisp != null) {
					mImgDisp.Dispose();
					mImgDisp = null;
				}
			} catch (Exception ex) {
				stt = Stat.ER3_ACE;
				CtStatus.Report(stt, ex);
			}
			return stt;
		}

		#endregion
	}
}
