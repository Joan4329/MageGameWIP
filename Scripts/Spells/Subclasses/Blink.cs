using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Spell
{
    private float distance = 10f;
    private GameObject blinkParticles;

    public Blink(float castTime, float cooldownTime, float manaCost, SpellAnimation spellAnimation, float animationSpeedOffset, float distance) : base(castTime, cooldownTime, manaCost, spellAnimation, animationSpeedOffset)
    {
        this.distance = distance;

        castingParticles = Resources.Load("Prefabs/Spells/CastingParticles/ArcaneCastingParticles") as GameObject;
        blinkParticles = Resources.Load("Prefabs/Spells/BlinkParticles") as GameObject;
    }

    public override void Cast(Transform casterTransform, Quaternion targetDir)
    {
        // Teleport caster a distance in targeted direction. Provide a 50% movement speed buff for 2 seconds.

        GameObject firstBlinkParticle = MonoBehaviour.Instantiate(blinkParticles, casterTransform.position, targetDir);
        MonoBehaviour.DestroyObject(firstBlinkParticle.gameObject, 1f);

        casterTransform.rotation = targetDir;
        casterTransform.Translate(Vector3.forward * distance);

        GameObject secondBlinkParticle = MonoBehaviour.Instantiate(blinkParticles, casterTransform.position, targetDir);
        MonoBehaviour.DestroyObject(secondBlinkParticle.gameObject, 1f);

        casterTransform.gameObject.GetComponent<EffectManager>().ApplyEffect(new MoveModifier(2f, 1.5f));
    }
}
