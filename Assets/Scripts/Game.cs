using UnityEngine;
using System;
using System.Collections.Generic;

public class OnlineGame
{
    public int myPlayerNum;
    public List<string> playerNames;
}

public class Game : MonoBehaviour
{
    public static FInt TIMESTEP = new FInt(0.01666f);

    public static Game instance = null;
    public static int numPlayers = 2;
    public static bool isPlaying = false;
    public static bool isOnline = false;

    public int localPlayerNum = 0;

    [SerializeField] private World world;

    private OnlineNetwork network = new OnlineNetwork();
    private List<InputModule> inputModules = new List<InputModule>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Player> players = new List<Player>();
    private List<SwordPart> swordParts = new List<SwordPart>();
    private List<Spawner> spawners = new List<Spawner>();

    private int frameNumber = 0;
    private InputModule localPlayerInput = null;
    
    void OnApplicationQuit()
    {
        network.StopNetworking();
    }

    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        network.Advance();
        
        if (!isPlaying) return;

        // If we're missing frames then skip the update!
        if (isOnline)
        {
            bool hasFrames = true;
            for (int pnum = 0; pnum < numPlayers; ++pnum)
            {
                if (!network.HasFrame(pnum, frameNumber))
                {
                    // Ask politely using TCP
                    network.AskForFrame(pnum, frameNumber);
                    hasFrames = false;
                }
            }
            if (!hasFrames) return;
        }

        world.Advance();
        PickUpSwordParts();
        foreach (Spawner spawner in spawners)
        {
            spawner.Advance();
        }
        foreach (InputModule inputModule in inputModules)
        {
            inputModule.Advance(frameNumber);
        }
        foreach (Player player in players)
        {
            player.Advance();
        }
        foreach (Enemy enemy in enemies)
        {
            enemy.Advance();
        }

        frameNumber += 1;
    }

    public void AddSpawner(Spawner spawner)
    {
        spawners.Add(spawner);
    }

    public void StartGame(int numPlayers, OnlineGame onlineGame)
    {
        isOnline = onlineGame != null;

        frameNumber = 0;
        Game.numPlayers = numPlayers;

        world.SetPlayersAndCameras();

        List<GameObject> playerObjects = world.GetPlayers();
        for (int pnum = 0; pnum < numPlayers; ++pnum)
        {
            Player p = playerObjects[pnum].GetComponent<Player>();
            InputModule i;
            if (isOnline)
            {
                if (onlineGame.myPlayerNum == pnum)
                {
                    // This forces the local player to accept input from p1 controls
                    i = new InputModule(network, true, pnum);
                    localPlayerInput = i;
                    localPlayerNum = pnum;
                }
                else
                {
                    i = new InputModule(network, false, pnum);
                }
                p.Setup(i, FInt.Zero(), FInt.Zero(), pnum, onlineGame.playerNames[pnum]);
            }
            else
            {
                i = new InputModule(network, true, pnum);
                p.Setup(i, FInt.Zero(), FInt.Zero(), pnum, string.Format("Player {0}", pnum));
                OnlineNetwork.seed = new System.Random();
            }
            players.Add(p);
            inputModules.Add(i);
        }

        isPlaying = true;
        if (isOnline)
        {
            for (int i = 0; i < OnlineNetwork.bufferSize; ++i)
            {
                localPlayerInput.Advance(0);
            }
        }
    }

    /// <summary>
    /// Only call this from panels
    /// </summary>
    public OnlineNetwork GetNetwork()
    {
        return network;
    }

    private void PickUpSwordParts()
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < swordParts.Count; ++i)
            {
                SwordPart part = swordParts[i];
                if (Collision.CircleToCircle(
                        part.position,
                        player.position,
                        FInt.Zero(),
                        player.radius))
                {
                    player.sword.AddPart(part);
                    swordParts.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }

    public FVector GetWrappedPosition(FVector pos)
    {
        return world.GetWrappedPosition(pos);
    }

    public void SummonEnemy(GameObject enemyObject, FVector pos)
    {
        Enemy enemy = world.SummonEnemy(enemyObject, pos);
        enemy.Setup(pos);
        enemies.Add(enemy);
    }

    private void DropPart(GameObject swordPartObject, FVector pos)
    {
        SwordPart part = world.DropPart(swordPartObject, pos);
        part.Setup(pos);
        swordParts.Add(part);
    }

    public void Attack(
        int sourceTeam,
        FVector source,
        FInt radius,
        FInt damage)
    {
        // Generate a list of characters to do a generic hit test
        Character[] characters = new Character[players.Count + enemies.Count];
        int i = 0;
        foreach (Player player in players)
        {
            characters[i] = player;
            ++i;
        }
        foreach (Enemy enemy in enemies)
        {
            characters[i] = enemy;
            ++i;
        }
        for (i = 0; i < players.Count + enemies.Count; ++i)
        {
            Character character = characters[i];
            if (character.team != sourceTeam &&
                character.invincibility.rawValue <= 0 &&
                Collision.CircleToCircle(
                    source,
                    character.position,
                    radius,
                    character.radius))
            {
                character.Damage(damage);
            }
        }
    }

    public void KillEnemy(Enemy enemy)
    {
        DropPart(null, enemy.position);
        enemies.Remove(enemy);
        world.KillEnemy(enemy.gameObject);
    }

    public bool IsPlayerInRange(FVector pos, FInt radius)
    {
        foreach (Player p in players)
        {
            if (Collision.CircleToCircle(p.position, pos, Player.PRADIUS, radius))
            {
                return true;
            }
        }
        return false;
    }

    public FVector GetNearestPlayerPosition(FVector position)
    {
        FVector pos = new FVector(players[0].position);
        FInt dist = Collision.Distance(pos.x, pos.y, position.x, position.y);
        foreach (Player player in players)
        {
            FInt newDist = Collision.Distance(player.position.x, player.position.y, position.x, position.y);
            if (dist > newDist)
            {
                dist = newDist;
                pos = new FVector(player.position);
            }
        }
        return pos;
    }
}
