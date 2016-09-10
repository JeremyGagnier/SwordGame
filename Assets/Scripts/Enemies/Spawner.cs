using UnityEngine;

public class Spawner : MonoBehaviour
{
    public FInt radius;
    public FInt waitTime;
    public FInt cooldown;
    public FVector spawnPos;

    private FInt waiting = 0L;
    private FInt coolingDown = 0L;

    [SerializeField] private GameObject spawn;

    void Start()
    {
        Game.instance.AddSpawner(this);
    }

    public virtual void Advance()
    {
        if (coolingDown > waiting)
        {
            coolingDown -= Game.TIMESTEP;
        }
        else if (waiting > 0L)
        {
            waiting -= coolingDown;
            coolingDown = 0L;
        }
        else
        {
            coolingDown = 0L;
        }

        if (Game.instance.IsPlayerInRange(spawnPos, radius))
        {
            waiting += Game.TIMESTEP;
            if (waiting >= waitTime)
            {
                waiting = waitTime;
                if (coolingDown <= 0L)
                {
                    Spawn();
                    coolingDown += cooldown;
                }
            }
        }
        else if (waiting > 0L)
        {
            waiting -= Game.TIMESTEP;
        }
        else
        {
            waiting = 0L;
        }
    }

    public virtual void Spawn()
    {
        Game.instance.SummonEnemy(spawn, spawnPos);
    }
}
