using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] private int weight;

    [SerializeField] private Sprite normalTex;
    [SerializeField] private Sprite hitTex;
    [SerializeField] private Sprite attackTex;
    [SerializeField] private Image character;
    
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
        Game.instance.Attack(team, position, radius, damage);
        base.Advance();
    }

    public override void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Game.instance.KillEnemy(this);
            return;
        }
        invincibility += new FInt(0.4f);
    }
}
