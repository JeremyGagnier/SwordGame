using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public GameObject p1Camera1;
    public GameObject p2Camera1;
    public GameObject p1Camera2;
    public GameObject p2Camera2;
    public GameObject p3Camera;
    public GameObject p4Camera;

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> swordPartPrefabs;
    [SerializeField] private List<GameObject> players;

    void Start()
    {
        //GetComponent<AudioSource>().Play();
    }

    public void Advance()
    {
        
    }

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public void SetPlayersAndCameras()
    {
        players[0].SetActive(true);
        players[1].SetActive(true);
        if (Game.numPlayers == 2)
        {
            p1Camera1.SetActive(true);
            p2Camera1.SetActive(true);
            p1Camera2.SetActive(false);
            p2Camera2.SetActive(false);
        }
        else
        {
            p1Camera1.SetActive(false);
            p2Camera1.SetActive(false);
            p1Camera2.SetActive(true);
            p2Camera2.SetActive(true);
        }

        if (Game.numPlayers >= 3)
        {
            p3Camera.SetActive(true);
            players[2].SetActive(true);
        }
        else
        {
            p3Camera.SetActive(false);
            players[2].SetActive(false);
        }

        if (Game.numPlayers >= 4)
        {
            p4Camera.SetActive(true);
            players[3].SetActive(true);
        }
        else
        {
            p4Camera.SetActive(false);
            players[3].SetActive(false);
        }
    }
    
    public SwordPart DropPart(GameObject swordPartObject, FVector pos)
    {
        GameObject obj = Instantiate(swordPartPrefabs[0]) as GameObject;
        obj.transform.SetParent(this.transform);
        obj.transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat());
        SwordPart part = obj.GetComponent<SwordPart>();
        return part;
    }
    
    public Enemy SummonEnemy(GameObject enemyObject, FVector pos)
    {
        GameObject obj = Instantiate(enemyObject);
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat());
        Enemy enemy = obj.GetComponent<Enemy>();
        return enemy;
    }
    
    public void KillEnemy(GameObject enemy)
    {
        DestroyObject(enemy);
    }
}
