using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage {

    public struct DamageInfo
    {
        private GameObject damageSource;
        private DamageType type;
        private GameObject particleEffect;
        private bool isDirectHit;

        public GameObject DamageSource
        {
            get { return damageSource; }
            //set { damageSource = value; }
        }

        public DamageType Type
        {
            get { return type; }
            set { type = value; }
        }

        public GameObject ParticleEffect
        {
            get { return particleEffect; }
            //set { particleEffect = value; }
        }

        public bool IsDirectHit
        {
            get { return isDirectHit; }
            //set { isDirectHit = value; }
        }

        public DamageInfo(GameObject damageSource, DamageType type, GameObject particleEffect, bool isDirectHit)
        {
            this.damageSource = damageSource;
            this.type = type;
            this.particleEffect = particleEffect;
            this.isDirectHit = isDirectHit;
        }
    }

    private DamageInfo info;
    private List<Effect> effects = new List<Effect>();
    private float magnitude;

    public DamageInfo Info
    {
        get { return info; }
        //set { info = value; }
    }

    public List<Effect> Effects
    {
        get { return effects; }
        //set { effects = value; }
    }

    public float Magnitude
    {
        get { return magnitude; }
        set
        {
            magnitude = Mathf.Abs(value);
        }
    }


    public Damage(DamageInfo info, List<Effect> effects, float magnitude)
    {
        this.info = info;
        this.effects = effects;
        Magnitude = magnitude;
    }
}

public enum DamageType
{
    Physical,
    Arcane,
    Fire,
    Cold,
    Lightning,
    Nature
}
