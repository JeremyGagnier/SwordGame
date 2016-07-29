using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetworkingManager
{
    private const string DNS_NAME = "progressiongames.servegame.org";
    private const int PORT = 5287;
    private const int UDP_SEND_COUNT = 5;

    public static System.Random seed = null;
    public static int bufferSize = 10;

    private static SocketHandler.UDPController udp = null;
    private static SocketHandler.Client clientSocket = null;
    private static Dictionary<string, Action<string[]>> routes =
        new Dictionary<string, Action<string[]>>()
    {
        { "ng", NewGame }
    };

    private static InputTracker inputTracker = null;
    private static Queue<string> messageQueue = new Queue<string>();

    public static void Advance()
    {
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            string[] args = message.Split(' ');
            routes[args[0]](args);
        }
    }

    public static bool StartNetworking()
    {
        IPAddress ipAddress = Dns.GetHostAddresses(DNS_NAME)[0];
        /*
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
        */
        clientSocket = new SocketHandler.Client(ipAddress, PORT);
        
        if (!clientSocket.isRunning)
        {
            Debug.LogError("Failed to connect to server");
            clientSocket = null;
            return false;
        }
        clientSocket.onReceiveData += (m) => messageQueue.Enqueue(m);

        udp = new SocketHandler.UDPController(clientSocket.socket);

        return true;
    }

    public static void StopNetworking()
    {
        if (udp != null)
        {
            udp.Stop();
            udp = null;
        }
        if (clientSocket != null)
        {
            if (clientSocket.isRunning)
            {
                clientSocket.Stop();
            }
            clientSocket = null;
        }
    }

    public static void SetUsername(string name)
    {
        if (clientSocket != null)
        {
            clientSocket.SendData("name " + name);
        }
    }

    public static void StartSearching(List<int> gameModes)
    {
        string[] modeStrings = new string[gameModes.Count];
        for (int i = 0; i < gameModes.Count; ++i)
        {
            modeStrings[i] = gameModes[i].ToString();
        }
        clientSocket.SendData("queue " + string.Join(",", modeStrings));
    }

    public static void SendGameMessage(InputSegment segment)
    {
        udp.SendData(inputTracker.SendInput(segment));
    }

    public static bool HasFrame(int frame)
    {
        return inputTracker.HasFrame(frame);
    }

    private static void NewGame(string[] args)
    {
        OnlineGame gameInfo = new OnlineGame();
        seed = new System.Random(Convert.ToInt32(args[1]));
        int numPlayers = Convert.ToInt32(args[2]);
        gameInfo.myPlayerNum = Convert.ToInt32(args[3]);
        gameInfo.playerNames = new List<string>();
        for (int i = 0; i < numPlayers; ++i)
        {
            gameInfo.playerNames.Add(args[i + 4]);
        }

        inputTracker = new InputTracker();
        inputTracker.Setup(numPlayers, gameInfo.myPlayerNum);
        udp.onReceiveData = inputTracker.AddInput;

        UIManager.instance.ClosePanel("Online Setup");
        UIManager.instance.ClosePanel("Title Screen");
        Game.instance.StartGame(numPlayers, gameInfo);
    }
}
