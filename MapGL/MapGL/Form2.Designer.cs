namespace MapGL
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MapGL.CastecMapUI.Pos pos1 = new MapGL.CastecMapUI.Pos();
            this.lbSetCarY = new System.Windows.Forms.Label();
            this.lbSetCarX = new System.Windows.Forms.Label();
            this.txtSetCarY = new System.Windows.Forms.TextBox();
            this.txtSetCarX = new System.Windows.Forms.TextBox();
            this.lbSetCarTheta = new System.Windows.Forms.Label();
            this.txtSetCarTheta = new System.Windows.Forms.TextBox();
            this.btnSetCarPosition = new System.Windows.Forms.Button();
            this.txtSetGoalTheta = new System.Windows.Forms.TextBox();
            this.lbSetGoalTheta = new System.Windows.Forms.Label();
            this.txtSetGoalX = new System.Windows.Forms.TextBox();
            this.txtSetGoalY = new System.Windows.Forms.TextBox();
            this.lbSetGoalX = new System.Windows.Forms.Label();
            this.lbSetGoalY = new System.Windows.Forms.Label();
            this.btnSetGoalPosition = new System.Windows.Forms.Button();
            this.txtSetPowerTheta = new System.Windows.Forms.TextBox();
            this.lbSetPowerTheta = new System.Windows.Forms.Label();
            this.txtSetPowerX = new System.Windows.Forms.TextBox();
            this.txtSetPowerY = new System.Windows.Forms.TextBox();
            this.lbSetPowerX = new System.Windows.Forms.Label();
            this.lbSetPowerY = new System.Windows.Forms.Label();
            this.btnSetPowerPosition = new System.Windows.Forms.Button();
            this.btnClrGoal = new System.Windows.Forms.Button();
            this.btnClrPower = new System.Windows.Forms.Button();
            this.btnClearDraw = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnWhereCar = new System.Windows.Forms.Button();
            this.castecMapUI1 = new MapGL.CastecMapUI();
            this.SuspendLayout();
            // 
            // lbSetCarY
            // 
            this.lbSetCarY.AutoSize = true;
            this.lbSetCarY.BackColor = System.Drawing.Color.Transparent;
            this.lbSetCarY.Location = new System.Drawing.Point(141, 10);
            this.lbSetCarY.Name = "lbSetCarY";
            this.lbSetCarY.Size = new System.Drawing.Size(49, 13);
            this.lbSetCarY.TabIndex = 10;
            this.lbSetCarY.Text = "Set CarY";
            // 
            // lbSetCarX
            // 
            this.lbSetCarX.AutoSize = true;
            this.lbSetCarX.BackColor = System.Drawing.Color.Transparent;
            this.lbSetCarX.Location = new System.Drawing.Point(34, 10);
            this.lbSetCarX.Name = "lbSetCarX";
            this.lbSetCarX.Size = new System.Drawing.Size(49, 13);
            this.lbSetCarX.TabIndex = 11;
            this.lbSetCarX.Text = "Set CarX";
            // 
            // txtSetCarY
            // 
            this.txtSetCarY.Location = new System.Drawing.Point(114, 26);
            this.txtSetCarY.Name = "txtSetCarY";
            this.txtSetCarY.Size = new System.Drawing.Size(94, 20);
            this.txtSetCarY.TabIndex = 8;
            this.txtSetCarY.Text = "0";
            this.txtSetCarY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSetCarX
            // 
            this.txtSetCarX.Location = new System.Drawing.Point(7, 26);
            this.txtSetCarX.Name = "txtSetCarX";
            this.txtSetCarX.Size = new System.Drawing.Size(94, 20);
            this.txtSetCarX.TabIndex = 9;
            this.txtSetCarX.Text = "0";
            this.txtSetCarX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSetCarTheta
            // 
            this.lbSetCarTheta.AutoSize = true;
            this.lbSetCarTheta.BackColor = System.Drawing.Color.Transparent;
            this.lbSetCarTheta.Location = new System.Drawing.Point(235, 10);
            this.lbSetCarTheta.Name = "lbSetCarTheta";
            this.lbSetCarTheta.Size = new System.Drawing.Size(70, 13);
            this.lbSetCarTheta.TabIndex = 13;
            this.lbSetCarTheta.Text = "Set CarTheta";
            // 
            // txtSetCarTheta
            // 
            this.txtSetCarTheta.Location = new System.Drawing.Point(220, 26);
            this.txtSetCarTheta.Name = "txtSetCarTheta";
            this.txtSetCarTheta.Size = new System.Drawing.Size(94, 20);
            this.txtSetCarTheta.TabIndex = 12;
            this.txtSetCarTheta.Text = "0";
            this.txtSetCarTheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSetCarPosition
            // 
            this.btnSetCarPosition.Location = new System.Drawing.Point(343, 3);
            this.btnSetCarPosition.Name = "btnSetCarPosition";
            this.btnSetCarPosition.Size = new System.Drawing.Size(102, 47);
            this.btnSetCarPosition.TabIndex = 14;
            this.btnSetCarPosition.Text = "Set Car Position";
            this.btnSetCarPosition.UseVisualStyleBackColor = true;
            this.btnSetCarPosition.Click += new System.EventHandler(this.btnSetCarPosition_Click);
            // 
            // txtSetGoalTheta
            // 
            this.txtSetGoalTheta.Location = new System.Drawing.Point(220, 95);
            this.txtSetGoalTheta.Name = "txtSetGoalTheta";
            this.txtSetGoalTheta.Size = new System.Drawing.Size(94, 20);
            this.txtSetGoalTheta.TabIndex = 12;
            this.txtSetGoalTheta.Text = "0";
            this.txtSetGoalTheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSetGoalTheta
            // 
            this.lbSetGoalTheta.AutoSize = true;
            this.lbSetGoalTheta.BackColor = System.Drawing.Color.Transparent;
            this.lbSetGoalTheta.Location = new System.Drawing.Point(234, 79);
            this.lbSetGoalTheta.Name = "lbSetGoalTheta";
            this.lbSetGoalTheta.Size = new System.Drawing.Size(79, 13);
            this.lbSetGoalTheta.TabIndex = 13;
            this.lbSetGoalTheta.Text = "Set Goal Theta";
            // 
            // txtSetGoalX
            // 
            this.txtSetGoalX.Location = new System.Drawing.Point(7, 95);
            this.txtSetGoalX.Name = "txtSetGoalX";
            this.txtSetGoalX.Size = new System.Drawing.Size(94, 20);
            this.txtSetGoalX.TabIndex = 9;
            this.txtSetGoalX.Text = "0";
            this.txtSetGoalX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSetGoalY
            // 
            this.txtSetGoalY.Location = new System.Drawing.Point(114, 95);
            this.txtSetGoalY.Name = "txtSetGoalY";
            this.txtSetGoalY.Size = new System.Drawing.Size(94, 20);
            this.txtSetGoalY.TabIndex = 8;
            this.txtSetGoalY.Text = "0";
            this.txtSetGoalY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSetGoalX
            // 
            this.lbSetGoalX.AutoSize = true;
            this.lbSetGoalX.BackColor = System.Drawing.Color.Transparent;
            this.lbSetGoalX.Location = new System.Drawing.Point(33, 79);
            this.lbSetGoalX.Name = "lbSetGoalX";
            this.lbSetGoalX.Size = new System.Drawing.Size(58, 13);
            this.lbSetGoalX.TabIndex = 11;
            this.lbSetGoalX.Text = "Set Goal X";
            // 
            // lbSetGoalY
            // 
            this.lbSetGoalY.AutoSize = true;
            this.lbSetGoalY.BackColor = System.Drawing.Color.Transparent;
            this.lbSetGoalY.Location = new System.Drawing.Point(140, 79);
            this.lbSetGoalY.Name = "lbSetGoalY";
            this.lbSetGoalY.Size = new System.Drawing.Size(58, 13);
            this.lbSetGoalY.TabIndex = 10;
            this.lbSetGoalY.Text = "Set Goal Y";
            // 
            // btnSetGoalPosition
            // 
            this.btnSetGoalPosition.Location = new System.Drawing.Point(343, 73);
            this.btnSetGoalPosition.Name = "btnSetGoalPosition";
            this.btnSetGoalPosition.Size = new System.Drawing.Size(102, 47);
            this.btnSetGoalPosition.TabIndex = 14;
            this.btnSetGoalPosition.Text = "Set Goal Position";
            this.btnSetGoalPosition.UseVisualStyleBackColor = true;
            this.btnSetGoalPosition.Click += new System.EventHandler(this.btnSetGoalPosition_Click);
            // 
            // txtSetPowerTheta
            // 
            this.txtSetPowerTheta.Location = new System.Drawing.Point(220, 169);
            this.txtSetPowerTheta.Name = "txtSetPowerTheta";
            this.txtSetPowerTheta.Size = new System.Drawing.Size(94, 20);
            this.txtSetPowerTheta.TabIndex = 12;
            this.txtSetPowerTheta.Text = "0";
            this.txtSetPowerTheta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSetPowerTheta
            // 
            this.lbSetPowerTheta.AutoSize = true;
            this.lbSetPowerTheta.BackColor = System.Drawing.Color.Transparent;
            this.lbSetPowerTheta.Location = new System.Drawing.Point(232, 153);
            this.lbSetPowerTheta.Name = "lbSetPowerTheta";
            this.lbSetPowerTheta.Size = new System.Drawing.Size(87, 13);
            this.lbSetPowerTheta.TabIndex = 13;
            this.lbSetPowerTheta.Text = "Set Power Theta";
            // 
            // txtSetPowerX
            // 
            this.txtSetPowerX.Location = new System.Drawing.Point(7, 169);
            this.txtSetPowerX.Name = "txtSetPowerX";
            this.txtSetPowerX.Size = new System.Drawing.Size(94, 20);
            this.txtSetPowerX.TabIndex = 9;
            this.txtSetPowerX.Text = "0";
            this.txtSetPowerX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSetPowerY
            // 
            this.txtSetPowerY.Location = new System.Drawing.Point(114, 169);
            this.txtSetPowerY.Name = "txtSetPowerY";
            this.txtSetPowerY.Size = new System.Drawing.Size(94, 20);
            this.txtSetPowerY.TabIndex = 8;
            this.txtSetPowerY.Text = "0";
            this.txtSetPowerY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbSetPowerX
            // 
            this.lbSetPowerX.AutoSize = true;
            this.lbSetPowerX.BackColor = System.Drawing.Color.Transparent;
            this.lbSetPowerX.Location = new System.Drawing.Point(31, 153);
            this.lbSetPowerX.Name = "lbSetPowerX";
            this.lbSetPowerX.Size = new System.Drawing.Size(66, 13);
            this.lbSetPowerX.TabIndex = 11;
            this.lbSetPowerX.Text = "Set Power X";
            // 
            // lbSetPowerY
            // 
            this.lbSetPowerY.AutoSize = true;
            this.lbSetPowerY.BackColor = System.Drawing.Color.Transparent;
            this.lbSetPowerY.Location = new System.Drawing.Point(138, 153);
            this.lbSetPowerY.Name = "lbSetPowerY";
            this.lbSetPowerY.Size = new System.Drawing.Size(66, 13);
            this.lbSetPowerY.TabIndex = 10;
            this.lbSetPowerY.Text = "Set Power Y";
            // 
            // btnSetPowerPosition
            // 
            this.btnSetPowerPosition.Location = new System.Drawing.Point(343, 146);
            this.btnSetPowerPosition.Name = "btnSetPowerPosition";
            this.btnSetPowerPosition.Size = new System.Drawing.Size(102, 47);
            this.btnSetPowerPosition.TabIndex = 14;
            this.btnSetPowerPosition.Text = "Set Power Position";
            this.btnSetPowerPosition.UseVisualStyleBackColor = true;
            this.btnSetPowerPosition.Click += new System.EventHandler(this.btnSetPowerPosition_Click);
            // 
            // btnClrGoal
            // 
            this.btnClrGoal.Location = new System.Drawing.Point(451, 73);
            this.btnClrGoal.Name = "btnClrGoal";
            this.btnClrGoal.Size = new System.Drawing.Size(102, 47);
            this.btnClrGoal.TabIndex = 14;
            this.btnClrGoal.Text = "Clear Goal Position";
            this.btnClrGoal.UseVisualStyleBackColor = true;
            this.btnClrGoal.Click += new System.EventHandler(this.btnClrGoal_Click);
            // 
            // btnClrPower
            // 
            this.btnClrPower.Location = new System.Drawing.Point(451, 146);
            this.btnClrPower.Name = "btnClrPower";
            this.btnClrPower.Size = new System.Drawing.Size(102, 47);
            this.btnClrPower.TabIndex = 14;
            this.btnClrPower.Text = "Clear Power Position";
            this.btnClrPower.UseVisualStyleBackColor = true;
            this.btnClrPower.Click += new System.EventHandler(this.btnClrPower_Click);
            // 
            // btnClearDraw
            // 
            this.btnClearDraw.Location = new System.Drawing.Point(451, 219);
            this.btnClearDraw.Name = "btnClearDraw";
            this.btnClearDraw.Size = new System.Drawing.Size(102, 47);
            this.btnClearDraw.TabIndex = 15;
            this.btnClearDraw.Text = "Clear Obstacle";
            this.btnClearDraw.UseVisualStyleBackColor = true;
            this.btnClearDraw.Click += new System.EventHandler(this.btnClearDraw_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(343, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 47);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save Draw";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnWhereCar
            // 
            this.btnWhereCar.Location = new System.Drawing.Point(451, 3);
            this.btnWhereCar.Name = "btnWhereCar";
            this.btnWhereCar.Size = new System.Drawing.Size(102, 47);
            this.btnWhereCar.TabIndex = 16;
            this.btnWhereCar.Text = "Where am I ?";
            this.btnWhereCar.UseVisualStyleBackColor = true;
            this.btnWhereCar.Click += new System.EventHandler(this.btnWhereCar_Click_1);
            // 
            // castecMapUI1
            // 
            this.castecMapUI1.AutoSize = true;
            this.castecMapUI1.CarShape = MapGL.CastecMapUI.Shape.Ellipse;
            this.castecMapUI1.ColorAxisX = System.Drawing.Color.Red;
            this.castecMapUI1.ColorAxisY = System.Drawing.Color.Green;
            this.castecMapUI1.ColorBackground = System.Drawing.Color.White;
            this.castecMapUI1.ColorCarIcon = System.Drawing.Color.DarkBlue;
            this.castecMapUI1.ColorGoalIcon = System.Drawing.Color.Lime;
            this.castecMapUI1.ColorGrid = System.Drawing.Color.Silver;
            this.castecMapUI1.ColorObstacle = System.Drawing.Color.Maroon;
            this.castecMapUI1.ColorPowerIcon = System.Drawing.Color.Orange;
            this.castecMapUI1.ColorTextPoint = System.Drawing.Color.Yellow;
            this.castecMapUI1.CountTotalCar = ((uint)(0u));
            this.castecMapUI1.CountTotalPower = ((uint)(1u));
            this.castecMapUI1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.castecMapUI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.castecMapUI1.EnableAxis = true;
            this.castecMapUI1.EnableCar = true;
            this.castecMapUI1.EnableGoal = true;
            this.castecMapUI1.EnableGrid = true;
            this.castecMapUI1.EnableMouseLocatation = true;
            this.castecMapUI1.EnablePower = true;
            this.castecMapUI1.FontText = new System.Drawing.Font("Arial", 12F);
            this.castecMapUI1.GoalShape = MapGL.CastecMapUI.Shape.Ellipse;
            this.castecMapUI1.Location = new System.Drawing.Point(0, 0);
            this.castecMapUI1.Margin = new System.Windows.Forms.Padding(0);
            this.castecMapUI1.MaxPos = new System.Drawing.Point(200, 200);
            this.castecMapUI1.MinPos = new System.Drawing.Point(-200, -200);
            this.castecMapUI1.Name = "castecMapUI1";
            pos1.id = ((uint)(0u));
            pos1.name = null;
            pos1.theta = 0D;
            pos1.x = 0D;
            pos1.y = 0D;
            this.castecMapUI1.PosCar = pos1;
            this.castecMapUI1.Resolution = 1;
            this.castecMapUI1.Size = new System.Drawing.Size(571, 416);
            this.castecMapUI1.SizeCar = new System.Drawing.Size(450, 450);
            this.castecMapUI1.SizeGoal = new System.Drawing.Size(500, 500);
            this.castecMapUI1.SizeGrid = 100;
            this.castecMapUI1.TabIndex = 0;
            this.castecMapUI1.Zoom = 1D;
            this.castecMapUI1.MouseSelectObj += new MapGL.CastecMapUI.DelMouseSelectObj(this.castecMapUI1_MouseSelectObj);
            this.castecMapUI1.MouseClickRealPos += new MapGL.CastecMapUI.DelMouseClickRealPos(this.castecMapUI1_MouseClickRealPos);
            this.castecMapUI1.MouseSelectRange += new MapGL.CastecMapUI.DelMouseSelectRange(this.castecMapUI1_MouseSelectRange);
            this.castecMapUI1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.castecMapUI1_MouseDown);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 416);
            this.Controls.Add(this.btnWhereCar);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClearDraw);
            this.Controls.Add(this.btnSetPowerPosition);
            this.Controls.Add(this.btnClrPower);
            this.Controls.Add(this.btnClrGoal);
            this.Controls.Add(this.btnSetGoalPosition);
            this.Controls.Add(this.btnSetCarPosition);
            this.Controls.Add(this.lbSetPowerY);
            this.Controls.Add(this.lbSetGoalY);
            this.Controls.Add(this.lbSetCarY);
            this.Controls.Add(this.lbSetPowerX);
            this.Controls.Add(this.lbSetGoalX);
            this.Controls.Add(this.lbSetCarX);
            this.Controls.Add(this.txtSetPowerY);
            this.Controls.Add(this.txtSetGoalY);
            this.Controls.Add(this.txtSetCarY);
            this.Controls.Add(this.txtSetPowerX);
            this.Controls.Add(this.lbSetPowerTheta);
            this.Controls.Add(this.txtSetGoalX);
            this.Controls.Add(this.lbSetGoalTheta);
            this.Controls.Add(this.txtSetPowerTheta);
            this.Controls.Add(this.txtSetCarX);
            this.Controls.Add(this.txtSetGoalTheta);
            this.Controls.Add(this.lbSetCarTheta);
            this.Controls.Add(this.txtSetCarTheta);
            this.Controls.Add(this.castecMapUI1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CastecMapUI castecMapUI1;
        private System.Windows.Forms.Label lbSetCarY;
        private System.Windows.Forms.Label lbSetCarX;
        private System.Windows.Forms.TextBox txtSetCarY;
        private System.Windows.Forms.TextBox txtSetCarX;
        private System.Windows.Forms.Label lbSetCarTheta;
        private System.Windows.Forms.TextBox txtSetCarTheta;
        private System.Windows.Forms.Button btnSetCarPosition;
        private System.Windows.Forms.TextBox txtSetGoalTheta;
        private System.Windows.Forms.Label lbSetGoalTheta;
        private System.Windows.Forms.TextBox txtSetGoalX;
        private System.Windows.Forms.TextBox txtSetGoalY;
        private System.Windows.Forms.Label lbSetGoalX;
        private System.Windows.Forms.Label lbSetGoalY;
        private System.Windows.Forms.Button btnSetGoalPosition;
        private System.Windows.Forms.TextBox txtSetPowerTheta;
        private System.Windows.Forms.Label lbSetPowerTheta;
        private System.Windows.Forms.TextBox txtSetPowerX;
        private System.Windows.Forms.TextBox txtSetPowerY;
        private System.Windows.Forms.Label lbSetPowerX;
        private System.Windows.Forms.Label lbSetPowerY;
        private System.Windows.Forms.Button btnSetPowerPosition;
        private System.Windows.Forms.Button btnClrGoal;
        private System.Windows.Forms.Button btnClrPower;
        private System.Windows.Forms.Button btnClearDraw;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnWhereCar;
    }
}