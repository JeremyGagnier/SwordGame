using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public FVector position = new FVector(FInt.Zero(), FInt.Zero());
    public float posx = 0.0f;
    public float posy = 0.0f;
    public int team;
    public FInt invincibility = FInt.Zero();

    public void Update()
    {
        if (invincibility.rawValue > 0)
        {
            invincibility -= Game.TIMESTEP;
        }
        transform.position = new Vector3(position.x.ToFloat(), position.y.ToFloat());
    }
}
