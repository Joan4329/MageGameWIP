using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectManager : MonoBehaviour {

    private CombatStats combatStats;

    private List<Effect> effectList = new List<Effect>();

    public List<Effect> EffectList
    {
        get { return effectList; }
        //set { effectList = value; }
    }

    private void Awake()
    {
        combatStats = GetComponent<CombatStats>();
    }

    // Use this for initialization
    void Start () {
        combatStats.DamageEvent.AddListener(TakeDamageActions);
    }

    private void ApplyEffects(Damage damage)
    {
        // Check if there are currently any action over time effects running.
        bool hasCurrentActionOverTimeEffect = effectList.Any((Effect effect) => effect.GetType().IsSubclassOf(typeof(ActionOverTimeEffect)));
        bool damageContainsActionOverTimeEffect = false;

        // Add effects from the damage source to the characters effect list and start a coroutine for effects that have a duration.
        if (damage.Effects != null)
        {
            effectList.AddRange(damage.Effects);
            // Buffs of 0 duration does not expire.
            foreach (Effect e in damage.Effects.FindAll((Effect effect) => effect.Duration > Effect.PERMANENT_DURATION))
            {
                StartCoroutine(RemoveEffectAfterExpired(e));
            }
            damageContainsActionOverTimeEffect = damage.Effects.Any((Effect effect) => effect.GetType().IsSubclassOf(typeof(ActionOverTimeEffect)));
        }

        // Start a coroutine only if there isn't one currently running (one will be running if there is a actionOverTimeEffect in effectList). Also don't start one if there isn't a actionOverTime in damage.Effects
        if (!hasCurrentActionOverTimeEffect && damageContainsActionOverTimeEffect)
            StartCoroutine(ProcessActionOverTimeEffects());
    }

    public void ApplyEffect(Effect e)
    {
        // Check if there are currently any action over time effects running.
        bool hasCurrentActionOverTimeEffect = effectList.Any((Effect effect) => effect.GetType().IsSubclassOf(typeof(ActionOverTimeEffect)));

        // Add effect to the characters effect list and start a coroutine for effects that have a duration.
        if (e != null)
        {
            effectList.Add(e);
            // Buffs of 0 duration does not expire.
            if (e.Duration > Effect.PERMANENT_DURATION)
                StartCoroutine(RemoveEffectAfterExpired(e));
        }

        // Start a coroutine only if there isn't one currently running (one will be running if there is a actionOverTimeEffect in effectList). Also don't start one if e isn't an actionOverTime.
        if (!hasCurrentActionOverTimeEffect && e.GetType().IsSubclassOf(typeof(ActionOverTimeEffect)))
            StartCoroutine(ProcessActionOverTimeEffects());
    }

    private IEnumerator ProcessActionOverTimeEffects()
    {
        // This whole method needs to be more efficient. Maybe add a global list of each effect type (using interfaces?) that is updated on each change instead.

        // Find all action over time effects affecting the character.
        List<ActionOverTimeEffect> actionOverTimeEffects = effectList.OfType<ActionOverTimeEffect>().ToList();
        while (actionOverTimeEffects.Count > 0 && combatStats.IsAlive)
        {
            // Wait slightly less than one second (in order to fit 4 ticks of damage on a 4 second duration spell, probably needs some rework) and then find all DoTs affecting the character.
            yield return new WaitForSeconds(0.99f);
            actionOverTimeEffects = effectList.OfType<ActionOverTimeEffect>().ToList();

            List<System.Type> typeList = new List<System.Type>();

            // Process each damage over time effect
            foreach (ActionOverTimeEffect aot in actionOverTimeEffects)
            {
                // Makes sure that several effects of the same type does not stack.
                // Later make sure that it is the strongest effect for each type that is applied. i.e. a burn that does 40 damage should take preference over a burn that does 20 damage.
                System.Type type = aot.GetType();
                if (!typeList.Contains(type))
                {
                    //Debug.Log("Time: " + Time.time + " Number of DoTs: " + actionOverTimeEffects.Count);
                    aot.Process(this.gameObject);
                    typeList.Add(type);
                }
            }
        }
    }

    private IEnumerator RemoveEffectAfterExpired(Effect e)
    {
        e.OnApplied(this.gameObject);
        //Debug.Log("Start: " + Time.time);
        yield return new WaitForSeconds(e.Duration);
        effectList.Remove(e);
        //Debug.Log("Stop: " + Time.time);
        e.OnExit(this.gameObject);
    }


    private void TakeDamageActions(Damage damage)
    {
        ApplyEffects(damage);
    }
}
