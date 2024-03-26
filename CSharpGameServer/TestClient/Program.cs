
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string ip = "127.0.0.1";
        int port = 10001;

        TcpClient client = new TcpClient(ip, port);
        NetworkStream stream = client.GetStream();

        string message = "testString";
        byte[] sendData = Encoding.ASCII.GetBytes(message);
        stream.Write(sendData, 0, sendData.Length);

        byte[] recvData = new byte[512];
        StringBuilder builder = new StringBuilder();
        int recvBytes = stream.Read(recvData, 0, recvData.Length);
        builder.Append(Encoding.ASCII.GetString(recvData, 0, recvBytes));

        Console.WriteLine("Received : {0}", recvData.ToString());

        stream.Close();
        client.Close();
    }
}