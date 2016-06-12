using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public FVector position = new FVector(FInt.Zero(), FInt.Zero());
    public int team;
    public FInt invincibility = FInt.Zero();

    public virtual void Advance()
    {
        if (invincibility.rawValue > 0)
        {
            invincibility -= Game.TIMESTEP;
        }
        transform.position = new Vector3(position.x.ToFloat(), position.y.ToFloat());
    }
}
