using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

class ChatClient
{
    static void Main()
    { 
        Console.Write("Enter server IP (defaults to localhost): ");
        string ip = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(ip)) ip = "127.0.0.1";
        Console.WriteLine("enter nickname");
        string name = Console.ReadLine();

        TcpClient client = new TcpClient(ip, 19420);
        using NetworkStream ns = client.GetStream();
        using StreamReader reader = new(ns);
        using StreamWriter writer = new(ns) { AutoFlush = true };

        Console.WriteLine("connected to chat. type to send messages.");

        new Thread(() =>
        {
            try
            {
                string? msg;
                while ((msg = reader.ReadLine()) != null)
                    Console.WriteLine(msg);
            }
            catch { }
        }).Start();

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            writer.WriteLine(name + ": " + input + " | At " + DateTime.Now);
        }
    }
}
