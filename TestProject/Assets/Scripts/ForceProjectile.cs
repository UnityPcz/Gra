using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceProjectile : ProjectileScript {

    public TrailRenderer trail2;

    protected override void Awake()
    {
        base.Awake();
        trail2.Clear();
    }
}
