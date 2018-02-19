using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityParticlesManager : MonoBehaviour {

    // Create a base class for PlayerSpellManager called SpellManager.
    private PlayerSpellManager spellMngr;

    // Character bones for hands
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    // Currently instantiated particle system when casting spells.
    GameObject castParticlesHandLeft = null;
    GameObject castParticlesHandRight = null;

    private void Awake()
    {
        spellMngr = GetComponent<PlayerSpellManager>();
    }

    // Use this for initialization
    void Start () {
        if (leftHand == null || rightHand == null)
            throw new System.Exception("Left or right hand is null in an EntityParticlesManager on " + gameObject.name);

        spellMngr.AddOnStartCastingListener(StartCasting);
        spellMngr.AddOnStopCastingListener(StopCasting);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void StartCasting()
    {
        //Instantiate the particles in each hand. Can probably be rewritten a bit more elegant.
        castParticlesHandLeft = Instantiate(spellMngr.CurrentSelectedSpell.CastingParticles);
        castParticlesHandRight = Instantiate(spellMngr.CurrentSelectedSpell.CastingParticles);
        castParticlesHandLeft.transform.parent = leftHand.transform;
        castParticlesHandRight.transform.parent = rightHand.transform;
        castParticlesHandLeft.transform.position = leftHand.transform.position;
        castParticlesHandRight.transform.position = rightHand.transform.position;
    }

    private void StopCasting()
    {
        //if (spellMngr.CurrentSelectedSpell != null)
        if (castParticlesHandLeft != null)
        {
            foreach (ParticleSystem ps in castParticlesHandLeft.GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(castParticlesHandLeft, 2f);
        }

        if (castParticlesHandRight != null)
        {
            foreach (ParticleSystem ps in castParticlesHandRight.GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(castParticlesHandRight, 2f);
        }
    }
}
