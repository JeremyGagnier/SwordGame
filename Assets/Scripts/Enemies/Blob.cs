using UnityEngine;
using System.Collections;

public class Blob : Enemy
{
    [SerializeField] private FInt speed;
    [SerializeField] private FInt jumpCooldown;
    [SerializeField] private FInt jumpDuration;

    private enum State
    {
        WAITING,
        JUMPING
    }
    private State state;
    private FVector jumpDirection;
    private FInt cooldown = FInt.Zero();
    private FInt timeJumping = FInt.Zero();

    void Start()
    {
        state = State.WAITING;
    }

    public override void Advance()
    {
        if (state == State.WAITING)
        {
            cooldown += Game.TIMESTEP;
            if (cooldown >= jumpCooldown)
            {
                cooldown = FInt.Zero();
                jumpDirection = Game.instance.GetNearestPlayerPosition(position);
                jumpDirection.x = (jumpDirection.x - position.x);
                jumpDirection.y = (jumpDirection.y - position.y);
                jumpDirection = jumpDirection.Normalize();
                if (jumpDirection.x.rawValue < 0)
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
                state = State.JUMPING;
            }
        }
        else if (state == State.JUMPING)
        {
            timeJumping += Game.TIMESTEP;
            if (timeJumping >= jumpDuration)
            {
                timeJumping = FInt.Zero();
                state = State.WAITING;
            }
            else
            {
                position.x += jumpDirection.x * speed * Game.TIMESTEP;
                position.y += jumpDirection.y * speed * Game.TIMESTEP;
            }
        }
        base.Advance();
    }
}
