using UnityEngine;
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

    private Player owner;
    private List<Node> freeNodes = new List<Node>();
    private List<SwordPart> parts = new List<SwordPart>();

    private FVector position;
    private FInt _rotation;
    private FInt rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            hilt.rotation = value;
            _rotation = value;
        }
    }

    private SwingState state = SwingState.NONE;
    private FInt swingDuration = FInt.Zero();
    private FInt maxDuration = FInt.Zero();
    private FInt swingCooldown = FInt.Zero();
    private FInt angle = FInt.Zero();

    [SerializeField] private SwordPart hilt;
    
    [HideInInspector] public int weight;
    [HideInInspector] public int damage;

    public void Setup(Player owner)
    {
        this.owner = owner;
        hilt.player = owner;
        freeNodes.Add(hilt.nodePoints[0]);
        weight = hilt.weight;
        damage = hilt.damage;
    }
    
    public void Advance()
    {
        if (swingDuration > Game.TIMESTEP)
        {
            hilt.gameObject.SetActive(true);
            swingDuration -= Game.TIMESTEP;
            FInt pct = swingDuration / maxDuration;
            switch (state)
            {
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

    public void AddPart(SwordPart part)
    {
        Node n = freeNodes[0];
        FInt w = FInt.Zero();
        // First iteration counts the weight
        foreach (Node m in freeNodes)
        {
            if (m.parent.depthInSword.rawValue < 0)
            {
                continue;
            }
            w += new FInt(1.0f) + m.parent.depthInSword * m.parent.depthInSword;
        }
        if (w.rawValue == 0)
        {
            return;
        }
        FInt choose = FInt.RandomRange(OnlineNetwork.seed, FInt.Zero(), w);
        // Second iteration finds the part at a random weight
        w = FInt.Zero();
        foreach (Node m in freeNodes)
        {
            if (m.parent.depthInSword.rawValue < 0)
            {
                continue;
            }
            w += new FInt(1.0f) + m.parent.depthInSword * m.parent.depthInSword;
            if (choose <= w)
            {
                n = m;
                break;
            }
        }

        part.transform.SetParent(n.parent.transform);
        int attachPoint = Random.Range(0, part.nodePoints.Length);
        part.Attach(n, attachPoint, this.gameObject, owner);
        freeNodes.Remove(n);
        part.consumedNode = n;
        for (int i = 0; i < part.nodePoints.Length; ++i)
        {
            if (i == attachPoint)
            {
                continue;
            }
            freeNodes.Add(part.nodePoints[i]);
        }

        parts.Add(part);
        weight += part.weight;
        damage += part.damage;
    }

    public void RemovePart()
    {
        if (parts.Count == 0)
        {
            return;
        }
        
        SwordPart part = parts[parts.Count - 1];
        for (int i = 0; i < part.nodePoints.Length; ++i)
        {
            if (freeNodes.Contains(part.nodePoints[i]))
            {
                freeNodes.Remove(part.nodePoints[i]);
            }
        }
        freeNodes.Add(part.consumedNode);
        weight -= part.weight;
        damage -= part.damage;
        parts.RemoveAt(parts.Count - 1);
    }

    private void CheckCollisions()
    {
        parts.Add(hilt);
        foreach (SwordPart part in parts)
        {
            part.Attack();
        }
        parts.Remove(hilt);
    }
}
