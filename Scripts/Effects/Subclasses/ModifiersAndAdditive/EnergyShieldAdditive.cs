using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldAdditive : AdditiveEffect
{
    private GameObject energyShieldFXInstance;
    private CombatStats affectedTargetCombatStats;

    public EnergyShieldAdditive(float duration, float magnitude) : base(duration, magnitude)
    {
        effectParticles = Resources.Load("Prefabs/Spells/ForceShield") as GameObject;
    }

    public override void OnApplied(GameObject affectedTarget)
    {
        // Should probably check first if there is an effect of this type already.

        affectedTargetCombatStats = affectedTarget.gameObject.GetComponent<CombatStats>();
        affectedTargetCombatStats.MaxShield.Update();
        affectedTargetCombatStats.CurrentShield += Magnitude;

        // Move to EntityParticlesManager
        energyShieldFXInstance = MonoBehaviour.Instantiate(effectParticles, affectedTarget.transform.position + Vector3.up * 1f, affectedTarget.transform.rotation, affectedTarget.transform); 
    }

    public override void OnExit(GameObject affectedTarget)
    {
        affectedTargetCombatStats.MaxShield.Update();
        // Update current shield value. This will cause it to cap out at the maxShield value
        affectedTargetCombatStats.CurrentShield -= 0f;

        // Move to EntityParticlesManager
        foreach (ParticleSystem ps in energyShieldFXInstance.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            MonoBehaviour.DestroyObject(energyShieldFXInstance, 3f);
        }
    }
}
