using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour {
    // Write an abstract projectile class or interface that FireballProjectile extends

    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float maxDistance = 20f;
    private Vector3 startingPos;
    //private GameObject fireballExplosion;
    private float travelledDistance = 0f;
    private bool isExploding = false;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float blastDamage = 40f;

    [SerializeField] private GameObject projectileChildObject;
    [SerializeField] private GameObject explosionChildObject;

    private GameObject source;

    public GameObject Source
    {
        get { return source; }
        set { source = value; }
    }

    // Use this for initialization
    void Start () {
        startingPos = transform.position;
        explosionChildObject.SetActive(false);
        StartCoroutine(MoveForwardUntilMaxRange());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Also start a coroutine if it hasn't exploded on collision, in order to check if it is still colliding, since this method only happens the frame that the colliders meet.
        // This makes it so that the player can follow the projectile he casts and not be hit by it if he manages to stay within its collision box.

        // Check if it collides with caster. If it isn't caster it explodes immediately but otherwise the fireball must travel a certain distance before it explodes. (Avoids explosion on cast)
        if (!isExploding)
        {
            if (collision.gameObject != source.gameObject)
                Explode();
            else if (travelledDistance > 1f)
                Explode();
        }
        
    }

    void Explode()
    {
        // Deactivate collider, also set isExploding to true
        GetComponent<BoxCollider>().enabled = false;
        isExploding = true;

        // Stop emitting projectile particles.
        foreach (ParticleSystem ps in projectileChildObject.GetComponentsInChildren<ParticleSystem>())
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        // Activate explosion child object for explosion particles.
        explosionChildObject.SetActive(true);
        
        // Check hits in a sphere on attackable Layer and apply damage and burn on those who were hit.
        List<Collider> hits = new List<Collider>();
        //hits.AddRange(Physics.SphereCastAll(transform.position, blastRadius, Vector3.up, attackableLayer));
        hits.AddRange(Physics.OverlapSphere(transform.position, blastRadius, CombatStats.ATTACKABLE_LAYER));

        List<Effect> effects = new List<Effect>();
        effects.Add(new Burn(4f, new Damage(new Damage.DamageInfo(source, DamageType.Fire, null, false), null, 5f)));
        Damage damage = new Damage(new Damage.DamageInfo(source, DamageType.Fire, null, true), effects, blastDamage);
        foreach (Collider hit in hits)
        {
            hit.transform.gameObject.GetComponent<CombatStats>().ApplyDamage(damage);
        }

        // Destroy object after 3 seconds (enough time for explosion particles to finish. might start a coroutine that destroys the object once that particle system is no longer playing.
        Destroy(this.gameObject, 3f);
    }

    private IEnumerator MoveForwardUntilMaxRange()
    {
        while (!isExploding)
        {
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
            travelledDistance = Vector3.Distance(startingPos, transform.position);
            if (travelledDistance > maxDistance)
            {
                Explode();
            }
            yield return null;
        }
    }
}
