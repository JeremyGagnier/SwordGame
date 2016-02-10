using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public float posx = 0.0f;
    public float posy = 0.0f;
    public int team;
    public float invincibility = 0.0f;

    public void Update()
    {
        if (invincibility > 0.0f)
        {
            invincibility -= Time.deltaTime;
        }
        transform.position = new Vector3(posx, posy);
    }
}
