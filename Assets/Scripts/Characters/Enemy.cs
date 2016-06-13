using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public FInt radius;

    [SerializeField] private FInt speed;
    [SerializeField] private FInt health;
    [SerializeField] private FInt damage;
    [SerializeField] private FInt weight;

    [SerializeField] private Sprite normalTex;
    [SerializeField] private Sprite hitTex;
    [SerializeField] private Sprite attackTex;
    [SerializeField] private Image character;
    
    private FInt rehitTimer = FInt.Zero();
    private FInt damagedTimer = FInt.Zero();
    
    void Awake()
    {
        team = 0;
    }

    public void Setup(FVector pos)
    {
        position = new FVector(pos);
        transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat());
    }

    public override void Advance()
    {
        // TODO: Implement enemy movement
        FVector dir = new FVector(FInt.Zero(), FInt.Zero());

        position.x += dir.x * speed * Game.TIMESTEP;
        position.y += dir.y * speed * Game.TIMESTEP;

        if (dir.x.rawValue < 0)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        Game.instance.Attack(team, position, radius, damage, () => { rehitTimer = new FInt(1.0f); });
        if (rehitTimer.rawValue > 0)
        {
            rehitTimer -= Game.TIMESTEP;
        }

        if (damagedTimer.rawValue > 0)
        {
            character.sprite = hitTex;
            damagedTimer -= Game.TIMESTEP;
        }
        else if (rehitTimer.rawValue > 0 && rehitTimer < new FInt(0.25f))
        {
            character.sprite = attackTex;
        }
        else
        {
            character.sprite = normalTex;
        }

        base.Advance();
    }

    public void Damage(FInt damage)
    {
        // Die if life reaches zero
        health -= damage;
        if (health.rawValue <= 0)
        {
            Game.instance.KillEnemy(this);
            return;
        }
        damagedTimer = new FInt(0.25f);
        invincibility += new FInt(0.4f);
    }
}
