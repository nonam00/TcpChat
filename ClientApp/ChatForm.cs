using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
    public partial class ChatForm : Form
    {
        private readonly string _name;
        private readonly TcpClient _client;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        public ChatForm(string name)
        {
            InitializeComponent();
            _name = name;

            _client = new TcpClient();

            _client.Connect(IPAddress.Loopback, 5000);
            _writer = new StreamWriter(_client.GetStream());
            _reader = new StreamReader(_client.GetStream());

            if (_writer is null || _reader is null)
            {
                throw new Exception();
            }

            Thread thread = new Thread(() => ReseiveMessageAsync(_reader));
            thread.Start();
        }

        private async void ReseiveMessageAsync(StreamReader reader)
        {
            while (true)
            {
                string? message = await reader.ReadLineAsync();
                if (String.IsNullOrEmpty(message))
                {
                    continue;
                }
                PrintMessage(message);
            }
        }

        private void PrintMessage(string message)
        {
            listBox1.Items.Add(message + "\n");
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string message = richTextBox1.Text.Trim();
            if (message.Length > 0)
            {
                await _writer.WriteLineAsync(_name + "\n" + message);
                await _writer.FlushAsync();
                PrintMessage($"Me: {message}");
                richTextBox1.Text = "";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _reader.Close();
            _writer.Close();
        }
    }
}
