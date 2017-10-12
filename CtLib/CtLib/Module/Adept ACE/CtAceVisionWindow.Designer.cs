﻿namespace CtLib.Module.Adept {
    partial class CtAceVisionWindow {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
				if (rAce != null) {
					rAce.Vision.RemoveVisionEventHandler();

					rAce.Vision.ImageBufferAdded -= mIVisPlug_ServerBufferAdded;
					rAce.Vision.ImageBufferRemoved -= mIVisPlug_ServerBufferRemoved;
					rAce.Vision.ImageBufferRenamed -= mIVisPlug_ServerBufferNameChanged;
					rAce.Vision.VisionServerChagned -= rAce_VisionServerChagned;

					rAce = null;
				}
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
			this.tabWindow = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// tabWindow
			// 
			this.tabWindow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabWindow.Location = new System.Drawing.Point(0, 0);
			this.tabWindow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabWindow.Name = "tabWindow";
			this.tabWindow.SelectedIndex = 0;
			this.tabWindow.Size = new System.Drawing.Size(200, 200);
			this.tabWindow.TabIndex = 0;
			// 
			// CtAceVisionWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabWindow);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "CtAceVisionWindow";
			this.Size = new System.Drawing.Size(200, 200);
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TabControl tabWindow;
	}
}
