using UnityEngine;

public class Spawner : MonoBehaviour
{
    public FInt radius;
    public FInt waitTime;
    public FInt cooldown;
    public FVector spawnPos;

    private FInt waiting = FInt.Zero();
    private FInt coolingDown = FInt.Zero();

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
        else if (waiting.rawValue > 0)
        {
            waiting -= coolingDown;
            coolingDown = FInt.Zero();
        }
        else
        {
            coolingDown = FInt.Zero();
        }

        if (Game.instance.IsPlayerInRange(spawnPos, radius))
        {
            waiting += Game.TIMESTEP;
            if (waiting >= waitTime)
            {
                waiting = waitTime;
                if (coolingDown.rawValue <= 0)
                {
                    Spawn();
                    coolingDown += cooldown;
                }
            }
        }
        else if (waiting.rawValue > 0)
        {
            waiting -= Game.TIMESTEP;
        }
        else
        {
            waiting = FInt.Zero();
        }
    }

    public virtual void Spawn()
    {
        Game.instance.SummonEnemy(spawn, spawnPos);
    }
}
