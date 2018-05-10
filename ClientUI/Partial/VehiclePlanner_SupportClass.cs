using CtLib.Module.Utility;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VehiclePlanner.Module.Implement;
using VehiclePlanner.Module.Interface;

namespace VehiclePlanner.Partial.VehiclePlannerUI {
    
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


    /// <summary>
    /// 子視窗權限定義
    /// </summary>
    public class DockContainerAuthority {

        /// <summary>
        /// 回傳子視窗權限
        /// </summary>
        /// <param name="typeName">子視窗類型名稱</param>
        /// <param name="lv">使用者權限</param>
        /// <returns></returns>
        public virtual bool Authority(string typeName, AccessLevel lv) {
            switch (typeName) {
                case nameof(CtTesting):
                case nameof(ITesting):
                    return lv > AccessLevel.Operator;

                case nameof(CtConsole):
                case nameof(IConsole):
                    return true;

                case nameof(BaseGoalSetting):
                case nameof(IBaseGoalSetting):
                    return lv > AccessLevel.None;

                case nameof(BaseMapGL):
                case nameof(IBaseMapGL):
                    return lv > AccessLevel.None;

                default:
                    throw new Exception($"未定義{typeName}權限");
            }
        }
    }

}