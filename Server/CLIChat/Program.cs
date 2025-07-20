using System.Net.Http;
using System.Net.WebSockets;
using System;
using System.Net.Sockets;
using System.Net;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace CLIChat
{
    internal class Program
    {
        static List<StreamWriter> clients = new();
        static object lockObj = new();
        static void Main(string[] args)
        {
            
            Console.WriteLine("starting chat server init - please wait.");
            TcpListener listener = new TcpListener(IPAddress.Any, 19420);
            Console.WriteLine("initializing tcp...");
            listener.Start();
            // test bullshit
            TcpClient testClient = new TcpClient("127.0.0.1", 19420);
            using NetworkStream ns = testClient.GetStream();
            using StreamWriter writer = new(ns) { AutoFlush = true };
            writer.WriteLine("test completed! should now be able to accept clients.");
            // test bullshit end
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("client connected.");

                Thread clientThread = new(() => HandleClient(client));
                clientThread.Start();
            }
        }

            static void HandleClient(TcpClient client)
            {
                using NetworkStream networkStream = client.GetStream();
                using StreamReader streamReader = new(networkStream);
                using StreamWriter streamWriter = new(networkStream) { AutoFlush = true };

                lock (lockObj) clients.Add(streamWriter);
                try
                {
                    while (true)
                    {
                        string? msg = streamReader.ReadLine();
                        if (msg == null) break;

                        Console.WriteLine(msg);

                        lock (lockObj)
                        {
                            foreach (var w in clients)
                            {
                                try { w.WriteLine(msg); } catch { }
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                lock (lockObj) clients.Remove(streamWriter);
                Console.WriteLine("client disconnected");
            }

        }
    }


    

