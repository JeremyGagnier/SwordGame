using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : Character
{
    public static FInt PRADIUS = new FInt(100.0f); // How big players are

    public World world;

    public GameObject characterImg;
    public GameObject characterMask;

    private List<FVector> lastFacing;
    public FInt facing {
        get
        {
            FInt dx = FInt.Zero();
            FInt dy = FInt.Zero();
            foreach (FVector vec in lastFacing)
            {
                dx += vec.x;
                dy += vec.y;
            }
            dx /= lastFacing.Count;
            dy /= lastFacing.Count;
            return FInt.Atan(dx, dy);
        }
    }

    public int pnum;
    public Sword sword;
    private FInt dmgLeftovers = FInt.Zero();

    public FInt speed
    {
        get { return 10 * FInt.Max(new FInt(100.0f) - sword.weight, new FInt(10.0f)); }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(position.x.ToFloat(), position.y.ToFloat()), PRADIUS.ToFloat());
    }

    void Start()
    {
        lastFacing = new List<FVector>();
        for (int i = 0; i < 3; ++i)
        {
            lastFacing.Add(new FVector(FInt.Zero(), FInt.Zero()));
        }
    }

    public void Setup(World world, FInt startx, FInt starty, int player, int team)
    {
        this.world = world;
        this.team = team;
        sword.world = world;
        sword.owner = this;
        position.x = startx;
        position.y = starty;
        pnum = player;
        transform.position = new Vector3(posx, posy);
    }

    new void Update()
    {
        FInt dx = FInt.Zero();
        FInt dy = FInt.Zero();
        if (pnum == 1)
        {
            dx += InputManager.p1axisx;
            dy -= InputManager.p1axisy;
            if (InputManager.p1button1)
            {
                // Swing counter-clockwise
                sword.Swing(Sword.SwingState.CCWISE, facing);
            }
            if (InputManager.p1button2)
            {
                // Stab
                sword.Swing(Sword.SwingState.STAB, facing);
            }
            if (InputManager.p1button3)
            {
                // Swing clockwise
                sword.Swing(Sword.SwingState.CWISE, facing);
            }
        }
        if (pnum == 2)
        {
            dx += InputManager.p2axisx;
            dy -= InputManager.p2axisy;
            if (InputManager.p2button1)
            {
                // Swing counter-clockwise
                sword.Swing(Sword.SwingState.CCWISE, facing);
            }
            if (InputManager.p2button2)
            {
                // Stab
                sword.Swing(Sword.SwingState.STAB, facing);
            }
            if (InputManager.p2button3)
            {
                // Swing clockwise
                sword.Swing(Sword.SwingState.CWISE, facing);
            }
        }
        if (pnum == 3)
        {
            dx += InputManager.p3axisx;
            dy -= InputManager.p3axisy;
            if (InputManager.p3button1)
            {
                // Swing counter-clockwise
                sword.Swing(Sword.SwingState.CCWISE, facing);
            }
            if (InputManager.p3button2)
            {
                // Stab
                sword.Swing(Sword.SwingState.STAB, facing);
            }
            if (InputManager.p3button3)
            {
                // Swing clockwise
                sword.Swing(Sword.SwingState.CWISE, facing);
            }
        }
        if (pnum == 3)
        {
            dx += InputManager.p4axisx;
            dy -= InputManager.p4axisy;
            if (InputManager.p4button1)
            {
                // Swing counter-clockwise
                sword.Swing(Sword.SwingState.CCWISE, facing);
            }
            if (InputManager.p4button2)
            {
                // Stab
                sword.Swing(Sword.SwingState.STAB, facing);
            }
            if (InputManager.p4button3)
            {
                // Swing clockwise
                sword.Swing(Sword.SwingState.CWISE, facing);
            }
        }

        if (dx.rawValue != 0 || dy.rawValue != 0)
        {
            position.x += dx * speed * Game.TIMESTEP;
            position.y += dy * speed * Game.TIMESTEP;
            lastFacing.RemoveAt(0);
            lastFacing.Add(new FVector(dx, dy));
        }

        // Check for collisions with items to add them to sword
        for (int i = 0; i < world.swordParts.Count; ++i)
        {
            GameObject part = world.swordParts[i];
            SwordPart p = part.GetComponent<SwordPart>();
            if (Collision.dist(p.position.x,
                               p.position.y,
                               position.x,
                               position.y) < PRADIUS)
            {
                sword.AddPart(part);
                world.swordParts.RemoveAt(i);
                i -= 1;
            }
        }

        FInt fdx = FInt.Zero();
        FInt fdy = FInt.Zero();
        foreach (FVector vec in lastFacing)
        {
            fdx += vec.x;
            fdy += vec.y;
        }
        if (fdx.rawValue < 0)
        {
            characterImg.transform.localPosition = new Vector3(30, 118, 0);
            characterImg.transform.localScale = new Vector3(1, 1, 1);
            characterMask.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            characterImg.transform.localPosition = new Vector3(-30, 118, 0);
            characterImg.transform.localScale = new Vector3(-1, 1, 1);
            characterMask.transform.localScale = new Vector3(-1, 1, 1);
        }

        base.Update();
    }

    public void Damage(FVector source, FInt damage, FInt weight)
    {
        // Apply knockback
        FInt dx = position.x - source.x;
        FInt dy = position.y - source.y;
        FInt a = FInt.Atan(dx, dy);
        position.x += new FInt(7.0f) * weight * FInt.Cos(a);
        position.y += new FInt(7.0f) * weight * FInt.Sin(a);

        // Remove sword parts
        dmgLeftovers += damage;
        while (dmgLeftovers >= FInt.One())
        {
            dmgLeftovers -= FInt.One();
            sword.RemovePart();
        }
        invincibility += new FInt(0.4f);
    }
}
