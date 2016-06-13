using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [HideInInspector] public FVector position = new FVector(FInt.Zero(), FInt.Zero());
    [HideInInspector] public int team;
    [HideInInspector] public FInt invincibility = FInt.Zero();

    public virtual void Advance()
    {
        if (invincibility.rawValue > 0)
        {
            invincibility -= Game.TIMESTEP;
        }
        transform.position = new Vector3(position.x.ToFloat(), position.y.ToFloat());
    }
}
