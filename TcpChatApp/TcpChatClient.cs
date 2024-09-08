using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatListenerApp
{
    public class TcpChatClient : IDisposable
    {
        private TcpClient client;
        private TcpChatServer server;
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }

        async Task SendAndWrite(string message)
        {
            Console.WriteLine(message);
            await server.MessageSendAsync(Id, message);
        }

        public TcpChatClient(TcpClient client, TcpChatServer server)
        {
            this.client = client;
            this.server = server;

            var stream = client.GetStream();
            Writer = new StreamWriter(stream);
            Reader = new StreamReader(stream);
        }

        public async Task ProcessAsync()
        {
            string? message;
            try
            {
                string? userNickname = await Reader.ReadLineAsync();
                if (userNickname is not null)
                {
                    Name = userNickname;
                    message = $"User {Name} in to chat";
                    await SendAndWrite(message);
                }

                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message is null)
                        {
                            continue;
                        }

                        message = $"{Name}: {message}";
                        await SendAndWrite(message);
                    }
                    catch (Exception ex)
                    {
                        message = $"User {Name} out from chat";
                        await SendAndWrite(message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.ClientClose(Id);
            }
        }

        public void Dispose()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
