namespace ClientApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length > 0)
            {
                this.Hide();
                var chatForm = new ChatForm(textBox1.Text.Trim());
                chatForm.ShowDialog();
                this.Close();
            }
        }
    }
}
