using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rejuvenation : Spell
{
    private GameObject rejuvParticles;

    public Rejuvenation(float castTime, float cooldownTime, float manaCost, SpellAnimation spellAnimation, float animationSpeedOffset) : base(castTime, cooldownTime, manaCost, spellAnimation, animationSpeedOffset)
    {

        castingParticles = Resources.Load("Prefabs/Spells/CastingParticles/NatureCastingParticles") as GameObject;
        rejuvParticles = Resources.Load("Prefabs/Spells/RejuvenationParticles") as GameObject;
    }

    public override void Cast(Transform casterTransform, Quaternion targetDir)
    {
        // Place a heal over time buff on caster. Place a 15% speed penalty and a 50% mana regen bonus. All buffs lasts 10 seconds.

        GameObject particleInstance = MonoBehaviour.Instantiate(rejuvParticles, casterTransform.position, targetDir);
        particleInstance.transform.parent = casterTransform;
        MonoBehaviour.DestroyObject(particleInstance.gameObject, 4f);

        casterTransform.gameObject.GetComponent<EffectManager>().ApplyEffect(new MoveModifier(10f, 0.85f));
        casterTransform.gameObject.GetComponent<EffectManager>().ApplyEffect(new ManaRegenModifier(10f, 1.5f));
        casterTransform.gameObject.GetComponent<EffectManager>().ApplyEffect(new Regenerate(10f, new Damage(new Damage.DamageInfo(casterTransform.gameObject, DamageType.Nature, null, false), null, 3f)));

    }
}
