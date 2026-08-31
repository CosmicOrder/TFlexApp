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
            CheckBoxHasHole = new CheckBox();
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
            TextLength.Location = new Point(115, 53);
            TextLength.Name = "TextLength";
            TextLength.Size = new Size(100, 23);
            TextLength.TabIndex = 0;
            // 
            // TextHeight
            // 
            TextHeight.Location = new Point(115, 76);
            TextHeight.Name = "TextHeight";
            TextHeight.Size = new Size(100, 23);
            TextHeight.TabIndex = 1;
            // 
            // TextThickness
            // 
            TextThickness.Location = new Point(115, 99);
            TextThickness.Name = "TextThickness";
            TextThickness.Size = new Size(100, 23);
            TextThickness.TabIndex = 2;
            // 
            // TextHoleDiameter
            // 
            TextHoleDiameter.Enabled = false;
            TextHoleDiameter.Location = new Point(454, 96);
            TextHoleDiameter.Name = "TextHoleDiameter";
            TextHoleDiameter.Size = new Size(100, 23);
            TextHoleDiameter.TabIndex = 3;
            // 
            // CheckBoxHasHole
            // 
            CheckBoxHasHole.AutoSize = true;
            CheckBoxHasHole.Location = new Point(454, 58);
            CheckBoxHasHole.Name = "CheckBoxHasHole";
            CheckBoxHasHole.Size = new Size(15, 14);
            CheckBoxHasHole.TabIndex = 4;
            CheckBoxHasHole.UseVisualStyleBackColor = true;
            CheckBoxHasHole.CheckedChanged += CheckBoxHasHole_CheckedChanged;
            // 
            // LabelLength
            // 
            LabelLength.AutoSize = true;
            LabelLength.Location = new Point(64, 56);
            LabelLength.Name = "LabelLength";
            LabelLength.Size = new Size(45, 15);
            LabelLength.TabIndex = 5;
            LabelLength.Text = "Длина:";
            // 
            // LabelHeight
            // 
            LabelHeight.AutoSize = true;
            LabelHeight.Location = new Point(59, 79);
            LabelHeight.Name = "LabelHeight";
            LabelHeight.Size = new Size(50, 15);
            LabelHeight.TabIndex = 6;
            LabelHeight.Text = "Высота:";
            // 
            // LabelThickness
            // 
            LabelThickness.AutoSize = true;
            LabelThickness.Location = new Point(47, 102);
            LabelThickness.Name = "LabelThickness";
            LabelThickness.Size = new Size(62, 15);
            LabelThickness.TabIndex = 7;
            LabelThickness.Text = "Толщина:";
            // 
            // LabelHoleDiameter
            // 
            LabelHoleDiameter.AutoSize = true;
            LabelHoleDiameter.Location = new Point(326, 99);
            LabelHoleDiameter.Name = "LabelHoleDiameter";
            LabelHoleDiameter.Size = new Size(116, 15);
            LabelHoleDiameter.TabIndex = 8;
            LabelHoleDiameter.Text = "Диаметр отверстия:";
            // 
            // LabelHasHole
            // 
            LabelHasHole.AutoSize = true;
            LabelHasHole.Location = new Point(326, 56);
            LabelHasHole.Name = "LabelHasHole";
            LabelHasHole.Size = new Size(117, 15);
            LabelHasHole.TabIndex = 9;
            LabelHasHole.Text = "Наличие отверстия:";
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
            ClientSize = new Size(618, 650);
            Controls.Add(buttonRun);
            Controls.Add(LabelHasHole);
            Controls.Add(LabelHoleDiameter);
            Controls.Add(LabelThickness);
            Controls.Add(LabelHeight);
            Controls.Add(LabelLength);
            Controls.Add(CheckBoxHasHole);
            Controls.Add(TextHoleDiameter);
            Controls.Add(TextThickness);
            Controls.Add(TextHeight);
            Controls.Add(TextLength);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Построение детали";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextLength;
        private TextBox TextHeight;
        private TextBox TextThickness;
        private TextBox TextHoleDiameter;
        private CheckBox CheckBoxHasHole;
        private Label LabelLength;
        private Label LabelHeight;
        private Label LabelThickness;
        private Label LabelHoleDiameter;
        private Label LabelHasHole;
        private Button buttonRun;
        internal TFlex.Control? tfControl;
    }
}
