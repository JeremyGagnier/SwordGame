using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class NetworkingManager
{
    private const string DNS_NAME = "progressiongames.servegame.org";
    private const int PORT = 5287;

    public static System.Random seed = null;
    public static int bufferSize = 10;

    private static string username = "Guest";
    private static SocketHandler.Client clientSocket;
    private static Dictionary<string, Action<string[]>> routes =
        new Dictionary<string, Action<string[]>>()
    {
        { "ng", NewGame },
        { "g", GetGameMessage },
    };

    private static List<int> playerFrames = new List<int>();

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
        clientSocket.onReceiveData += ProcessMessage;

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

    public static void SetUsername(string name)
    {
        username = name;
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

    public static void SendGameMessage(string inputs)
    {
        clientSocket.SendData("g " + inputs);
    }

    public static int GetMinimumFrame()
    {
        int min = playerFrames[0];
        for (int i = 1; i < playerFrames.Count; ++i)
        {
            if (playerFrames[i] < min)
            {
                min = playerFrames[i];
            }
        }
        return min;
    }

    private static void ProcessMessage(string message)
    {
        string[] args = message.Split(' ');
        routes[args[0]](args);
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
            playerFrames.Add(0);
        }
        Game.instance.StartGame(numPlayers, gameInfo);
    }

    private static void GetGameMessage(string[] args)
    {
        int pnum = Convert.ToInt32(args[1]);
        string inputs = args[2];
        for (int i = 3; i < args.Length; ++i)
        {
            inputs += " " + args[i];
        }
        playerFrames[pnum] += 1;
        Game.instance.GameMessage(pnum, inputs);
    }
}
