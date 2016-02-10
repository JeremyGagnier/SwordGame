using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Collision
{
    public static float dist(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}

public class World : MonoBehaviour
{
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

    public enum Enemies
    {
        BLOB,
        DOG,
        GOOSE,
        DINO
    }
    public GameObject blobPrefab;
    public GameObject dogEnemyPrefab;
    public GameObject goosePrefab;
    public GameObject dinoPrefab;

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
    private Player player1;
    private Player player2;
    private Player player3;
    private Player player4;

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
        player1.Setup(this, -300, 0, 1, 1);
        p2Object.SetActive(true);
        player2.Setup(this, 300, 0, 2, 2);
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
            player3.Setup(this, 0, -300, 3, 3);
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
            player4.Setup(this, 0, 300, 4, 4);
        }
        else
        {
            p4Camera.SetActive(false);
            p4Object.SetActive(false);
        }

        StartCoroutine(Timer());
    }

    public GameObject DropPart(Vector2 pos, Part part)
    {
        if (pos.x < -2880)
        {
            pos.x = -2880;
        }
        if (pos.x > 2880)
        {
            pos.x = 2880;
        }
        if (pos.y < -2160)
        {
            pos.y = -2160;
        }
        if (pos.y > 2160)
        {
            pos.y = 2160;
        }
        GameObject obj = this.gameObject;
        switch (part)
        {
            case Part.CACTUS:
                obj = Instantiate(cactusPrefab) as GameObject;
                obj.name = "Cactus";
                break;
            case Part.LEMON:
                obj = Instantiate(lemonPrefab) as GameObject;
                obj.name = "Lemon";
                break;
            case Part.FLOWER:
                obj = Instantiate(flowerPrefab) as GameObject;
                obj.name = "Flower";
                break;
            case Part.BLADE:
                obj = Instantiate(bladePrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.BUCKET:
                obj = Instantiate(bucketPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.DOG:
                obj = Instantiate(dogPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.LIGHTSABER:
                obj = Instantiate(lightsaberPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.LOBSTER:
                obj = Instantiate(lobsterPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.LOG:
                obj = Instantiate(logPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.PANTS:
                obj = Instantiate(pantsPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.PENCIL:
                obj = Instantiate(pencilPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.ANCHOR:
                obj = Instantiate(anchorPrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.UKELELE:
                obj = Instantiate(ukelelePrefab) as GameObject;
                //obj.name = "Flower";
                break;
            case Part.UKIWI:
                obj = Instantiate(ukiwiPrefab) as GameObject;
                //obj.name = "Flower";
                break;
        }
        obj.transform.SetParent(this.transform);
        obj.transform.position = new Vector3(pos.x, pos.y);
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        swordParts.Add(obj);
        return obj;
    }

    public GameObject SummonEnemy(Vector2 pos, Enemies enemy)
    {
        GameObject obj = this.gameObject;
        switch (enemy)
        {
            case Enemies.BLOB:
                obj = Instantiate(blobPrefab) as GameObject;
                obj.name = "Blob";
                break;
            case Enemies.DOG:
                obj = Instantiate(dogEnemyPrefab) as GameObject;
                obj.name = "Dog";
                break;
            case Enemies.GOOSE:
                obj = Instantiate(goosePrefab) as GameObject;
                obj.name = "Goose";
                break;
            case Enemies.DINO:
                obj = Instantiate(dinoPrefab) as GameObject;
                obj.name = "Dino";
                break;
        }
        obj.transform.SetParent(this.transform);
        Enemy e = obj.GetComponent<Enemy>();
        e.posx = pos.x;
        e.posy = pos.y;
        e.world = this;
        enemies.Add(obj);
        return obj;
    }

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
        DropPart(new Vector2(enemy.transform.position.x, enemy.transform.position.y), dropPart);

        enemies.Remove(enemy);
        DestroyObject(enemy);
    }

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
                int choose = UnityEngine.Random.Range(0, 26);
                int side = UnityEngine.Random.Range(0, 3);
                Vector2 vec = new Vector2();
                if (side == 0)
                {
                    int slider = UnityEngine.Random.Range(0, 4320);
                    vec = new Vector2(-2880, slider - 2160);
                }
                if (side == 1)
                {
                    int slider = UnityEngine.Random.Range(0, 5760);
                    vec = new Vector2(slider - 2880, 2160);
                }
                if (side == 2)
                {
                    int slider = UnityEngine.Random.Range(0, 4320);
                    vec = new Vector2(2880, slider - 2160);
                }
                if (side == 3)
                {
                    int slider = UnityEngine.Random.Range(0, 5760);
                    vec = new Vector2(slider - 2880, -2160);
                }

                if (choose == 0)
                {
                    SummonEnemy(vec, Enemies.DINO);
                }
                else if (choose <= 2)
                {
                    SummonEnemy(vec, Enemies.GOOSE);
                }
                else if (choose <= 8)
                {
                    SummonEnemy(vec, Enemies.DOG);
                }
                else if (choose <= 26)
                {
                    SummonEnemy(vec, Enemies.BLOB);
                }
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

    public void Attack(int sourceTeam, Transform source, float radius, Transform knockbackSource, float damage, float weight, Action callback)
    {
        if (player1.team != sourceTeam &&
            player1.invincibility <= 0.0f &&
            Collision.dist(source.position.x, source.position.y, player1.posx, player1.posy) <= Player.PRADIUS + radius)
        {
            player1.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player2.team != sourceTeam &&
            player2.invincibility <= 0.0f &&
            Collision.dist(source.position.x, source.position.y, player2.posx, player2.posy) <= Player.PRADIUS + radius)
        {
            player2.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player3.team != sourceTeam &&
            player3.invincibility <= 0.0f &&
            Collision.dist(source.position.x, source.position.y, player3.posx, player3.posy) <= Player.PRADIUS + radius)
        {
            player3.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }
        if (player4.team != sourceTeam &&
            player4.invincibility <= 0.0f &&
            Collision.dist(source.position.x, source.position.y, player4.posx, player4.posy) <= Player.PRADIUS + radius)
        {
            player4.Damage(knockbackSource, damage, weight);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }

        foreach (GameObject enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e.team != sourceTeam &&
                e.invincibility <= 0.0f &&
                Collision.dist(source.position.x, source.position.y, e.posx, e.posy) <= e.radius + radius)
            {
                e.Damage(knockbackSource, damage, weight);
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }
        }
    }
}
