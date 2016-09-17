using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> swordPartPrefabs;
    [SerializeField] private GameObject playerPrefab;

    private List<GameObject> players = new List<GameObject>();

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 1f);
        Gizmos.DrawLine(
            new Vector3(-width / 2, -height / 2),
            new Vector3(width / 2, -height / 2));
        Gizmos.DrawLine(
            new Vector3(-width / 2, -height / 2),
            new Vector3(-width / 2, height / 2));
        Gizmos.DrawLine(
            new Vector3(width / 2, height / 2),
            new Vector3(width / 2, -height / 2));
        Gizmos.DrawLine(
            new Vector3(width / 2, height / 2),
            new Vector3(-width / 2, height / 2));
    }

    void Start()
    {
        //GetComponent<AudioSource>().Play();
    }

    public void Advance()
    {
        
    }

    public List<Player> GetPlayers()
    {
        List<Player> tmp = new List<Player>();
        foreach (GameObject playerObj in players)
        {
            tmp.Add(playerObj.GetComponent<Player>());
        }
        return tmp;
    }

    public void Generate(int numPlayers)
    {
        #region World Wrapping
        GameObject mirror = Resources.Load<GameObject>("Prefabs/Mirror");
        List<Material> materials = new List<Material>();
        for (int i = 0; i < 9; ++i)
        {
            int x = (i % 3) - 1;
            int y = (i / 3) - 1;

            RenderTexture rTex = new RenderTexture(width, height, 24);

            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            mat.mainTexture = rTex;
            materials.Add(mat);

            GameObject camObj = new GameObject();
            camObj.transform.SetParent(transform);
            camObj.transform.position = new Vector3(width * x, -height * y, -100);
            camObj.name = string.Format("Mirror Camera ({0}, {1})", x, y);

            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
            cam.orthographic = true;
            cam.orthographicSize = height / 2;
            cam.targetTexture = rTex;
            if (i == 4)
            {
                cam.cullingMask = (1 << LayerMask.NameToLayer("Background")) | (1 << LayerMask.NameToLayer("Foreground"));
            }
            else
            {
                cam.cullingMask = 1 << LayerMask.NameToLayer("Foreground");
            }
        }

        for (int i = 0; i < 9; ++i)
        {
            int x = (i % 3) - 1;
            int y = (i / 3) - 1;

            GameObject mirrorObj = GameObject.Instantiate(mirror);
            mirrorObj.transform.position = new Vector3(width * x, -height * y, 0);
            mirrorObj.transform.localScale = new Vector3(width, height, 1);
            mirrorObj.transform.SetParent(transform);
            mirrorObj.name = string.Format("Mirror ({0}, {1})", x, y);

            Material[] matArray;
            if (i == 4)
            {
                matArray = new Material[8];
                for (int j = 0; j < 8; ++j)
                {
                    matArray[j] = materials[j + ((j >= 4) ? 1 : 0)];
                }
            }
            else
            {
                matArray = new Material[1];
                matArray[0] = materials[4];
            }
            mirrorObj.GetComponent<MeshRenderer>().materials = matArray;
        }
        #endregion

        #region Instatiate Players
        for (int p = 0; p < numPlayers; ++p)
        {
            CreatePlayer();
            AttachCameraToPlayer(
                p,
                (numPlayers == 2) ? 2 : 4,
                new Vector3(0f, 118f, -100f));
        }
        #endregion
    }

    private void CreatePlayer()
    {
        GameObject newPlayer = GameObject.Instantiate<GameObject>(playerPrefab);
        newPlayer.name = "Player " + (players.Count + 1).ToString();
        newPlayer.transform.SetParent(transform);
        players.Add(newPlayer);
    }

    private void AttachCameraToPlayer(int player, int numScreens, Vector3 position)
    {
        GameObject cameraObj = new GameObject("Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.transform.parent = players[player].transform;
        cameraObj.transform.localPosition = position;

        camera.clearFlags = CameraClearFlags.Color;
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
        if (pos.x < new FInt(-width) / 2)
        {
            px = pos.x + width;
        }
        else if (pos.x > new FInt(width) / 2)
        {
            px = pos.x - width;
        }
        else
        {
            px = new FInt(pos.x);
        }
        FInt py;
        if (pos.y < new FInt(-height) / 2)
        {
            py = pos.y + height;
        }
        else if (pos.y > new FInt(height) / 2)
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
