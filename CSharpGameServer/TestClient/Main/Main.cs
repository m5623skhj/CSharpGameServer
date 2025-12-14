using TestClient.Client;

namespace TestClient.Main
{
    internal class Program
    {
        private static ChattingClient? _client;

        private const string Ip = "127.0.0.1";
        private const int Port = 10001;

        private static void Main(string[] _)
        {
            _client = new ChattingClient(Ip, Port);
            ShowMenu();

            while (_client.Connect())
            {
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                    {
                        Console.Write("Enter your name: ");
                        var name = Console.ReadLine() ?? "";
                        _client.SetMyName(name);

                        ShowMenu();

                        break;
                    }
                    case "2":
                    {
                        Console.Write("Enter room name to create: ");
                        var roomName = Console.ReadLine() ?? "";
                        _client.CreateRoom(roomName);
                        break;
                    }
                    case "3":
                    {
                        Console.Write("Enter room name to join: ");
                        var roomName = Console.ReadLine() ?? "";
                        _client.JoinRoom(roomName);

                        Console.Clear();

                        break;
                    }
                    case "4":
                    {
                        _client.LeaveRoom();
                        
                        ShowMenu();

                        break;
                    }
                    case "5":
                    {
                        Console.Write("Enter message to send: ");
                        var message = Console.ReadLine() ?? "";
                        _client.SendChatMessage(message);
                        break;
                    }
                    case "6":
                    {
                        _client.Disconnect();
                        return;
                    }
                    default:
                    {
                        Console.WriteLine("Invalid input");
                        break;
                    }
                }
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();

            Console.WriteLine("=== Chatting Client Menu ===");
            Console.WriteLine("1. Set Name");
            Console.WriteLine("2. Create Room");
            Console.WriteLine("3. Join Room");
            Console.WriteLine("4. Leave Room");
            Console.WriteLine("5. Send Chat Message");
            Console.WriteLine("6. Request Room List");
            Console.WriteLine("0. Exit");
            Console.WriteLine("============================");
            Console.Write("Select an option: ");
        }
    }
}