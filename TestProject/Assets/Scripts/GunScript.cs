using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    public GameObject projectilePrefab;
    public float projectileSpeed = 10;
    public float fireCooldown = 0.5f; // mogłoby być użyte :D  
    public float cooldown = 0;
    public Transform aimerpos;
    public float recoil = 0;
    protected Rigidbody2D characterRB;
    public bool addInitialVelocity = true;

    private void Awake()
    {
        characterRB = transform.parent.gameObject.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        // odliczanie do kolejnego strzału
        if (cooldown > 0)
        { cooldown -= Time.deltaTime; }
    }
    // ruszanie celownikiem w górę lub w dół
    public void Aim(float aiminput)
    {
        transform.Rotate(0, 0, aiminput);

        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 180)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 90f);
        }

        if (transform.eulerAngles.z > 180 && transform.eulerAngles.z < 270)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 270);
        }
    }

    // celowanie w "punkt" 1 góra, 0 przed siebie, -1 w dół
    public void AimAt(float aiminput)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aiminput * 90f);
    }

    // oddanie strału
    public virtual bool Shoot()
    {
        if (cooldown <= 0)
        {
            Vector2 ProjectileVector = aimerpos.position - transform.position;
            
            ReleaseProjectile(ProjectileVector, projectilePrefab);
            characterRB.AddForce(-ProjectileVector * recoil * projectileSpeed);
            cooldown = fireCooldown;
            return true;
        }
        return false;
    }

    protected virtual void ReleaseProjectile(Vector2 ProjectileVector, GameObject projectilePrefab)
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position + new Vector3(ProjectileVector.x,ProjectileVector.y);
        ProjectileVector *= projectileSpeed;
        if (addInitialVelocity)
            projectile.GetComponent<Rigidbody2D>().velocity = ProjectileVector + characterRB.velocity;
        else
            projectile.GetComponent<Rigidbody2D>().velocity = ProjectileVector;
    }

}
