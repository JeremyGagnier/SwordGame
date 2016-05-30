﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : Character
{
    public World world;
    public int rarity;
    public FInt speed;
    public FInt health;
    public FInt damage;
    public FInt weight;
    public FInt radius;

    private FInt rehitTimer = FInt.Zero();
    private FInt damagedTimer = FInt.Zero();

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
        FInt d1 = Collision.dist(position.x,
                                 position.y,
                                 world.player1.position.x,
                                 world.player1.position.y);
        FInt d2 = Collision.dist(position.x,
                                 position.y,
                                 world.player2.position.x,
                                 world.player2.position.y);
        FInt d3 = Collision.dist(position.x,
                                 position.y,
                                 world.player3.position.x,
                                 world.player3.position.y);
        FInt d4 = Collision.dist(position.x,
                                 position.y,
                                 world.player4.position.x,
                                 world.player4.position.y);
        FInt closest = d1;
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
        FVector dir = new FVector();
        switch (closest_pnum)
        {
            case 1:
                dir = new FVector(world.player1.position.x - position.x,
                                  world.player1.position.y - position.y);
                dir.Normalize();
                break;

            case 2:
                dir = new FVector(world.player2.position.x - position.x,
                                  world.player2.position.y - position.y);
                dir.Normalize();
                break;

            case 3:
                dir = new FVector(world.player3.position.x - position.x,
                                  world.player3.position.y - position.y);
                dir.Normalize();
                break;

            case 4:
                dir = new FVector(world.player4.position.x - position.x,
                                  world.player4.position.y - position.y);
                dir.Normalize();
                break;
        }
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

        world.Attack(team, position, radius, position, damage, weight, () => { rehitTimer = new FInt(1.0f); });
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

        base.Update();
    }

    public void Damage(FVector source, FInt damage, FInt weight)
    {
        // Apply knockback
        FInt dx = position.x - source.x;
        FInt dy = position.y - source.y;
        FInt a = FInt.Atan(dx, dy);

        // TODO: Smarter knockback calculation
        position = new FVector(position.x + new FInt(7) * weight * FInt.Cos(a),
                               position.y + new FInt(7) * weight * FInt.Sin(a));

        // Die if life reaches zero
        health -= damage;
        if (health.rawValue <= 0)
        {
            world.KillEnemy(gameObject);
            return;
        }
        damagedTimer = new FInt(0.25f);
        invincibility += new FInt(0.4f);
    }
}