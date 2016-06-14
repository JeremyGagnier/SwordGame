using UnityEngine;
using System;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public static FInt TIMESTEP = new FInt(0.01666f);

    public static Game instance = null;
    public static int numPlayers = 2;
    public static bool isPlaying = false;

    [SerializeField] private World world;

    private List<InputManager> inputModules = new List<InputManager>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Player> players = new List<Player>();
    private List<SwordPart> swordParts = new List<SwordPart>();
    private List<Spawner> spawners = new List<Spawner>();

    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        if (!isPlaying) return;

        world.Advance();
        PickUpSwordParts();
        foreach (Spawner spawner in spawners)
        {
            spawner.Advance();
        }
        foreach (InputManager inputModule in inputModules)
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
    }

    public void AddSpawner(Spawner spawner)
    {
        spawners.Add(spawner);
    }

    public void StartGame(int numPlayers)
    {
        Game.numPlayers = numPlayers;

        NetworkingManager.seed = new System.Random();

        world.SetPlayersAndCameras();

        List<GameObject> playerObjects = world.GetPlayers();
        for (int pnum = 0; pnum < numPlayers; ++pnum)
        {
            Player p = playerObjects[pnum].GetComponent<Player>();
            InputManager i = new InputManager(true, pnum + 1);
            players.Add(p);
            inputModules.Add(i);
            p.Setup(i, FInt.Zero(), FInt.Zero(), pnum + 1);
        }

        isPlaying = true;
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
}
