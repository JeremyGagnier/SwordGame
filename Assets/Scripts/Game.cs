using UnityEngine;
using System;
using System.Collections;
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
                if (Collision.Distance(part.position.x,
                                   part.position.y,
                                   player.position.x,
                                   player.position.y) < Player.PRADIUS)
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
        FInt damage,
        Action callback)
    {
        // TODO: Clean this up so that this simply checks against all Characters
        for (int i = 0; i < players.Count; ++i)
        {
            Player p = players[i].GetComponent<Player>();
            if (p.team != sourceTeam &&
                p.invincibility.rawValue <= 0 &&
                Collision.Distance(source.x, source.y, p.position.x, p.position.y) <= Player.PRADIUS + radius)
            {
                p.Damage(damage);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }
        }

        for (int i = 0; i < enemies.Count; ++i)
        {
            int startlen = enemies.Count;
            Enemy e = enemies[i].GetComponent<Enemy>();
            if (e.team != sourceTeam &&
                e.invincibility.rawValue <= 0 &&
                Collision.Distance(source.x, source.y, e.position.x, e.position.y) <= e.radius + radius)
            {
                e.Damage(damage);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }
            // Check if the enemy died
            // TODO: Improve this flow because this is a hack
            if (enemies.Count != startlen)
            {
                i -= 1;
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
