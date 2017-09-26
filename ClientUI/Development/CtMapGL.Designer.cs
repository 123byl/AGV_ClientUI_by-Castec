namespace ClientUI {
    partial class CtMapGL {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
                glMap.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            MapGL.CastecMapUI.Pos pos1 = new MapGL.CastecMapUI.Pos();
            this.glMap = new MapGL.CastecMapUI();
            this.SuspendLayout();
            // 
            // glMap
            // 
            this.glMap.AutoSize = true;
            this.glMap.CarShape = MapGL.CastecMapUI.Shape.Rectangle;
            this.glMap.ColorAxisX = System.Drawing.Color.Red;
            this.glMap.ColorAxisY = System.Drawing.Color.Green;
            this.glMap.ColorBackground = System.Drawing.Color.White;
            this.glMap.ColorCarIcon = System.Drawing.Color.Blue;
            this.glMap.ColorGoalIcon = System.Drawing.Color.Lime;
            this.glMap.ColorGrid = System.Drawing.Color.Silver;
            this.glMap.ColorObstacle = System.Drawing.Color.Maroon;
            this.glMap.ColorPowerIcon = System.Drawing.Color.Orange;
            this.glMap.ColorTextPoint = System.Drawing.Color.Yellow;
            this.glMap.CountTotalCar = ((uint)(0u));
            this.glMap.CountTotalPower = ((uint)(1u));
            this.glMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glMap.EnableAxis = false;
            this.glMap.EnableCar = true;
            this.glMap.EnableGoal = true;
            this.glMap.EnableGrid = true;
            this.glMap.EnableMouseLocatation = true;
            this.glMap.EnablePower = true;
            this.glMap.FontText = new System.Drawing.Font("Arial", 12F);
            this.glMap.GoalShape = MapGL.CastecMapUI.Shape.Rectangle;
            this.glMap.Location = new System.Drawing.Point(0, 0);
            this.glMap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.glMap.MaxPos = new System.Drawing.Point(2000, 2000);
            this.glMap.MinPos = new System.Drawing.Point(-2000, -2000);
            this.glMap.Name = "glMap";
            pos1.id = ((uint)(0u));
            pos1.name = null;
            pos1.theta = 0D;
            pos1.x = 0D;
            pos1.y = 0D;
            this.glMap.PosCar = pos1;
            this.glMap.Resolution = 1;
            this.glMap.Size = new System.Drawing.Size(978, 698);
            this.glMap.SizeCar = new System.Drawing.Size(700, 560);
            this.glMap.SizeGoal = new System.Drawing.Size(750, 610);
            this.glMap.SizeGrid = 1000;
            this.glMap.TabIndex = 6;
            this.glMap.Zoom = 6.25D;
            this.glMap.MouseSelectObj += new MapGL.CastecMapUI.DelMouseSelectObj(this.glMap_MouseSelectObj);
            this.glMap.MouseClickRealPos += new MapGL.CastecMapUI.DelMouseClickRealPos(this.glMap_MouseClickRealPos);
            this.glMap.MouseSelectRange += new MapGL.CastecMapUI.DelMouseSelectRange(this.glMap_MouseSelectRange);
            // 
            // CtMapGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 698);
            this.CloseButton = false;
            this.Controls.Add(this.glMap);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "CtMapGL";
            this.Text = "MapGL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapGL.CastecMapUI glMap;
    }
}