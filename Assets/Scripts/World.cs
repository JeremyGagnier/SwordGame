using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    [SerializeField] private FInt width;
    [SerializeField] private FInt height;

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> swordPartPrefabs;
    [SerializeField] private GameObject playerPrefab;

    private List<GameObject> players = new List<GameObject>();

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawLine(
            new Vector3(-width.ToFloat() / 2, -height.ToFloat() / 2),
            new Vector3(width.ToFloat() / 2, -height.ToFloat() / 2));
        Gizmos.DrawLine(
            new Vector3(-width.ToFloat() / 2, -height.ToFloat() / 2),
            new Vector3(-width.ToFloat() / 2, height.ToFloat() / 2));
        Gizmos.DrawLine(
            new Vector3(width.ToFloat() / 2, height.ToFloat() / 2),
            new Vector3(width.ToFloat() / 2, -height.ToFloat() / 2));
        Gizmos.DrawLine(
            new Vector3(width.ToFloat() / 2, height.ToFloat() / 2),
            new Vector3(-width.ToFloat() / 2, height.ToFloat() / 2));
    }

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

    private void CreatePlayer()
    {
        GameObject newPlayer = GameObject.Instantiate<GameObject>(playerPrefab);
        newPlayer.name = "Player " + (players.Count + 1).ToString();
        newPlayer.transform.parent = this.transform;
        players.Add(newPlayer);
    }

    public void SetPlayersAndCameras()
    {
        for (int p = 0; p < Game.numPlayers; ++p)
        {
            CreatePlayer();
            for (int x = 0; x < 3; ++x)
            {
                for (int y = 0; y < 3; ++y)
                {
                    if (x == 1 && y == 1)
                    {
                        continue;
                    }
                    AttachCameraToPlayer(
                        p,
                        (Game.numPlayers == 2) ? 2 : 4,
                        new Vector3(
                            width.ToFloat() * (x - 1),
                            height.ToFloat() * (y - 1) + 118f,
                            -10f));
                }
            }
            // Add the main camera last so that unity renders it last.
            AttachCameraToPlayer(
                p,
                (Game.numPlayers == 2) ? 2 : 4,
                new Vector3(0f, 118f, -10f));
        }
    }
    
    private void AttachCameraToPlayer(int player, int numScreens, Vector3 position)
    {
        GameObject cameraObj = new GameObject("Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.transform.parent = players[player].transform;
        cameraObj.transform.localPosition = position;

        camera.clearFlags = CameraClearFlags.Nothing;
        camera.renderingPath = RenderingPath.Forward;
        camera.orthographic = true;
        switch (numScreens)
        {
            case 2:
                camera.orthographicSize = 800;
                switch (player)
                {
                    case 0:
                        camera.rect = new Rect(0, 0, 0.5f, 1);
                        break;
                    case 1:
                        camera.rect = new Rect(0.5f, 0, 0.5f, 1);
                        break;
                }
                break;
            case 4:
                camera.orthographicSize = 500;
                switch (player)
                {
                    case 0:
                        camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                        break;
                    case 1:
                        camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        break;
                    case 2:
                        camera.rect = new Rect(0, 0, 0.5f, 0.5f);
                        break;
                    case 3:
                        camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                        break;
                }
                break;
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

    public FVector GetWrappedPosition(FVector pos)
    {
        FInt px;
        if (pos.x < -width / 2)
        {
            px = pos.x + width;
        }
        else if (pos.x > width / 2)
        {
            px = pos.x - width;
        }
        else
        {
            px = new FInt(pos.x);
        }
        FInt py;
        if (pos.y < -height / 2)
        {
            py = pos.y + height;
        }
        else if (pos.y > height / 2)
        {
            py = pos.y - height;
        }
        else
        {
            py = new FInt(pos.y);
        }
        return new FVector(px, py);
    }
}
