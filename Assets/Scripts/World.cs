using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Collision
{
    public static FInt dist(FInt x1, FInt y1, FInt x2, FInt y2)
    {
        return FInt.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}

public class World : MonoBehaviour
{
    // TODO: Automate this
    public enum Part
    {
        CACTUS,
        LEMON,
        FLOWER,
        BLADE,
        BUCKET,
        DOG,
        LIGHTSABER,
        LOBSTER,
        LOG,
        PANTS,
        PENCIL,
        ANCHOR,
        UKELELE,
        UKIWI
    }
    public GameObject cactusPrefab;
    public GameObject lemonPrefab;
    public GameObject flowerPrefab;
    public GameObject bladePrefab;
    public GameObject bucketPrefab;
    public GameObject dogPrefab;
    public GameObject lightsaberPrefab;
    public GameObject lobsterPrefab;
    public GameObject logPrefab;
    public GameObject pantsPrefab;
    public GameObject pencilPrefab;
    public GameObject anchorPrefab;
    public GameObject ukelelePrefab;
    public GameObject ukiwiPrefab;
    
    public List<GameObject> enemyPrefabs;

    public GameObject titleScreen;

    public GameObject p1Camera1;
    public GameObject p2Camera1;
    public GameObject p1Camera2;
    public GameObject p2Camera2;
    public GameObject p3Camera;
    public GameObject p4Camera;

    public GameObject p1Object;
    public GameObject p2Object;
    public GameObject p3Object;
    public GameObject p4Object;
    public Player player1;
    public Player player2;
    public Player player3;
    public Player player4;

    public int numPlayers;
    public int timeLeft;
    public Text timeText;
    public Text egt1;
    public Text egt2;

    public List<GameObject> swordParts = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        //GetComponent<AudioSource>().Play();
        player1 = p1Object.GetComponent<Player>();
        player2 = p2Object.GetComponent<Player>();
        player3 = p3Object.GetComponent<Player>();
        player4 = p4Object.GetComponent<Player>();
    }

    public void StartGame(int numPlayers)
    {
        titleScreen.SetActive(false);
        this.numPlayers = numPlayers;

        p1Object.SetActive(true);
        player1.Setup(this, new FInt(-300), new FInt(0), 1, 1);
        p2Object.SetActive(true);
        player2.Setup(this, new FInt(300), new FInt(0), 2, 2);
        if (numPlayers == 2)
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

        if (numPlayers >= 3)
        {
            p3Camera.SetActive(true);
            p3Object.SetActive(true);
            player3.Setup(this, new FInt(0), new FInt(-300), 3, 3);
        }
        else
        {
            p3Camera.SetActive(false);
            p3Object.SetActive(false);
        }

        if (numPlayers >= 4)
        {
            p4Camera.SetActive(true);
            p4Object.SetActive(true);
            player4.Setup(this, new FInt(0), new FInt(300), 4, 4);
        }
        else
        {
            p4Camera.SetActive(false);
            p4Object.SetActive(false);
        }

        StartCoroutine(Timer());
    }

    // TODO: Automate this
    public GameObject DropPart(FVector pos, Part part)
    {
        GameObject obj = Instantiate(logPrefab) as GameObject;
        SwordPart objPart = obj.GetComponent<SwordPart>();
        objPart.position = pos;
        obj.transform.SetParent(this.transform);
        obj.transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat());
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        swordParts.Add(obj);
        return obj;
    }

    // TODO: Automate this
    public GameObject SummonEnemy(FVector pos)
    {
        GameObject obj = Instantiate(enemyPrefabs[0]);
        obj.transform.SetParent(this.transform);
        Enemy e = obj.GetComponent<Enemy>();
        e.position.x = pos.x;
        e.position.y = pos.y;
        e.transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat());
        e.world = this;
        enemies.Add(obj);
        return obj;
    }

    // TODO: Automate this
    public void KillEnemy(GameObject enemy)
    {
        Part dropPart = Part.CACTUS;
        Enemy e = enemy.GetComponent<Enemy>();
        if (e.rarity == 1)
        {
            int pick = UnityEngine.Random.Range(0, 3);
            if (pick == 0) dropPart = Part.BUCKET;
            if (pick == 1) dropPart = Part.LOG;
            if (pick == 2) dropPart = Part.FLOWER;
            if (pick == 3) dropPart = Part.PENCIL;
        }
        else if (e.rarity == 2)
        {
            int pick = UnityEngine.Random.Range(0, 2);
            if (pick == 0) dropPart = Part.CACTUS;
            if (pick == 1) dropPart = Part.LEMON;
            if (pick == 2) dropPart = Part.UKELELE;
        }
        else if (e.rarity == 3)
        {
            int pick = UnityEngine.Random.Range(0, 3);
            if (pick == 0) dropPart = Part.ANCHOR;
            if (pick == 1) dropPart = Part.BLADE;
            if (pick == 2) dropPart = Part.LOBSTER;
            if (pick == 3) dropPart = Part.PANTS;
        }
        else if (e.rarity == 4)
        {
            int pick = UnityEngine.Random.Range(0, 2);
            if (pick == 0) dropPart = Part.DOG;
            if (pick == 1) dropPart = Part.UKIWI;
            if (pick == 2) dropPart = Part.LIGHTSABER;
        }
        DropPart(new FVector(e.position.x, e.position.y), dropPart);

        enemies.Remove(enemy);
        DestroyObject(enemy);
    }

    // TODO: Remove vital game code, this isn't deterministic
    private IEnumerator Timer()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeLeft -= 1;
            string minutes = (timeLeft % 60).ToString();
            if (minutes.Length == 1)
            {
                timeText.text = (timeLeft / 60).ToString() + ":0" + minutes;
            }
            else
            {
                timeText.text = (timeLeft / 60).ToString() + ":" + minutes;
            }

            if (timeLeft > 110 && (timeLeft % (5 - numPlayers) == 0))
            {
                int side = UnityEngine.Random.Range(0, 3);
                FVector vec = new FVector();
                if (side == 0)
                {
                    int slider = UnityEngine.Random.Range(0, 4320);
                    vec = new FVector(new FInt(-2880), new FInt(slider - 2160));
                }
                if (side == 1)
                {
                    int slider = UnityEngine.Random.Range(0, 5760);
                    vec = new FVector(new FInt(slider - 2880), new FInt(2160));
                }
                if (side == 2)
                {
                    int slider = UnityEngine.Random.Range(0, 4320);
                    vec = new FVector(new FInt(2880), new FInt(slider - 2160));
                }
                if (side == 3)
                {
                    int slider = UnityEngine.Random.Range(0, 5760);
                    vec = new FVector(new FInt(slider - 2880), new FInt(-2160));
                }

                SummonEnemy(vec);
            }
        }

        EndGame();
    }

    public void EndGame()
    {
        int winner_size = p1Object.GetComponent<Player>().sword.parts.Count;
        int winner = 1;
        if (p2Object.GetComponent<Player>().sword.parts.Count > winner_size)
        {
            winner_size = p2Object.GetComponent<Player>().sword.parts.Count;
            winner = 2;
        }
        if (p3Object.GetComponent<Player>().sword.parts.Count > winner_size)
        {
            winner_size = p3Object.GetComponent<Player>().sword.parts.Count;
            winner = 3;
        }
        if (p4Object.GetComponent<Player>().sword.parts.Count > winner_size)
        {
            winner_size = p4Object.GetComponent<Player>().sword.parts.Count;
            winner = 4;
        }
        egt1.enabled = true;
        egt1.text = "Player" + winner.ToString() + " Wins!!!";
        egt2.enabled = true;
        egt2.text = "Their sword had " + (winner_size + 1).ToString() + " parts";
    }

    public void Attack(int sourceTeam, FVector source, FInt radius, FVector knockbackSource, FInt damage, FInt weight, Action callback)
    {
        if (player1.team != sourceTeam &&
            player1.invincibility.rawValue <= 0 &&
            Collision.dist(source.x, source.y, player1.position.x, player1.position.y) <= Player.PRADIUS + radius)
        {
            player1.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player2.team != sourceTeam &&
            player2.invincibility.rawValue <= 0 &&
            Collision.dist(source.x, source.y, player2.position.x, player2.position.y) <= Player.PRADIUS + radius)
        {
            player2.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player3.team != sourceTeam &&
            player3.invincibility.rawValue <= 0 &&
            Collision.dist(source.x, source.y, player3.position.x, player3.position.y) <= Player.PRADIUS + radius)
        {
            player3.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player4.team != sourceTeam &&
            player4.invincibility.rawValue <= 0 &&
            Collision.dist(source.x, source.y, player4.position.x, player4.position.y) <= Player.PRADIUS + radius)
        {
            player4.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }

        for (int i = 0; i < enemies.Count; ++i)
        {
            int startlen = enemies.Count;
            Enemy e = enemies[i].GetComponent<Enemy>();
            if (e.team != sourceTeam &&
                e.invincibility.rawValue <= 0 &&
                Collision.dist(source.x, source.y, e.position.x, e.position.y) <= e.radius + radius)
            {
                e.Damage(knockbackSource, damage, weight);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }
            // Check if the enemy died
            // TODO: Improve this flow because this is a hack
            if (enemies.Count != startlen)
            {
                i -= 1;
            }
        }
    }
}
