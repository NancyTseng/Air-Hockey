﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ConsoleApp
{
    class Cons
    {
        public Cons() { }
        static void Main()
        {
            System.Net.IPAddress ip;
            Console.WriteLine("SERVER 1");
            Console.WriteLine("Enter IP address:");
            string userIP = Console.ReadLine();
            ip = System.Net.IPAddress.Parse(userIP);
            TcpListener listener = new TcpListener(ip, 36000);
            Console.WriteLine("Waiting for host");
            listener.Start();
            TcpClient client1 = listener.AcceptTcpClient();
            Console.WriteLine("Connected");
            Console.WriteLine("Waiting for player");
            listener.Start();
            TcpClient client2 = listener.AcceptTcpClient();
            Console.WriteLine("Connected");
            if (client1.Connected && client2.Connected)
            {
                while(true)
                {
                    NetworkStream nwstream = client1.GetStream();
                    NetworkStream nwstream1 = client2.GetStream();
                    byte[] buffer = new byte[client1.ReceiveBufferSize];                 
                    int bytesread = nwstream.Read(buffer, 0, 3);
                    string datareceived = Encoding.ASCII.GetString(buffer, 0, bytesread);
                    byte[] buffer1 = Encoding.ASCII.GetBytes(datareceived);
                    nwstream1.Write(buffer1, 0, buffer1.Length);
                    Console.WriteLine(datareceived);
                }
            }
        }
    }
}
