﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    [System.NonSerialized]
    public SwordPart parent;
    public FVector pos;
    public FVector dir;
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

    public FVector position;
    public FInt weight;
    public FInt damage;

    public SwingState state = SwingState.NONE;
    public FInt swingDuration = FInt.Zero();
    public FInt maxDuration = FInt.Zero();
    public FInt swingCooldown = FInt.Zero();
    public FInt angle = FInt.Zero();

    void Start()
    {
        freeNodes.Add(hilt.nodePoints[0]);
        weight = hilt.weight;
        damage = hilt.damage;
    }

    void Update()
    {
        if (swingDuration.rawValue > 0)
        {
            hilt.gameObject.SetActive(true);
            swingDuration -= Game.TIMESTEP;
            float pct = (swingDuration / maxDuration).ToFloat();
            switch (state)
            {
                case SwingState.STAB:
                    // Move in accordance with stabbing
                    transform.localPosition = new Vector3(150 * (1 - pct) * Mathf.Cos(angle.ToFloat() + Mathf.PI / 2),
                                                          61 + 150 * (1 - pct) * Mathf.Sin(angle.ToFloat() + Mathf.PI / 2));
                    transform.localEulerAngles = new Vector3(0, 0, angle.ToFloat() * 180 / Mathf.PI);
                    break;
                case SwingState.CWISE:
                    transform.localPosition = new Vector3(150 * Mathf.Cos(angle.ToFloat() + Mathf.PI / 2 + Mathf.PI / 2 * (1 - pct)),
                                                          61 + 150 * Mathf.Sin(angle.ToFloat() + Mathf.PI / 2 + Mathf.PI / 2 * (1 - pct)));
                    transform.localEulerAngles = new Vector3(0, 0, angle.ToFloat() * 180 / Mathf.PI + 90 * (1 - pct));
                    break;
                case SwingState.CCWISE:
                    transform.localPosition = new Vector3(150 * Mathf.Cos(angle.ToFloat() + Mathf.PI / 2 - Mathf.PI / 2 * (1 - pct)),
                                                          61 + 150 * Mathf.Sin(angle.ToFloat() + Mathf.PI / 2 - Mathf.PI / 2 * (1 - pct)));
                    transform.localEulerAngles = new Vector3(0, 0, angle.ToFloat() * 180 / Mathf.PI - 90 * (1 - pct));
                    break;

            }
            CheckCollisions();
        }
        else
        {
            hilt.gameObject.SetActive(false);
            if (swingCooldown.rawValue > 0)
            {
                swingCooldown -= Game.TIMESTEP;
            }
            else
            {
                state = SwingState.NONE;
            }
        }
    }

    public void Swing(SwingState swingType, FInt swingAngle)
    {
        if (state != SwingState.NONE)
        {
            return;
        }
        state = swingType;
        swingDuration = new FInt(0.1f) + new FInt(0.025f) * weight;
        maxDuration = swingDuration;
        swingCooldown = new FInt(0.1f) + new FInt(0.01f) * weight;
        switch (state)
        {
            case SwingState.STAB:
                angle = swingAngle - new FInt(3.1415f / 2.0f);
                break;
            case SwingState.CWISE:
                angle = swingAngle - new FInt(3.1415f);
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
        FInt w = FInt.Zero();
        foreach (Node m in freeNodes)
        {
            if (m.parent.truey.rawValue < 0) continue;
            w += new FInt(1.0f) + m.parent.truey * m.parent.truey;
        }
        if (w.rawValue == 0)
        {
            return;
        }
        FInt choose = FInt.RandomRange(null, FInt.Zero(), w);
        w = FInt.Zero();
        foreach (Node m in freeNodes)
        {
            if (m.parent.truey.rawValue < 0) continue;
            w += new FInt(1.0f) + m.parent.truey * m.parent.truey;
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
        FInt randangle = FInt.RandomRange(null, FInt.Zero(), new FInt(2.0f * 3.14159f));
        FInt randdist = FInt.RandomRange(null, new FInt(3.0f) * Player.PRADIUS, new FInt(6.0f) * Player.PRADIUS);
        FVector position = new FVector(owner.position.x + randdist * FInt.Cos(randangle),
                                       owner.position.y + randdist * FInt.Sin(randangle));

        // Don't throw parts outside the game map
        if (position.x.rawValue < -2880)
        {
            position.x.rawValue = -2880;
        }
        if (position.x.rawValue > 2880)
        {
            position.x.rawValue = 2880;
        }
        if (position.y.rawValue < -2160)
        {
            position.y.rawValue = -2160;
        }
        if (position.y.rawValue > 2160)
        {
            position.y.rawValue = 2160;
        }

        part.transform.position = new Vector3(position.x.ToFloat(), position.y.ToFloat());
        world.swordParts.Add(part);
    }

    private void CheckCollisions()
    {
        parts.Add(hilt);
        foreach (SwordPart part in parts)
        {
            world.Attack(owner.team, SwordPart.Position(this, part), FInt.Zero(), owner.position, FInt.Sqrt(damage), weight, null);
        }
        parts.Remove(hilt);
    }
}
