using System.Net;
using System.Net.Sockets;

namespace TcpChatListenerApp
{
    public class TcpChatServer
    {
        private IPAddress address = IPAddress.Loopback; //IPAddress.Parse("192.168.0.158"); //
        private int port = 5000;
        private TcpListener listener;
        private List<TcpChatClient> clients;
        private Random random = new Random();

        public TcpChatServer()
        {
            listener = new TcpListener(new IPEndPoint(address, port));
            clients = new List<TcpChatClient>();
        }

        public async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Server start...");

                while(true)
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    TcpChatClient client = new TcpChatClient(tcpClient, this, random);

                    clients.Add(client);
                    Task.Run(client.ProcessAsync);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ClientsAllClose();
            }

        }

        public async Task MessageSendAsync(string id, string message)
        {
            foreach(var client in clients)
            {
                if(client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        public void ClientClose(string id)
        {
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client is not null)
            {
                clients.Remove(client);
            }
            client?.Dispose();
        }

        public void ClientsAllClose()
        {
            Parallel.ForEach(clients, (c) => c.Dispose());
            listener.Stop();
        }
    }
}
