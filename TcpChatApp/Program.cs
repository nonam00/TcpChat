using TcpChatListenerApp;

TcpChatServer server = new();
await server.ListenAsync();