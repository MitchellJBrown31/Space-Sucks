using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking;
using Unity.Netcode;

public class PreyHealth : NetworkBehaviour
{

    #region Health Variables
    public NetworkVariable<bool> isInjured, isFainted;
    [Tooltip("Period of time in seconds for invulnerability after revived or attacked")]
    [SerializeField] private float iFramePeriod;
    private int matchFaintCount;
    #endregion

    private NetworkVariable<bool> canBeHit;
    public NetworkVariable<bool> rescuingTeammate;

    public event Action event_OnTookDamage;
    public event Action event_OnRescued;

    public GameObject healEffect;

    private BodyMovement bodyMovement;
    private Perks perks;

    private GameObject rescueArea;
    public GameObject RescueArea { get { return rescueArea; } set { rescueArea = value; } }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip sound_WhenHit_Injured;
    [SerializeField] private AudioClip sound_WhenHit_Fainted;
    [SerializeField] private AudioClip sound_WhenRescued;
    [SerializeField] private AudioClip sound_WhenCompletedRescue;
    //this occurs everytime this object is spawned across the network
    public override void OnNetworkSpawn()
    {
        bodyMovement = GetComponent<BodyMovement>();
        perks = GetComponent<Perks>();
        isInjured.Value = false;
        isFainted.Value = false;
        rescuingTeammate.Value = false;
        canBeHit.Value = true;
        matchFaintCount = 0;

        isInjured.OnValueChanged += OnInjuredChanged;
        isFainted.OnValueChanged += OnFaintedChanged;
        rescuingTeammate.OnValueChanged += OnRescuingChanged;
    }

    private void Start()
    {
        if (IsOwner)
        {
            HealthStateIcons.Instance?.SetHealthyStateIcon(bodyMovement.characterId, true);
        }
    }

    public void HandleAttack(float damage)
    {

        if (!canBeHit.Value)
            return;

        //I am assuming we dont want the prey to be able to be attacked while they are fainted
        if (!isFainted.Value)
        {

            if (isInjured.Value)
            {

                if (IsOwner)
                    AudioManager.Instance.LoanOneShotSource(AudioCatagories.SFX, sound_WhenHit_Fainted);

                isInjured.Value = false;
                isFainted.Value = true;
                matchFaintCount++;
                //TakeDamage(damage);

                //Debug.Log("Help, I've fallen, and I can't get up (without the help of a teammate).");

                //here we can calculate how long we want the prey to be locked for based on number of times they fainted in the match
                //this is just a filler calculation that can be replaced

            }
            else
            {
                //Debug.Log("I'm one of Lifehouse's biggest songs - Halfway Gone");

                if (IsOwner)
                    AudioManager.Instance.LoanOneShotSource(AudioCatagories.SFX, sound_WhenHit_Injured);

                isInjured.Value = true;
                //currentSpeed = speed * 2;
                //currentFoodCount = 0; //maybe in the future also call a function before this to physically drop food assets
                //TakeDamage(damage);
                canBeHit.Value = false;
                perks.QuickGetaway();
                StartCoroutine(IFramePeriod());

            }

            event_OnTookDamage?.Invoke();

        }
    }

    public void ProcessAttack()
    {
        WasHitServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void WasHitServerRpc()
    {
        HandleAttack(1);
    }

    public bool PreyCanStun()
    {
        return (int)(prey.HEDGEHOG) == bodyMovement.characterId;
    }

    public void HandleRescue()
    {
        isFainted.Value = false;
        isInjured.Value = true;
        canBeHit.Value = false;
        StartCoroutine(IFramePeriod());

        AudioManager.Instance.LoanOneShotSource(AudioCatagories.SFX, sound_WhenRescued);

        event_OnRescued?.Invoke();

        HealingClientRPC();

    }

    [ClientRpc]
    public void HealingClientRPC() 
    {
        healEffect.SetActive(true);
        StartCoroutine(DelayedFunction(() => { healEffect.SetActive(false); }, 3f)); 
            
        //used for playing heal particle on rezzed player
     }

    IEnumerator DelayedFunction(Action toPerform, float toWait)
    {
        yield return new WaitForSeconds(toWait);
        toPerform?.Invoke();
    }

    public void ProcessRescue()
    {
        WasRescuedServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void WasRescuedServerRpc()
    {
        HandleRescue();
    }

    public void ToggleRescuingTeammate()
    {
        AudioManager.Instance.LoanOneShotSource(AudioCatagories.SFX, sound_WhenCompletedRescue);
        ToggleRescuingTeammateServerRpc();
    }

    [ServerRpc]
    public void ToggleRescuingTeammateServerRpc()
    {
        rescuingTeammate.Value = !rescuingTeammate.Value;
    }

    [ServerRpc]
    public void HandleSelfHealServerRpc()
    {
        isInjured.Value = false;

        HealingClientRPC();
    }


    public void OnInjuredChanged(bool previous, bool current)
    {
        //inform UI
        //Debug.Log($"Injured State Changed from {previous} to {current}");
        event_OnTookDamage?.Invoke();
        if (IsOwner)
        {
            HealthStateIcons.Instance.SetInjuredStateIcon(bodyMovement.characterId, current);
            HealthStateIcons.Instance.SetHealthyStateIcon(bodyMovement.characterId, !(isInjured.Value || isFainted.Value));
        }
    }

    public void OnFaintedChanged(bool previous, bool current)
    {
        //inform UI
        //Debug.Log($"Fainted State Changed from {previous} to {current}");
        rescueArea.SetActive(current);
        event_OnTookDamage?.Invoke();
        if (IsOwner)
        {
            HealthStateIcons.Instance.SetFaintedStateIcon(bodyMovement.characterId, current);
            HealthStateIcons.Instance.SetHealthyStateIcon(bodyMovement.characterId, !(isInjured.Value || isFainted.Value));
        }
    }

    public void OnRescuingChanged(bool previous, bool current)
    {
        //Debug.Log($"Rescuing state changed from {previous} to {current}");
    }

    /*
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            //I am just destroying the game object for now. we can replace this with whatever we want to happen to the prey when they die.
            Destroy(gameObject);
        }
    }

    */

    IEnumerator IFramePeriod()
    {
        yield return new WaitForSeconds(iFramePeriod);
        canBeHit.Value = true;
    }

    
}
