using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client
{ 
    public static string GetDataFromServer(string message, int port = 3228)
    {
        byte[] bytes = new byte[1024];
        
        IPHostEntry ipHost = Dns.GetHostEntry("localhost");
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            
        Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        sender.Connect(ipEndPoint);

        Debug.Log("Сокет соединяется с " + sender.RemoteEndPoint);
        byte[] msg = Encoding.UTF8.GetBytes(message);
        
        sender.Send(msg);
        
        int bytesRec = sender.Receive(bytes);
        
        if (message.Contains("receive"))
        {
            BinaryWriter bWrite = new BinaryWriter(File.Open("D:\\Reports\\Report.csv", FileMode.OpenOrCreate));
            message = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            bWrite.Write(message);
        }
        else
        {
            message = Encoding.UTF8.GetString(bytes, 0, bytesRec);
        }
        
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();

        return message;
    }
}

