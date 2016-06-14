using UnityEngine;
using System.Collections;

public class Blob : Enemy
{
    public override void Advance()
    {
        FVector dir = new FVector(FInt.Zero(), FInt.Zero());

        //position.x += dir.x * speed * Game.TIMESTEP;
        //position.y += dir.y * speed * Game.TIMESTEP;

        if (dir.x.rawValue < 0)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        base.Advance();
    }
}
