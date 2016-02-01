using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public World world;
    public int rarity;
    public float speed;
    public float health;
    public float damage;
    public float weight;
    public float radius;
    public float rehitTimer = 0.0f;
    public float damagedTimer = 0.0f;

    public Sprite normalTex;
    public Sprite hitTex;
    public Sprite attackTex;
    public Image character;

    void Update()
    {
        // Chase after closest available player
        float d1 = Collision.dist(this.transform.position.x,
                                  this.transform.position.y,
                                  world.player1.transform.position.x,
                                  world.player1.transform.position.y);
        float d2 = Collision.dist(this.transform.position.x,
                                  this.transform.position.y,
                                  world.player2.transform.position.x,
                                  world.player2.transform.position.y);
        float d3 = Collision.dist(this.transform.position.x,
                                  this.transform.position.y,
                                  world.player3.transform.position.x,
                                  world.player3.transform.position.y);
        float d4 = Collision.dist(this.transform.position.x,
                                  this.transform.position.y,
                                  world.player4.transform.position.x,
                                  world.player4.transform.position.y);
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
                dir = new Vector2(world.player1.transform.position.x - this.transform.position.x,
                                  world.player1.transform.position.y - this.transform.position.y);
                dir.Normalize();
                break;

            case 2:
                dir = new Vector2(world.player2.transform.position.x - this.transform.position.x,
                                  world.player2.transform.position.y - this.transform.position.y);
                dir.Normalize();
                break;

            case 3:
                dir = new Vector2(world.player3.transform.position.x - this.transform.position.x,
                                  world.player3.transform.position.y - this.transform.position.y);
                dir.Normalize();
                break;

            case 4:
                dir = new Vector2(world.player4.transform.position.x - this.transform.position.x,
                                  world.player4.transform.position.y - this.transform.position.y);
                dir.Normalize();
                break;
        }
        this.transform.position = new Vector3(this.transform.position.x + dir.x * this.speed * Time.deltaTime,
                                              this.transform.position.y + dir.y * this.speed * Time.deltaTime);
        if (dir.x < 0.0f)
        {
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            this.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        if (rehitTimer <= 0.0f)
        {
            // Damage player on contact
            if (Collision.dist(this.transform.position.x,
                               this.transform.position.y,
                               world.player1.transform.position.x,
                               world.player1.transform.position.y) < this.radius + Player.PRADIUS)
            {
                world.player1.GetComponent<Player>().Damage(this.gameObject, damage, weight);
                rehitTimer += 1.0f;
            }
            if (Collision.dist(this.transform.position.x,
                               this.transform.position.y,
                               world.player2.transform.position.x,
                               world.player2.transform.position.y) < this.radius + Player.PRADIUS)
            {
                world.player2.GetComponent<Player>().Damage(this.gameObject, damage, weight);
                rehitTimer += 1.0f;
            }
            if (world.numPlayers >= 3)
            {
                if (Collision.dist(this.transform.position.x,
                                   this.transform.position.y,
                                   world.player3.transform.position.x,
                                   world.player3.transform.position.y) < this.radius + Player.PRADIUS)
                {
                    world.player3.GetComponent<Player>().Damage(this.gameObject, damage, weight);
                    rehitTimer += 1.0f;
                }
            }
            if (world.numPlayers >= 4)
            {
                if (Collision.dist(this.transform.position.x,
                                   this.transform.position.y,
                                   world.player4.transform.position.x,
                                   world.player4.transform.position.y) < this.radius + Player.PRADIUS)
                {
                    world.player4.GetComponent<Player>().Damage(this.gameObject, damage, weight);
                    rehitTimer += 1.0f;
                }
            }
        }
        else
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
    }

    public bool Damage(GameObject source, float damage, float weight)
    {
        // Apply knockback
        float dx = this.transform.position.x - source.transform.position.x;
        float dy = this.transform.position.y - source.transform.position.y;
        float a = Mathf.Atan2(dy, dx);
        transform.position = new Vector3(this.transform.position.x + 7.0f * weight * Mathf.Cos(a),
                                         this.transform.position.y + 7.0f * weight * Mathf.Sin(a));

        // Die if life reaches zero
        health -= damage;
        if (health <= 0.0f)
        {
            world.KillEnemy(this.gameObject);
            return true;
        }
        damagedTimer = 0.25f;
        return false;
    }
}
