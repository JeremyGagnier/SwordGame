using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    public static FInt TIMESTEP = new FInt(0.01666f);

    void Start()
    {
        NetworkingManager.seed = new System.Random();
    }

    void FixedUpdate()
    {
        // TODO: Call everyone elses update functions here to fix multiplayer
    }
}
