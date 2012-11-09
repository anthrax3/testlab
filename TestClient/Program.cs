using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2020));

            Console.WriteLine("Connected");

            using (var stream = new NetworkStream(socket))
            using (var reader = new StreamReader(stream, new UTF8Encoding(), true, 2048))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(), 2048))
            {
                var line = reader.ReadLine();
                Console.WriteLine(line);

                while (true)
                {
                    line = Console.ReadLine();

                    if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        break;

                    writer.Write(line + "\r\n");
                    writer.Flush();
                    line = reader.ReadLine();

                    Console.WriteLine(line);
                }
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
