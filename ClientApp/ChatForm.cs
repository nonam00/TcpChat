using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ClientApp
{
    record ListBoxItem(string Message, Color Color);
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

            listBox1.DrawMode = DrawMode.OwnerDrawFixed;

            Thread thread = new Thread(() => ReseiveMessageAsync(_reader));
            thread.Start();
        }

        private async void ReseiveMessageAsync(StreamReader reader)
        {
            Color messageColor = Color.Black;
            Regex regex = new Regex(@"^<.+>$");
            int argb;

            while (true)
            {
                string? message = await reader.ReadLineAsync();
                if (String.IsNullOrEmpty(message))
                {
                    continue;
                }

                // parsing user color for messages
                if (regex.IsMatch(message))
                {
                    argb = int.Parse(message.Skip(1).SkipLast(1).ToArray());
                    messageColor = Color.FromArgb(argb);
                    continue;
                }

                PrintMessage(message, messageColor);
                messageColor = Color.Black;
            }
        }

        private void PrintMessage(string message, Color color)
        {
            listBox1.Items.Add(new ListBoxItem(message + "\n", color));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string message = richTextBox1.Text.Trim();
            if (message.Length > 0)
            {
                await _writer.WriteLineAsync(_name + "\n" + message);
                await _writer.FlushAsync();
                PrintMessage($"Me: {message}", Color.Black);
                richTextBox1.Text = "";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _reader.Close();
            _writer.Close();
        }

        // manualy writing string when adding message to the list
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBoxItem item = listBox1.Items[e.Index] as ListBoxItem;
            if (item != null)
            {
                e.Graphics.DrawString(item.Message, listBox1.Font, new SolidBrush(item.Color),
                    0, e.Index * listBox1.ItemHeight);
            }
        }
    }
}
