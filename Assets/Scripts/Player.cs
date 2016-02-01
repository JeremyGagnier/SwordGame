using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public const float PRADIUS = 100.0f; // How big players are

    public World world;

    public GameObject characterImg;
    public GameObject characterMask;

    private List<Vector2> lastFacing;
    public float facing {
        get
        {
            float dx = 0.0f;
            float dy = 0.0f;
            foreach (Vector2 vec in lastFacing)
            {
                dx += vec.x;
                dy += vec.y;
            }
            dx /= lastFacing.Count;
            dy /= lastFacing.Count;
            return Mathf.Atan2(dy, dx);
        }
    }

    public float posx = 0.0f;
    public float posy = 0.0f;
    public int pnum;
    public Sword sword;
    public float dmgLeftovers = 0.0f;

    public float speed
    {
        get { return Mathf.Max(100.0f - sword.weight, 10.0f) * 5.0f; }
    }

    void Start()
    {
        lastFacing = new List<Vector2>();
        for (int i = 0; i < 3; ++i)
        {
            lastFacing.Add(new Vector2(0, 0));
        }
    }

    public void Setup(World world, float startx, float starty, int player)
    {
        this.world = world;
        sword.world = world;
        sword.owner = this;
        posx = startx;
        posy = starty;
        pnum = player;
        transform.position = new Vector3(posx, posy);
    }

    void Update()
    {
        float dx = 0.0f;
        float dy = 0.0f;
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

        if (dx != 0.0f || dy != 0.0f)
        {
            posx += dx * speed * Time.deltaTime;
            posy += dy * speed * Time.deltaTime;
            lastFacing.RemoveAt(0);
            lastFacing.Add(new Vector2(dx, dy));
            transform.position = new Vector3(posx, posy);
        }

        // Check for collisions with items to add them to sword
        for (int i = 0; i < world.swordParts.Count; ++i)
        {
            GameObject part = world.swordParts[i];
            if (Collision.dist(part.transform.position.x,
                               part.transform.position.y,
                               this.transform.position.x,
                               this.transform.position.y) < PRADIUS)
            {
                sword.AddPart(part);
                world.swordParts.RemoveAt(i);
                i -= 1;
            }
        }

        float fdx = 0.0f;
        float fdy = 0.0f;
        foreach (Vector2 vec in lastFacing)
        {
            fdx += vec.x;
            fdy += vec.y;
        }
        if (fdx < 0.0f)
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

        if (posx < -2880)
        {
            posx = -2880;
        }
        if (posx > 2880)
        {
            posx = 2880;
        }
        if (posy < -2160)
        {
            posy = -2160;
        }
        if (posy > 2160)
        {
            posy = 2160;
        }
        this.transform.position = new Vector3(posx, posy);
    }

    public void Damage(GameObject source, float damage, float weight)
    {
        // Apply knockback
        float dx = posx - source.transform.position.x;
        float dy = posy - source.transform.position.y;
        float a = Mathf.Atan2(dy, dx);
        posx += 7.0f * weight * Mathf.Cos(a);
        posy += 7.0f * weight * Mathf.Sin(a);
        transform.position = new Vector3(posx, posy);

        // Remove sword parts
        dmgLeftovers += damage;
        while (dmgLeftovers >= 1.0f)
        {
            dmgLeftovers -= 1.0f;
            sword.RemovePart();
        }
    }
}
