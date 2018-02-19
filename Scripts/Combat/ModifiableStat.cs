using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A stat/attribute that can be modified through an Effect.
/// </summary>
/// <typeparam name="T">T tells the class what ModifierEffect to check for when looking for modifiers to change the value.</typeparam>
/// <typeparam name="U">U tells the class what AdditiveEffect to check for when looking for bonus value.</typeparam>
public class ModifiableStat<T, U> where T : ModifierEffect where U : AdditiveEffect  {
    // Need to rethink the ModifierEffect and AdditiveEffect classes, since they're essentially the same except in name and that AdditiveEffect can have negative numbers.

    private float baseValue;
    private float baseModifier;
    private float bonusValue;
    private float totalModifier;
    private EffectManager effectManager;
    private bool isInteger;

    public ModifiableStat(float baseValue, bool isInteger, EffectManager effectManager)
    {
        if (baseValue < 0)
            this.baseValue = 0;
        else
            this.baseValue = baseValue;
        baseModifier = 1f;
        bonusValue = 0f;
        totalModifier = baseModifier;
        this.isInteger = isInteger;
        this.effectManager = effectManager;
    }


    public float Value
    {
        get
        {
            float val = (baseValue + bonusValue) * totalModifier;
            if (isInteger)
                val = Mathf.Round(val);
            return val;
        }
    }


    public void Update()
    {
        UpdateModifier();
        UpdateBonus();
    }


    private void UpdateModifier()
    {
        List<T> modifiers = effectManager.EffectList.OfType<T>().ToList();
        totalModifier = baseModifier;
        if (modifiers.Count < 1)
            return;

        foreach (T mm in modifiers)
        {
            totalModifier *= mm.Magnitude;
        }
        if (totalModifier < 0f)
            totalModifier = 0f;
    }


    private void UpdateBonus()
    {
        List<U> bonuses = effectManager.EffectList.OfType<U>().ToList();
        bonusValue = 0f;
        if (bonuses.Count < 1)
            return;

        foreach (U mm in bonuses)
        {
            bonusValue += mm.Magnitude;
        }

        if (bonusValue < 0f)
            bonusValue = 0f;
    }
}