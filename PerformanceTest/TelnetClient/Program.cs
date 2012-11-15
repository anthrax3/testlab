using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelnetClient
{
    class Program
    {
        private static int m_RequestCount;

        private static Timer m_Timer;

        private static bool m_Stopped;

        private static string m_CurrentLine;

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Console.WriteLine("Connected");

            m_Timer = new Timer(OnTimerCallback, null, 5000, 5000);

            ThreadPool.QueueUserWorkItem(StartTest, socket);

            while (true)
            {
                var line = Console.ReadLine();

                if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    m_Stopped = true;
                    break;
                }
            }

            if (m_Timer != null)
                m_Timer.Change(Timeout.Infinite, Timeout.Infinite);

            Thread.Sleep(2000);
        }

        private static void StartTest(object state)
        {
            var socket = state as Socket;
            using (var stream = new NetworkStream(socket))
            using (var reader = new StreamReader(stream, new UTF8Encoding(), true, 2048))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(), 2048))
            {
                string line;

                while (!m_Stopped)
                {
                    line = "ECHO " + Guid.NewGuid().ToString() + "\r\n";
                    writer.Write(line);
                    writer.Flush();

                    m_CurrentLine = line;
                    line = reader.ReadLine();
                    Interlocked.Increment(ref m_RequestCount);
                }
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private static void OnTimerCallback(object state)
        {
            Console.WriteLine(m_RequestCount + " " + m_CurrentLine);
        }
    }
}
