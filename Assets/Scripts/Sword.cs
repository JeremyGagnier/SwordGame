using UnityEngine;
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
    public FInt rotation;
    public FInt weight;
    public FInt damage;

    public SwingState state = SwingState.NONE;
    public FInt swingDuration = FInt.Zero();
    public FInt maxDuration = FInt.Zero();
    public FInt swingCooldown = FInt.Zero();
    public FInt angle = FInt.Zero();

    void Start()
    {
        hilt.player = owner;
        freeNodes.Add(hilt.nodePoints[0]);
        weight = hilt.weight;
        damage = hilt.damage;
    }

    // TODO: Call this from somewhere elses fixed update
    // BREAKS: Multiplayer
    void FixedUpdate()
    {
        if (swingDuration > Game.TIMESTEP)
        {
            hilt.gameObject.SetActive(true);
            swingDuration -= Game.TIMESTEP;
            FInt pct = swingDuration / maxDuration;
            switch (state)
            {
                // TODO: Make the sword have FInt rotations and positions
                // BREAKS: Sword position/movement
                case SwingState.STAB:
                    position.x = 150 * (FInt.One() - pct) * FInt.Cos(angle + FInt.PI() / 2);
                    position.y = new FInt(61) + 150 * (FInt.One() - pct) * FInt.Sin(angle + FInt.PI() / 2);
                    rotation = angle;
                    break;
                case SwingState.CWISE:
                    position.x = 150 * FInt.Cos(angle + (new FInt(2) - pct) * FInt.PI() / 2);
                    position.y = new FInt(61) + 150 * FInt.Sin(angle + (new FInt(2) - pct) * FInt.PI() / 2);
                    rotation = angle + (-pct + 1) * FInt.PI() / 2;
                    break;
                case SwingState.CCWISE:
                    position.x = 150 * FInt.Cos(angle + pct * FInt.PI() / 2);
                    position.y = new FInt(61) + 150 * FInt.Sin(angle + pct * FInt.PI() / 2);
                    rotation = angle - (-pct + 1) * FInt.PI() / 2;
                    break;
            }
            transform.localPosition = new Vector3(position.x.ToFloat(), position.y.ToFloat());
            transform.localEulerAngles = new Vector3(0, 0, rotation.ToFloat() * 180 / Mathf.PI);
            hilt.position = position;
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
                angle = swingAngle - FInt.PI() / 2;
                break;
            case SwingState.CWISE:
                angle = swingAngle - FInt.PI();
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
        // First iteration counts the weight
        foreach (Node m in freeNodes)
        {
            if (m.parent.depthInSword.rawValue < 0) continue;
            w += new FInt(1.0f) + m.parent.depthInSword * m.parent.depthInSword;
        }
        if (w.rawValue == 0)
        {
            return;
        }
        FInt choose = FInt.RandomRange(NetworkingManager.seed, FInt.Zero(), w);
        // Second iteration finds the part at a random weight
        w = FInt.Zero();
        foreach (Node m in freeNodes)
        {
            if (m.parent.depthInSword.rawValue < 0) continue;
            w += new FInt(1.0f) + m.parent.depthInSword * m.parent.depthInSword;
            if (choose <= w)
            {
                n = m;
                break;
            }
        }

        part.transform.SetParent(n.parent.transform);
        int attachPoint = Random.Range(0, swordPart.nodePoints.Length);
        swordPart.Attach(n, attachPoint, this.gameObject, owner);
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
        FInt randangle = FInt.RandomRange(NetworkingManager.seed, FInt.Zero(), new FInt(2.0f * 3.14159f));
        FInt randdist = FInt.RandomRange(NetworkingManager.seed, new FInt(3.0f) * Player.PRADIUS, new FInt(6.0f) * Player.PRADIUS);
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
            part.Attack(world);
        }
        parts.Remove(hilt);
    }
}
