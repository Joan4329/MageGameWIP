using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell {

    public float castTime = 1f;
    private float castingTimer = 0f;
    public float cooldownTime = 1.5f;
    private float currentCDTimer = 0f;

    private float manaCost = 30f;
    private SpellAnimation spellAnimation = SpellAnimation.Standard;
    private float animationSpeedOffset = 0f;

    protected GameObject castingParticles;


    // This is temporary until I fix the casting code. This is needed for Spells to tell spellmanager to start its cooldown with a coroutine....
    //protected PlayerSpellManager spellMngr;

    private bool isCasting = false;
    protected bool isOnCooldown = false;

    public bool IsOnCooldown
    {
        get { return isOnCooldown; }
        set { isOnCooldown = value; }
    }

    public bool IsCasting
    {
        get { return isCasting; }
        set { isCasting = value; }
    }

    public float CastingTimer
    {
        get { return castingTimer; }
        set { castingTimer = value; }
    }

    public float CurrentCDTimer
    {
        get { return currentCDTimer; }
        set { currentCDTimer = value; }
    }

    public GameObject CastingParticles
    {
        get { return castingParticles; }
    }

    public float ManaCost
    {
        get { return manaCost; }
        //set { manaCost = value; }
    }

    public int SpellAnimationInt
    {
        get { return (int)spellAnimation; }
    }

    public float AnimationSpeedOffset
    {
        get { return animationSpeedOffset; }
    }

    public Spell(float castTime, float cooldownTime, float manaCost , SpellAnimation spellAnimation, float animationSpeedOffset)
    {
        this.castTime = castTime;
        this.cooldownTime = cooldownTime;
        this.manaCost = manaCost;
        this.spellAnimation = spellAnimation;
        this.animationSpeedOffset = animationSpeedOffset;
    }

    public abstract void Cast(Transform casterTransform, Quaternion targetDir);
    // Might need to rework this for other spells. i.e. should all spells have a casting time and then be cast or could some be channeling spells like Arcane Missiles in WoW.

    public enum SpellAnimation
    {
        Standard = 0,
        Buff = 1,
        MassiveBuff = 2,
        GroundSlam = 3,
        AreaAttack = 4,
        Beam = 5
    }
}
