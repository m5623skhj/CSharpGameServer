using TestClient.Client;

namespace TestClient.Main
{
    internal class Program
    {
        private static ChattingClient? client;

        private static string ip = "127.0.0.1";
        private static int port = 10001;

        private static void Main(string[] args)
        {
            client = new ChattingClient(ip, port);

            while (client.Connect())
            {
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                    {
                        Console.Write("Enter your name: ");
                        var name = Console.ReadLine() ?? "";
                        client.SetMyName(name);
                        break;
                    }
                    case "2":
                    {
                        Console.Write("Enter room name to create: ");
                        var roomName = Console.ReadLine() ?? "";
                        client.CreateRoom(roomName);
                        break;
                    }
                    case "3":
                    {
                        Console.Write("Enter room name to join: ");
                        var roomName = Console.ReadLine() ?? "";
                        client.JoinRoom(roomName);
                        break;
                    }
                    case "4":
                    {
                        client.LeaveRoom();
                        break;
                    }
                    case "5":
                    {
                        Console.Write("Enter message to send: ");
                        var message = Console.ReadLine() ?? "";
                        client.SendChatMessage(message);
                        break;
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