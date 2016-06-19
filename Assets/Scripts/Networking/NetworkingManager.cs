using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkingManager
{
    private const string DNS_NAME = "progressiongames.servegame.org";
    private const int PORT = 5287;

    public static System.Random seed = null;
        
    private static SocketHandler.Client clientSocket;

    public static void StartNetworking()
    {
        clientSocket = new SocketHandler.Client(Dns.GetHostAddresses(DNS_NAME)[0], PORT);
        /*
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = null;
        foreach (IPAddress addr in ipHostInfo.AddressList)
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                ipAddress = addr;
                break;
            }
        }
        clientSocket = new SocketHandler.Client(ipAddress, PORT);
        */
        if (!clientSocket.isRunning)
        {
            Debug.LogError("Failed to connect to server");
        }
        else
        {
            Debug.LogError("Successfully connected to server");
        }
    }

    public static void WaitForInput()
    {
        throw new NotImplementedException();
    }
}
