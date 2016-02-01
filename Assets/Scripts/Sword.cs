using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    [System.NonSerialized]
    public SwordPart parent;
    public Vector2 pos;
    public Vector2 dir;
}

public class Sword : MonoBehaviour
{
    public enum SwingState
    {
        NONE,
        CWISE,
        CCWISE,
        STAB
    }

    public World world;
    public Player owner;
    public SwordPart hilt;
    public List<Node> freeNodes = new List<Node>();
    public List<SwordPart> parts = new List<SwordPart>();
    public bool[] damaged = { false, false, false, false };
    public List<GameObject> damagedEnemies = new List<GameObject>();

    public float weight;
    public float damage;

    public SwingState state = SwingState.NONE;
    public float swingDuration = 0.0f;
    public float maxDuration = 0.0f;
    public float swingCooldown = 0.0f;
    public float angle = 0.0f;

    void Start()
    {
        freeNodes.Add(hilt.nodePoints[0]);
        weight = hilt.weight;
        damage = hilt.damage;
    }

    void Update()
    {
        if (swingDuration > 0.0f)
        {
            hilt.gameObject.SetActive(true);
            swingDuration -= Time.deltaTime;
            float pct = swingDuration / maxDuration;
            switch (state)
            {
                case SwingState.STAB:
                    // Move in accordance with stabbing
                    transform.localPosition = new Vector3(150 * (1 - pct) * Mathf.Cos(angle + Mathf.PI / 2),
                                                          61 + 150 * (1 - pct) * Mathf.Sin(angle + Mathf.PI / 2));
                    transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);
                    break;
                case SwingState.CWISE:
                    transform.localPosition = new Vector3(150 * Mathf.Cos(angle + Mathf.PI / 2 + Mathf.PI / 2 * (1 - pct)),
                                                          61 + 150 * Mathf.Sin(angle + Mathf.PI / 2 + Mathf.PI / 2 * (1 - pct)));
                    transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI + 90 * (1 - pct));
                    break;
                case SwingState.CCWISE:
                    transform.localPosition = new Vector3(150 * Mathf.Cos(angle + Mathf.PI / 2 - Mathf.PI / 2 * (1 - pct)),
                                                          61 + 150 * Mathf.Sin(angle + Mathf.PI / 2 - Mathf.PI / 2 * (1 - pct)));
                    transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI - 90 * (1 - pct));
                    break;

            }
            CheckCollisions();
        }
        else
        {
            hilt.gameObject.SetActive(false);
            if (swingCooldown > 0.0f)
            {
                swingCooldown -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < 4; ++i) damaged[i] = false;
                damagedEnemies.Clear();
                state = SwingState.NONE;
            }
        }
    }

    public void Swing(SwingState swingType, float swingAngle)
    {
        if (state != SwingState.NONE)
        {
            return;
        }
        state = swingType;
        swingDuration = 0.1f + 0.025f * weight;
        maxDuration = swingDuration;
        swingCooldown = 0.1f + 0.01f * weight;
        switch (state)
        {
            case SwingState.STAB:
                angle = swingAngle - Mathf.PI / 2.0f;
                break;
            case SwingState.CWISE:
                angle = swingAngle - Mathf.PI;
                break;
            case SwingState.CCWISE:
                angle = swingAngle;
                break;
        }
    }

    public void AddPart(GameObject part)
    {
        SwordPart swordPart = part.GetComponent<SwordPart>();

        Node n = freeNodes[0];
        float w = 0;
        foreach (Node m in freeNodes)
        {
            if (m.parent.truey < 0.0f) continue;
            w += 1.0f + m.parent.truey * m.parent.truey;
        }
        if (w == 0)
        {
            return;
        }
        float choose = Random.Range(0.0f, w);
        w = 0;
        foreach (Node m in freeNodes)
        {
            if (m.parent.truey < 0.0f) continue;
            w += 1.0f + m.parent.truey * m.parent.truey;
            if (choose <= w)
            {
                n = m;
                break;
            }
        }

        part.transform.SetParent(n.parent.transform);
        int attachPoint = Random.Range(0, swordPart.nodePoints.Length);
        swordPart.Attach(n, attachPoint, this.gameObject);
        freeNodes.Remove(n);
        swordPart.consumedNode = n;
        for (int i = 0; i < swordPart.nodePoints.Length; ++i)
        {
            if (i == attachPoint) continue;
            freeNodes.Add(swordPart.nodePoints[i]);
        }

        parts.Add(swordPart);
        weight += swordPart.weight;
        damage += swordPart.damage;
    }

    public void RemovePart()
    {
        if (parts.Count == 0)
        {
            return;
        }

        for (int i = 0; i < parts[parts.Count - 1].nodePoints.Length; ++i)
        {
            if (freeNodes.Contains(parts[parts.Count - 1].nodePoints[i]))
            {
                freeNodes.Remove(parts[parts.Count - 1].nodePoints[i]);
            }
        }
        freeNodes.Add(parts[parts.Count - 1].consumedNode);
        weight -= parts[parts.Count - 1].weight;
        damage -= parts[parts.Count - 1].damage;

        GameObject part = parts[parts.Count - 1].gameObject;
        parts.RemoveAt(parts.Count - 1);

        part.transform.SetParent(world.transform);
        float randangle = Random.Range(0.0f, 2.0f * Mathf.PI);
        float randdist = Random.Range(3.0f * Player.PRADIUS, 6.0f * Player.PRADIUS);
        Vector3 pos = new Vector3(owner.transform.position.x + randdist * Mathf.Cos(randangle),
                                  owner.transform.position.y + randdist * Mathf.Sin(randangle));
        if (pos.x < -2880)
        {
            pos.x = -2880;
        }
        if (pos.x > 2880)
        {
            pos.x = 2880;
        }
        if (pos.y < -2160)
        {
            pos.y = -2160;
        }
        if (pos.y > 2160)
        {
            pos.y = 2160;
        }
        part.transform.position = pos;
        world.swordParts.Add(part);
    }

    private void CheckCollisions()
    {
        parts.Add(hilt);
        if (owner.pnum != 1 && !damaged[0])
        {
            foreach (SwordPart part in parts)
            {
                if (Collision.dist(part.transform.position.x,
                                   part.transform.position.y,
                                   world.player1.transform.position.x,
                                   world.player1.transform.position.y) <= Player.PRADIUS)
                {
                    world.player1.GetComponent<Player>().Damage(owner.gameObject, Mathf.Sqrt(damage), weight);
                    damaged[0] = true;
                }
            }
        }
        if (owner.pnum != 2 && !damaged[1])
        {
            foreach (SwordPart part in parts)
            {
                if (Collision.dist(part.transform.position.x,
                                   part.transform.position.y,
                                   world.player2.transform.position.x,
                                   world.player2.transform.position.y) <= Player.PRADIUS)
                {
                    world.player2.GetComponent<Player>().Damage(owner.gameObject, Mathf.Sqrt(damage), weight);
                    damaged[1] = true;
                }
            }
        }
        if (owner.pnum != 3 && !damaged[2] && world.numPlayers >= 3)
        {
            foreach (SwordPart part in parts)
            {
                if (Collision.dist(part.transform.position.x,
                                   part.transform.position.y,
                                   world.player3.transform.position.x,
                                   world.player3.transform.position.y) <= Player.PRADIUS)
                {
                    world.player3.GetComponent<Player>().Damage(owner.gameObject, Mathf.Sqrt(damage), weight);
                    damaged[2] = true;
                }
            }
        }
        if (owner.pnum != 4 && !damaged[3] && world.numPlayers >= 4)
        {
            foreach (SwordPart part in parts)
            {
                if (Collision.dist(part.transform.position.x,
                                   part.transform.position.y,
                                   world.player4.transform.position.x,
                                   world.player4.transform.position.y) <= Player.PRADIUS)
                {
                    world.player4.GetComponent<Player>().Damage(owner.gameObject, Mathf.Sqrt(damage), weight);
                    damaged[3] = true;
                }
            }
        }

        for (int i = 0; i < world.enemies.Count; ++i)
        {
            foreach (SwordPart part in parts)
            {
                if (damagedEnemies.Contains(world.enemies[i])) continue;

                if (Collision.dist(part.transform.position.x,
                                   part.transform.position.y,
                                   world.enemies[i].transform.position.x,
                                   world.enemies[i].transform.position.y) <= world.enemies[i].GetComponent<Enemy>().radius)
                {
                    if (world.enemies[i].GetComponent<Enemy>().Damage(owner.gameObject, Mathf.Sqrt(damage), weight))
                    {
                        i -= 1;
                        break;
                    }
                    else
                    {
                        damagedEnemies.Add(world.enemies[i]);
                    }
                }
            }
        }

        parts.Remove(hilt);
    }
}
