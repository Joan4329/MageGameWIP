using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceShield : Spell
{
    private float bonusShield = 100f;
    public ForceShield(float castTime, float cooldownTime, float manaCost, SpellAnimation spellAnimation, float animationSpeedOffset, float bonusShield) : base(castTime, cooldownTime, manaCost, spellAnimation, animationSpeedOffset)
    {
        
        castingParticles = Resources.Load("Prefabs/Spells/CastingParticles/ArcaneCastingParticles") as GameObject;
    }

    public override void Cast(Transform casterTransform, Quaternion targetDir)
    {
        Effect e = new EnergyShieldAdditive(6f, bonusShield);
        casterTransform.gameObject.GetComponent<EffectManager>().ApplyEffect(e);   
    }
}
