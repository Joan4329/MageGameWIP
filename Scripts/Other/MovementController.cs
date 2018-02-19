using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MovementController : MonoBehaviour {

    protected Animator characterAnimator;
    protected CombatStats combatStats;
    protected EffectManager effectMngr;

    [SerializeField] private FactionType faction = FactionType.PlayerFaction;
    private ActionType currentAction = ActionType.None;

    protected ModifiableStat<MoveModifier, MoveAdditive> movementSpeed;
    // Currently have this 
    [SerializeField] private float movementSpeedSetter = 5f;
    //protected float movementSpeedModifier = 1f;
    protected float rotationSmoothness = 5f;

    public FactionType Faction
    {
        get { return faction; }
        set { faction = value; }
    }

    public ActionType CurrentAction
    {
        get { return currentAction; }
        set { currentAction = value; }
    }

    public bool IsAttackingMelee
    {
        get { return currentAction == ActionType.AttackingMelee; }
        set
        {
            if (value)
                currentAction = ActionType.AttackingMelee;
            else
                currentAction = ActionType.None;
        }
    }

    public bool IsAttackingRanged
    {
        get { return currentAction == ActionType.AttackingRanged; }
        set
        {
            if (value)
                currentAction = ActionType.AttackingRanged;
            else
                currentAction = ActionType.None;
        }
    }

    public bool IsCasting
    {
        get { return currentAction == ActionType.Casting; }
        set
        {
            if (value)
                currentAction = ActionType.Casting;
            else
                currentAction = ActionType.None;
        }
    }

    public ModifiableStat<MoveModifier, MoveAdditive> MovementSpeed
    {
        get { return movementSpeed; }
    }

    protected virtual void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        if (characterAnimator == null)
            characterAnimator = GetComponentInChildren<Animator>();

        combatStats = GetComponent<CombatStats>();
        effectMngr = GetComponent<EffectManager>();

        movementSpeed = new ModifiableStat<MoveModifier, MoveAdditive>(movementSpeedSetter, false, effectMngr);
    }

    protected virtual void Start()
    {
        combatStats.DeathEvent.AddListener(DeathActions);
        combatStats.DamageEvent.AddListener(TakeDamageActions);
    }

    protected virtual void Update()
    {

    }

    public enum FactionType
    {
        Neutral, // Does a neutral faction need to be handled differently in tasks and other checks. E.g. does FindTarget() need to exclude neutral targets? Should neutral targets only defend themselves or can there be aggressive neutral entities?
        PlayerFaction,
        Enemy,
        // Maybe other factions? This is only if I want to have 
        // TrollFaction
        // WolfFaction
    }

    public enum ActionType
    {
        None,
        AttackingMelee,
        AttackingRanged,
        Casting,

    }


    protected virtual void DeathActions()
    {
        characterAnimator.SetBool("IsAlive", false);
    }

    protected virtual void TakeDamageActions(Damage damage)
    {

    }
}
