using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkingManager
{
    private const string DNS_NAME = "progressiongames.servegame.org";
    private const int PORT = 5287;

    public static System.Random seed = null;

    private static string username = "Guest";
    private static SocketHandler.Client clientSocket;

    public static bool StartNetworking()
    {
        //clientSocket = new SocketHandler.Client(Dns.GetHostAddresses(DNS_NAME)[0], PORT);
        
        // This section finds the local IP address.
        // This is only used when the server is on the same computer.
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
        
        if (!clientSocket.isRunning)
        {
            Debug.LogError("Failed to connect to server");
            clientSocket = null;
            return false;
        }

        // Set up the connection. First say hello!
        clientSocket.SendData("queue " + username);

        return true;
    }

    public static void StopNetworking()
    {
        if (clientSocket != null)
        {
            if (clientSocket.isRunning)
            {
                clientSocket.Stop();
            }
            clientSocket = null;
        }
    }

    public static void WaitForInput()
    {
        throw new NotImplementedException();
    }

    public static void SetUsername(string name)
    {
        username = name;
    }
}
