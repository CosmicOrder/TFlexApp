namespace TFlexApp
{
    public partial class Form1 : Form, IView
    {
        public Form1()
        {
            InitializeComponent();
            TextHoleDiameter.Enabled = false;
        }

        public void InitializeTFlexControl()
        {
            tfControl = new TFlex.Control();
            tfControl.Location = new System.Drawing.Point(12, 220);
            tfControl.Name = "tfControl";
            tfControl.Size = new System.Drawing.Size(676, 410);
            tfControl.BackColor = System.Drawing.Color.White;
            tfControl.ShowPageTabs = true;
            tfControl.ShowControlButtons = true;
            Controls.Add(tfControl);
        }

        #region Неявная реализация интерфейса IFormView
        public string PartLength => TextLength.Text;
        public string PartHeight => TextHeight.Text;
        public string PartThickness => TextThickness.Text;
        public string HoleDiameter => TextHoleDiameter.Text;
        public bool HasHole => CheckBoxHasHole.Checked;

        // Методы для вывода сообщений пользователю
        public void ShowSuccess(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowError(string errorMessage, string title)
        {
            MessageBox.Show(errorMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Объявление события RunRequested
        public event EventHandler RunRequested = delegate { };
        #endregion

        // Логика блокировки поля диаметра
        private void CheckBoxHasHole_CheckedChanged(object sender, EventArgs e)
        {
            TextHoleDiameter.Enabled = CheckBoxHasHole.Checked;
            if (!CheckBoxHasHole.Checked)
            {
                TextHoleDiameter.Text = string.Empty; // Очищаем поле ввода диаметра отверстия, если CheckBox не отмечен
            }
        }

        private void ButtonRun_Click(object sender, EventArgs e)
        {
            RunRequested.Invoke(this, EventArgs.Empty); // Испускание события RunRequested при нажатии кнопки
        }

        private void TextLength_TextChanged(object sender, EventArgs e)
        {

        }

        private void LabelHoleDiameter_Click(object sender, EventArgs e)
        {

        }
    }
}
