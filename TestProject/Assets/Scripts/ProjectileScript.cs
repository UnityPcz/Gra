using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileScript : MonoBehaviour
{
    public float projectileLifetime = 10;
    public float explosionRadius = 5;
    public float explosionPushForce = 300;
    public float trailJitter = 0;
    public int projectileDamage = 10;

    Vector2 lastframepos;
    Map map;
    Rigidbody2D projectileRB;
    SpriteRenderer projectileSpriteRenderer;
    protected GameObject trail;
    float CollisionModeTimer = 0.1f;


    // Use this for initialization
    protected virtual void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
        projectileSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    protected virtual void Awake()
    {
        // przydatne do obliczania kierunku lotu pocisku
        trail = gameObject.transform.GetChild(0).gameObject;
        trail.GetComponent<TrailRenderer>().Clear();
        projectileRB = GetComponent<Rigidbody2D>();
        projectileRB.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        StartCoroutine(ChangeCollisionMode());
        lastframepos = transform.position;
    }

    IEnumerator ChangeCollisionMode()
    {
        yield return new WaitForSeconds(CollisionModeTimer);
        projectileRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CountDownLifetime();
        CalculateRotation();
        RandomiseTrailPos(trailJitter);
    }

    void RandomiseTrailPos(float jitter)
    {
        trail.transform.localPosition = new Vector3(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter), 0);
    }

    private void LateUpdate()
    {
        // przydatne do obliczania kierunku lotu pocisku
        lastframepos = transform.position;
    }

    // pocisk po x sekundach od pojawienia się znika w zależności od początkowej wartości zmiennej projectileLifetime
    void CountDownLifetime()
    {
        projectileLifetime -= Time.deltaTime;

        if (projectileLifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    // ustawianie pocisku przodem do toru lotu
    void CalculateRotation()
    {
        float deltaX = lastframepos.x - transform.position.x;
        float deltaY = lastframepos.y - transform.position.y;
        float angle = 0;
        if (lastframepos.x != transform.position.x && lastframepos.y != transform.position.y)
        {

            if (projectileRB.velocity.x > 0)
            {
                angle = Mathf.Atan(deltaY / (deltaX + 0.0001f)) * 180 / Mathf.PI;
                projectileSpriteRenderer.flipY = false;
            }

            else
            {
                angle = (Mathf.Atan(deltaY / (deltaX + 0.0001f)) * 180 / Mathf.PI) + 180;
                projectileSpriteRenderer.flipY = true;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        if (projectileRB.velocity.x == 0)
        {
            if (projectileRB.velocity.y > 0) angle = 90;
            else angle = 270;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    // no cóż... boom
    void Explosion(float radius, Vector3Int hitPos)
    {
        float x = Mathf.Round(hitPos.x - radius);
        float y = Mathf.Round(hitPos.y - radius);

        for (float i = x; i <= x + Mathf.Round(2 * radius); i++)
        {
            for (float j = y; j <= y + Mathf.Round(2 * radius); j++)
            {

                if (Vector2.Distance(new Vector2(i, j), new Vector2(hitPos.x, hitPos.y)) < radius)
                {
                    map.Chunk[(int)i / Map.chunksize, (int)j / Map.chunksize].SetTile(new Vector3Int((int)i, (int)j, 0), null);
                }
            }
        }
    }

 
    //reakcja na kolizje (głównie z mapą)
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 hitPosition = Vector3.zero;
        Tilemap tilemap;

        if (collision.collider.tag == "Map")
        {

            tilemap = collision.gameObject.GetComponent<Tilemap>();
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                Explosion(explosionRadius, tilemap.WorldToCell(hitPosition));
                Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, explosionRadius * 0.2f);
                List<GameObject> hitlist = new List<GameObject>();
                foreach (Collider2D c in collider)
                {                    
                    if (c.tag != "Map" && c != null && c.gameObject != gameObject)
                    {
                        Vector2 force = (c.transform.position - transform.position) / Vector2.Distance(c.transform.position, transform.position);
                        c.GetComponent<Rigidbody2D>().AddForce(force * explosionPushForce);
                    }
                    if (c.tag == "Player")
                    {
                        bool found = false;
                        foreach(GameObject g in hitlist)
                        {
                            if (c.gameObject == g)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            float damage = Vector2.Distance(hitPosition, new Vector2(c.transform.position.x,c.transform.position.y));
                            
                            damage = projectileDamage * (((explosionRadius / 10)+ 1)/damage);
                            c.gameObject.GetComponent<Platformer2DUserControl>().hitpoints -= (int)damage;
                            hitlist.Add(c.gameObject);
                        }

                    }
                }
                Destroy(gameObject);
                break;
            }
        }
    }

}
