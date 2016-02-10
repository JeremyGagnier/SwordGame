using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : Character
{
    public World world;
    public int rarity;
    public float speed;
    public float health;
    public float damage;
    public float weight;
    public float radius;

    private float rehitTimer = 0.0f;
    private float damagedTimer = 0.0f;

    public Sprite normalTex;
    public Sprite hitTex;
    public Sprite attackTex;
    public Image character;

    void Awake()
    {
        team = 0;
    }

    new void Update()
    {
        // Chase after closest available player
        float d1 = Collision.dist(posx,
                                  posy,
                                  world.p1Object.transform.position.x,
                                  world.p1Object.transform.position.y);
        float d2 = Collision.dist(posx,
                                  posy,
                                  world.p2Object.transform.position.x,
                                  world.p2Object.transform.position.y);
        float d3 = Collision.dist(posx,
                                  posy,
                                  world.p3Object.transform.position.x,
                                  world.p3Object.transform.position.y);
        float d4 = Collision.dist(posx,
                                  posy,
                                  world.p4Object.transform.position.x,
                                  world.p4Object.transform.position.y);
        float closest = d1;
        int closest_pnum = 1;
        if (d2 < closest)
        {
            closest = d2;
            closest_pnum = 2;
        }
        if (d3 < closest && world.numPlayers >= 3)
        {
            closest = d3;
            closest_pnum = 3;
        }
        if (d4 < closest && world.numPlayers >= 4)
        {
            closest = d4;
            closest_pnum = 4;
        }
        Vector2 dir = new Vector2();
        switch (closest_pnum)
        {
            case 1:
                dir = new Vector2(world.p1Object.transform.position.x - posx,
                                  world.p1Object.transform.position.y - posy);
                dir.Normalize();
                break;

            case 2:
                dir = new Vector2(world.p2Object.transform.position.x - posx,
                                  world.p2Object.transform.position.y - posy);
                dir.Normalize();
                break;

            case 3:
                dir = new Vector2(world.p3Object.transform.position.x - posx,
                                  world.p3Object.transform.position.y - posy);
                dir.Normalize();
                break;

            case 4:
                dir = new Vector2(world.p4Object.transform.position.x - posx,
                                  world.p4Object.transform.position.y - posy);
                dir.Normalize();
                break;
        }
        posx += dir.x * speed * Time.deltaTime;
        posy += dir.y * speed * Time.deltaTime;

        if (dir.x < 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        world.Attack(team, transform, radius, transform, damage, weight, () => { rehitTimer = 1.0f; });
        if (rehitTimer > 0.0f)
        {
            rehitTimer -= Time.deltaTime;
        }

        if (damagedTimer > 0.0f)
        {
            character.sprite = hitTex;
            damagedTimer -= Time.deltaTime;
        }
        else if (rehitTimer > 0.0f && rehitTimer < 0.25f)
        {
            character.sprite = attackTex;
        }
        else
        {
            character.sprite = normalTex;
        }

        base.Update();
    }

    public void Damage(Transform source, float damage, float weight)
    {
        // Apply knockback
        float dx = posx - source.position.x;
        float dy = posy - source.position.y;
        float a = Mathf.Atan2(dy, dx);
        transform.position = new Vector3(posx + 7.0f * weight * Mathf.Cos(a),
                                         posy + 7.0f * weight * Mathf.Sin(a));

        // Die if life reaches zero
        health -= damage;
        if (health <= 0.0f)
        {
            world.KillEnemy(gameObject);
            return;
        }
        damagedTimer = 0.25f;
        invincibility += 0.4f;
    }
}
