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

    private static SocketHandler.Client clientSocket;
    private static Dictionary<string, Action<string[]>> routes =
        new Dictionary<string, Action<string[]>>()
    {
        { "ng", NewGame },
        { "g", GetGameMessage },
    };

    private static List<int> playerFrames = new List<int>();
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
        playerFrames[Game.instance.localPlayerNum - 1] += 1;
        clientSocket.SendData("g " + inputs);
    }

    public static int GetMinimumFrame(int frameNumber)
    {
        int min = playerFrames[0];
        string[] frameStrings = new string[playerFrames.Count];
        for (int i = 1; i < playerFrames.Count; ++i)
        {
            if (playerFrames[i] < min)
            {
                min = playerFrames[i];
            }
            frameStrings[i] = playerFrames[i].ToString();
        }
        Debug.Log(string.Format(
            "Current Frame: {0} | Min: {1} | All Frame Counts: {2}",
            frameNumber.ToString(),
            min.ToString(),
            string.Join(", ", frameStrings)));
        return min;
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

        UIManager.instance.ClosePanel("Online Setup");
        UIManager.instance.ClosePanel("Title Screen");
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
        playerFrames[pnum - 1] += 1;
        Game.instance.GameMessage(pnum, inputs);
    }
}
