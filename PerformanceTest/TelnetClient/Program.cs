using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static long m_RequestCount;
        private static long m_ResponseCount;

        private static Timer m_Timer;

        private static bool m_Stopped;

        private static Socket[] m_Sockets;

        private static System.Threading.Semaphore m_ConnectSemaphore;

        static void Main(string[] args)
        {
            var connectionNumber = int.Parse(ConfigurationManager.AppSettings["connectionNumber"]);

            Console.WriteLine("Connecting...");

            m_Sockets = new Socket[connectionNumber];
            m_ConnectSemaphore = new Semaphore(0, connectionNumber);

            for (var i = 0; i < connectionNumber; i++)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012), OnClientConnected, Tuple.Create(socket, i));
            }

            var finishConnect = 0;

            while (finishConnect < connectionNumber)
            {
                m_ConnectSemaphore.WaitOne();
                finishConnect++;
            }

            var connected = m_Sockets.Count(s => s != null);

            Console.WriteLine("{0} connected, {1} not connected!", connected, m_Sockets.Length - connected);

            m_Timer = new Timer(OnTimerCallback, null, 5000, 5000);

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

        private static void OnClientConnected(IAsyncResult result)
        {
            var state = result.AsyncState as Tuple<Socket, int>;

            var socket = state.Item1;

            try
            {
                socket.EndConnect(result);

                if (socket.Connected)
                {
                    m_Sockets[state.Item2] = socket;
                }
            }
            catch
            {

            }
            finally
            {
                m_ConnectSemaphore.Release();
            }

            if (socket.Connected)
            {
                StartTest(socket);
            }
        }

        private static async void StartTest(Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            using (var reader = new StreamReader(stream, new UTF8Encoding(), true, 2048))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(), 2048))
            {
                string line;

                while (!m_Stopped)
                {
                    line = Guid.NewGuid().ToString();
                    writer.Write("ECHO " + line + "\r\n");
                    await writer.FlushAsync();

                    Interlocked.Increment(ref m_RequestCount);

                    var receivedLine = await reader.ReadLineAsync();

                    Interlocked.Increment(ref m_ResponseCount);

                    if (!line.Equals(receivedLine))
                        break;
                }
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private static void OnTimerCallback(object state)
        {
            Console.WriteLine("Request: {0}, Response: {1}", m_RequestCount, m_ResponseCount);
        }
    }
}
