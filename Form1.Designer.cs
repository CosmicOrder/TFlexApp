namespace TFlexApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TextLength = new TextBox();
            TextHeight = new TextBox();
            TextThickness = new TextBox();
            TextHoleDiameter = new TextBox();
            CheckHasHole = new CheckBox();
            LabelLength = new Label();
            LabelHeight = new Label();
            LabelThickness = new Label();
            LabelHoleDiameter = new Label();
            LabelHasHole = new Label();
            buttonRun = new Button();
            SuspendLayout();
            // 
            // TextLength
            // 
            TextLength.Location = new Point(115, 54);
            TextLength.Name = "TextLength";
            TextLength.Size = new Size(100, 23);
            TextLength.TabIndex = 0;
            TextLength.TextChanged += TextLength_TextChanged;
            // 
            // TextHeight
            // 
            TextHeight.Location = new Point(115, 76);
            TextHeight.Name = "TextHeight";
            TextHeight.Size = new Size(100, 23);
            TextHeight.TabIndex = 1;
            TextHeight.TextChanged += TextHeight_TextChanged;
            // 
            // TextThickness
            // 
            TextThickness.Location = new Point(115, 96);
            TextThickness.Name = "TextThickness";
            TextThickness.Size = new Size(100, 23);
            TextThickness.TabIndex = 2;
            TextThickness.TextChanged += TextThickness_TextChanged;
            // 
            // TextHoleDiameter
            // 
            TextHoleDiameter.Enabled = false;
            TextHoleDiameter.Location = new Point(454, 96);
            TextHoleDiameter.Name = "TextHoleDiameter";
            TextHoleDiameter.Size = new Size(100, 23);
            TextHoleDiameter.TabIndex = 3;
            TextHoleDiameter.TextChanged += TextHoleDiameter_TextChanged;
            // 
            // CheckHasHole
            // 
            CheckHasHole.AutoSize = true;
            CheckHasHole.Location = new Point(454, 58);
            CheckHasHole.Name = "CheckHasHole";
            CheckHasHole.Size = new Size(15, 14);
            CheckHasHole.TabIndex = 4;
            CheckHasHole.UseVisualStyleBackColor = true;
            CheckHasHole.CheckedChanged += CheckBoxHasHole_CheckedChanged;
            // 
            // LabelLength
            // 
            LabelLength.AutoSize = true;
            LabelLength.Location = new Point(64, 56);
            LabelLength.Name = "LabelLength";
            LabelLength.Size = new Size(45, 15);
            LabelLength.TabIndex = 5;
            LabelLength.Text = "Длина:";
            LabelLength.Click += LabelLength_Click;
            // 
            // LabelHeight
            // 
            LabelHeight.AutoSize = true;
            LabelHeight.Location = new Point(59, 79);
            LabelHeight.Name = "LabelHeight";
            LabelHeight.Size = new Size(50, 15);
            LabelHeight.TabIndex = 6;
            LabelHeight.Text = "Высота:";
            LabelHeight.Click += LabelHeight_Click;
            // 
            // LabelThickness
            // 
            LabelThickness.AutoSize = true;
            LabelThickness.Location = new Point(47, 99);
            LabelThickness.Name = "LabelThickness";
            LabelThickness.Size = new Size(62, 15);
            LabelThickness.TabIndex = 7;
            LabelThickness.Text = "Толщина:";
            LabelThickness.Click += LabelThickness_Click;
            // 
            // LabelHoleDiameter
            // 
            LabelHoleDiameter.AutoSize = true;
            LabelHoleDiameter.Location = new Point(326, 99);
            LabelHoleDiameter.Name = "LabelHoleDiameter";
            LabelHoleDiameter.Size = new Size(122, 15);
            LabelHoleDiameter.TabIndex = 8;
            LabelHoleDiameter.Text = "Диаметра отверстия:";
            LabelHoleDiameter.Click += LabelHoleDiameter_Click;
            // 
            // LabelHasHole
            // 
            LabelHasHole.AutoSize = true;
            LabelHasHole.Location = new Point(331, 56);
            LabelHasHole.Name = "LabelHasHole";
            LabelHasHole.Size = new Size(117, 15);
            LabelHasHole.TabIndex = 9;
            LabelHasHole.Text = "Наличие отверстия:";
            LabelHasHole.Click += LabelHasHole_Click;
            // 
            // buttonRun
            // 
            buttonRun.Location = new Point(189, 162);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new Size(217, 37);
            buttonRun.TabIndex = 10;
            buttonRun.Text = "ПОСТРОИТЬ";
            buttonRun.UseVisualStyleBackColor = true;
            buttonRun.Click += ButtonRun_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(608, 249);
            Controls.Add(buttonRun);
            Controls.Add(LabelHasHole);
            Controls.Add(LabelHoleDiameter);
            Controls.Add(LabelThickness);
            Controls.Add(LabelHeight);
            Controls.Add(LabelLength);
            Controls.Add(CheckHasHole);
            Controls.Add(TextHoleDiameter);
            Controls.Add(TextThickness);
            Controls.Add(TextHeight);
            Controls.Add(TextLength);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextLength;
        private TextBox TextHeight;
        private TextBox TextThickness;
        private TextBox TextHoleDiameter;
        private CheckBox CheckHasHole;
        private Label LabelLength;
        private Label LabelHeight;
        private Label LabelThickness;
        private Label LabelHoleDiameter;
        private Label LabelHasHole;
        private Button buttonRun;
    }
}
