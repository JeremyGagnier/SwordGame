using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public static FInt TIMESTEP = new FInt(0.01666f);

    public static int numPlayers = 2;
    public static bool isPlaying = false;

    public World world;

    private List<InputManager> inputModules = new List<InputManager>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Player> players = new List<Player>();

    public void StartGame(int numPlayers)
    {
        Game.numPlayers = numPlayers;

        NetworkingManager.seed = new System.Random();

        world.StartGame(numPlayers);

        List<GameObject> playerObjects = world.GetPlayers();
        for (int pnum = 0; pnum < numPlayers; ++pnum)
        {
            Player p = playerObjects[pnum].GetComponent<Player>();
            InputManager i = new InputManager(true, pnum + 1);
            players.Add(p);
            inputModules.Add(i);
            p.Setup(world, i, FInt.Zero(), FInt.Zero(), pnum, pnum);
        }

        isPlaying = true;
    }

    void FixedUpdate()
    {
        if (!isPlaying) return;

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
        world.Advance();
    }
}
