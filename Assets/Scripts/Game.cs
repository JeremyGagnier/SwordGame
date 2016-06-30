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

    [SerializeField] private World world;

    private List<InputModule> inputModules = new List<InputModule>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Player> players = new List<Player>();
    private List<SwordPart> swordParts = new List<SwordPart>();
    private List<Spawner> spawners = new List<Spawner>();

    private int frameNumber = 0;
    private int bufferSent = 0;
    private InputModule localPlayerInput = null;

    ~Game()
    {
        NetworkingManager.StopNetworking();
    }

    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        if (!isPlaying) return;


        if (isOnline)
        {
            // We need to send a bunch of buffered messages to reduce lag.
            if (bufferSent < NetworkingManager.bufferSize)
            {
                localPlayerInput.Advance();
                bufferSent += 1;
            }
            // If we're missing frames then skip the update!
            if (NetworkingManager.GetMinimumFrame() <= frameNumber)
            {
                return;
            }
        }

        world.Advance();
        PickUpSwordParts();
        foreach (Spawner spawner in spawners)
        {
            spawner.Advance();
        }
        foreach (InputModule inputModule in inputModules)
        {
            inputModule.Advance();
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
        frameNumber = 0;
        Game.numPlayers = numPlayers;

        world.SetPlayersAndCameras();

        List<GameObject> playerObjects = world.GetPlayers();
        for (int pnum = 0; pnum < numPlayers; ++pnum)
        {
            Player p = playerObjects[pnum].GetComponent<Player>();
            InputModule i;
            if (onlineGame != null)
            {
                if (onlineGame.myPlayerNum == pnum + 1)
                {
                    // This forces the local player to accept input from p1 controls
                    i = new InputModule(true, 1);
                    localPlayerInput = i;
                }
                else
                {
                    i = new InputModule(false, 0);
                }
                p.Setup(i, FInt.Zero(), FInt.Zero(), pnum + 1, onlineGame.playerNames[pnum]);
            }
            else
            {
                i = new InputModule(true, pnum + 1);
                p.Setup(i, FInt.Zero(), FInt.Zero(), pnum + 1, string.Format("Player {0}", pnum + 1));
                NetworkingManager.seed = new System.Random();
            }
            players.Add(p);
            inputModules.Add(i);
        }

        isPlaying = true;
        isOnline = onlineGame != null;
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

    public void GameMessage(int playerNum, string inputs)
    {
        inputModules[playerNum].Input(inputs);
    }
}
