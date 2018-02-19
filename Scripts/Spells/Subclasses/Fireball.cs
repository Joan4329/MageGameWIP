using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    GameObject projectile;
    GameObject explosion;
    

    public Fireball(float castTime, float cooldownTime, float manaCost, SpellAnimation spellAnimation, float animationSpeedOffset) : base(castTime, cooldownTime, manaCost, spellAnimation, animationSpeedOffset)
    {

        // Add variables for AoE, cast SFX, projectile SFX, Blast SFX
        
        projectile = Resources.Load("Prefabs/Spells/FireballProjectile") as GameObject;
        castingParticles = Resources.Load("Prefabs/Spells/CastingParticles/FireCastingParticles") as GameObject;
    }


    public override void Cast(Transform casterTransform, Quaternion targetDir)
    {
        Vector3 startingPosMod = (Vector3.up + casterTransform.forward) * 1.5f;
        GameObject fireballProj = MonoBehaviour.Instantiate(projectile, casterTransform.position + startingPosMod, targetDir);
        fireballProj.GetComponent<FireballProjectile>().Source = casterTransform.gameObject;
            
        
        
    }
}
