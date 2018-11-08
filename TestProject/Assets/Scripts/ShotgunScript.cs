using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : GunScript
{

    public float spread = 8;

    public override bool Shoot()
    {
        if (cooldown <= 0)
        {
            float radspread = Mathf.Deg2Rad * spread;

            Vector2 ProjectileVector = aimerpos.position - transform.position;
            ReleaseProjectile(ProjectileVector, projectilePrefab);

            float x = ProjectileVector.x * Mathf.Cos(radspread) - ProjectileVector.y * Mathf.Sin(radspread);
            float y = ProjectileVector.x * Mathf.Sin(radspread) + ProjectileVector.y * Mathf.Cos(radspread);
            ProjectileVector = new Vector3(x, y, 0);
            ReleaseProjectile(ProjectileVector, projectilePrefab);

            ProjectileVector = aimerpos.position - transform.position;
            x = ProjectileVector.x * Mathf.Cos(-radspread) - ProjectileVector.y * Mathf.Sin(-radspread);
            y = ProjectileVector.x * Mathf.Sin(-radspread) + ProjectileVector.y * Mathf.Cos(-radspread);
            ProjectileVector = new Vector3(x, y, 0);
            ReleaseProjectile(ProjectileVector, projectilePrefab);

            cooldown = fireCooldown;
            characterRB.AddForce(-ProjectileVector * recoil * projectileSpeed);
            return true;
        }
        return false;
    }
}
