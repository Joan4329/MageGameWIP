using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunarBeam : Spell
{
    private GameObject beam;
    private float distance = 15f;
    private float damage = 10f;

    public LunarBeam(float castTime, float cooldownTime, float manaCost, SpellAnimation spellAnimation, float animationSpeedOffset, float damage) : base(castTime, cooldownTime, manaCost, spellAnimation, animationSpeedOffset)
    {
        this.damage = damage;
        beam = Resources.Load("Prefabs/Spells/LunarBeam") as GameObject;
        castingParticles = Resources.Load("Prefabs/Spells/CastingParticles/ArcaneCastingParticles") as GameObject;
    }

    public override void Cast(Transform casterTransform, Quaternion targetDir)
    {
        Vector3 casterPosModified = casterTransform.position + Vector3.up * 1.5f;
        GameObject lunarBeam = MonoBehaviour.Instantiate(beam, casterPosModified + casterTransform.forward * 1.5f, targetDir);
        Ray ray = new Ray(casterPosModified, targetDir * Vector3.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1f, distance, CombatStats.ATTACKABLE_LAYER);
        //Debug.DrawRay(casterPosModified, targetDir * Vector3.forward * distance, Color.blue, 10f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject != casterTransform.gameObject)
            {
                Damage dmg = new Damage(new Damage.DamageInfo(casterTransform.gameObject, DamageType.Arcane, null, true), null, damage);
                Vector3 hitPointYFix = hit.transform.position;
                hitPointYFix.y = lunarBeam.transform.position.y;
                Ray terrainRay = new Ray(casterPosModified, hitPointYFix - casterPosModified);
                RaycastHit terrainHit;
                //Debug.DrawRay(casterPosModified, (hitPointYFix - casterPosModified) * distance, Color.green, 10f);
                if (Physics.Raycast(terrainRay, out terrainHit, distance, CombatStats.SCENE_OBJECTS_LAYER))
                {
                    float targetDistance = Vector3.Distance(hit.point, casterPosModified);
                    float terrainDistance = Vector3.Distance(terrainHit.point, casterPosModified);

                    if (targetDistance < terrainDistance) 
                    {
                        //Debug.Log("Target closer: TargetDistance: " + targetDistance + " vs TerrainDistance: " + terrainDistance);
                        hit.transform.gameObject.GetComponent<CombatStats>().ApplyDamage(dmg);
                    }
                    else
                    {
                        //Debug.Log("Terrain closer: TerrainDistance: " + terrainDistance + " vs TargetDistance: " + targetDistance);
                    }
                        
                }
                else
                {
                    //Debug.Log("No Terrain");
                    hit.transform.gameObject.GetComponent<CombatStats>().ApplyDamage(dmg);
                }
            }
        }
        

        MonoBehaviour.DestroyObject(lunarBeam, 4f);
    }
}
