using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class OnlineNetwork
{
    private const string DNS_NAME = "progressiongames.servegame.org";
    private const int PORT = 5287;
    private const int UDP_SEND_COUNT = 5;
    private const float FRAME_ASK_DELAY = 1.0f;
    
    public static System.Random seed = null;
    public static int bufferSize = 10;

    private SocketHandler.UDPController udp = null;
    private SocketHandler.Client clientSocket = null;
    private Dictionary<string, Action<string[]>> routes;

    private InputTracker inputTracker = null;
    private Queue<string> messageQueue = new Queue<string>();
    private int localPlayerNum;

    private float askTimer = 0.0f;

    public OnlineNetwork()
    {
        routes = new Dictionary<string, Action<string[]>>()
        {
            { "ng", NewGame },
            { "f", SendFrame }
        };
    }

    public void Advance()
    {
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            string[] args = message.Split(' ');
            routes[args[0]](args);
        }
    }

    public bool StartNetworking()
    {
        //IPAddress ipAddress = Dns.GetHostAddresses(DNS_NAME)[0];
        
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
        clientSocket.onReceiveData += (m) => messageQueue.Enqueue(m);

        udp = new SocketHandler.UDPController(clientSocket.socket);

        return true;
    }

    public void StopNetworking()
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

    public void SetUsername(string name)
    {
        if (clientSocket != null)
        {
            clientSocket.SendData("name " + name);
        }
    }

    public void StartSearching(List<int> gameModes)
    {
        string[] modeStrings = new string[gameModes.Count];
        for (int i = 0; i < gameModes.Count; ++i)
        {
            modeStrings[i] = gameModes[i].ToString();
        }
        clientSocket.SendData("queue " + string.Join(",", modeStrings));
    }

    public void SendGameMessage(InputSegment segment)
    {
        int frame = inputTracker.SaveInput(localPlayerNum, segment);
        byte[] data = inputTracker.GetData(localPlayerNum, frame);
        for (int i = 0; i < UDP_SEND_COUNT; ++i)
        {
            udp.SendData(data);
        }
    }

    public bool HasFrame(int pnum, int frame)
    {
        return inputTracker.HasFrame(pnum, frame);
    }

    public InputSegment GetInput(int pnum, int frame)
    {
        return inputTracker.GetInput(pnum, frame);
    }

    public void AskForFrame(int pnum, int frame)
    {
        if (Time.fixedTime >= askTimer + FRAME_ASK_DELAY)
        {
            clientSocket.SendData(string.Format("g {0} {1}", pnum, frame));
            askTimer = Time.fixedTime;
        }
    }

    private void NewGame(string[] args)
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

        localPlayerNum = gameInfo.myPlayerNum;

        inputTracker = new InputTracker();
        inputTracker.Setup(numPlayers);
        udp.onReceiveData = inputTracker.AddInput;

        UIManager.instance.ClosePanel(PanelType.ONLINE_SETUP);
        UIManager.instance.ClosePanel(PanelType.TITLE_SCREEN);
        Game.instance.StartGame(numPlayers, gameInfo);
    }

    private void SendFrame(string[] args)
    {
        int frame = Convert.ToInt32(args[1]);
        if (inputTracker.HasFrame(localPlayerNum, frame))
        {
            byte[] data = inputTracker.GetData(localPlayerNum, frame);
            for (int i = 0; i < UDP_SEND_COUNT; ++i)
            {
                udp.SendData(data);
            }
        }
        else
        {
            Debug.LogError("Asked to send frame but haven't computed it yet!");
        }
    }
}
