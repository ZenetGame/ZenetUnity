﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class StateManager : MonoBehaviour
{



    // Use this for initialization
    void Start()
    {
        PlayerAIs = new AIPlayer[NumberOfPlayers];

        PlayerAIs[0] = null;    // Is a human player
        //PlayerAIs[0] = new AIPlayer_UtilityAI();
        //PlayerAIs[1] = new AIPlayer_UtilityAI();
        PlayerAIs[1] = null;
    }

    public int NumberOfPlayers = 2;
    public int CurrentPlayerId = 0;

    AIPlayer[] PlayerAIs;

    public int DiceTotal;

    // NOTE: enum / statemachine is probably a stronger choice, but I'm aiming for simpler to explain.
    public bool IsDoneRolling = false;
    public int extraTurns = 0;
    public bool IsDoneClicking = false;
    //public bool IsDoneAnimating = false;
    public int AnimationsPlaying = 0;

    public GameObject NoLegalMovesPopup;

    public void NewTurn()
    {
        Debug.Log("NewTurn");
        // This is the start of a player's turn.
        // We don't have a roll for them yet.
        IsDoneRolling = false;
        IsDoneClicking = false;
        //IsDoneAnimating = false;

        CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
    }

    public void RollAgain()
    {
        Debug.Log("RollAgain");
        IsDoneRolling = false;
        IsDoneClicking = false;
        //IsDoneAnimating = false;
    }

    // Update is called once per frame
    void Update()
    {
		
        // Is the turn done?
        if (IsDoneRolling && IsDoneClicking && AnimationsPlaying==0)
        {
            Debug.Log("Turn is done!");
            NewTurn();
            return;
        }

        if( PlayerAIs[CurrentPlayerId] != null )
        {
            PlayerAIs[CurrentPlayerId].DoAI();
        }
            
    }

    public void CheckLegalMoves()
    {





        // If we rolled a zero, then we clearly have no legal moves.
        if(DiceTotal == 0)
        {
            StartCoroutine( NoLegalMoveCoroutine() );
            return;
        }

        // Loop through all of a player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        bool hasLegalMove = false;
        foreach( PlayerStone ps in pss )
        {
            if(ps.PlayerId == CurrentPlayerId)
            {
                if( ps.CanLegallyMoveAhead( DiceTotal) )
                {
                    // TODO: Highlight stones that can be legally moved
                    hasLegalMove = true;
                }
            }
        }

        // If no legal moves are possible, wait a sec then move to next player (probably give message)
        if(hasLegalMove == false)
        {
            StartCoroutine( NoLegalMoveCoroutine() );
            return;
        }

    }

    IEnumerator NoLegalMoveCoroutine() 
    {
        // Display message
        NoLegalMovesPopup.SetActive(true);

        // TODO: Trigger animations like have the stones shake or something?

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        NoLegalMovesPopup.SetActive(false);

        NewTurn();
    }
}
