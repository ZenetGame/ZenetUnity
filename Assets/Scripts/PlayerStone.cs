﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
        theStateManager = GameObject.FindObjectOfType<StateManager>();
        targetPosition = this.transform.position;

    }

    public Tile StartingTile;
    public Tile CurrentTile { get; protected set; }

    public int PlayerId;
    public StoneStorage MyStoneStorage;

    bool scoreMe = false;

    StateManager theStateManager;
    Tile[] moveQueue;
    int moveQueueIndex;

    bool isAnimating = false;

    Vector3 targetPosition;
    Vector3 velocity;
    float smoothTime = 0.25f;
    float smoothTimeVertical = 0.1f;
    float smoothDistance = 0.01f;
    float smoothHeight = 0.5f;

    PlayerStone stoneToBop;

	
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");

        if (isAnimating == false)
        {
            // Nothing for us to do.
            return;
        }
		
        if (Vector3.Distance(
               new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z),
               targetPosition) < smoothDistance)
        {
            // We've reached the target position -- do we still have moves in the queue?



            if( 
                (moveQueue == null || moveQueueIndex == (moveQueue.Length))
                &&
                ((this.transform.position.y-smoothDistance) > targetPosition.y)
            )
            {
                // We are totally out of moves (and too high up), the only thing left to do is drop down.
                this.transform.position = Vector3.SmoothDamp(
                    this.transform.position, 
                    new Vector3(this.transform.position.x, targetPosition.y, this.transform.position.z), 
                    ref velocity, 
                    smoothTimeVertical);

                // Check for bops
                if(stoneToBop != null)
                {
                    stoneToBop.ReturnToStorage();
                    stoneToBop = null;
                }
            }
            else
            {
                // Right position, right height -- let's advance the queue
                AdvanceMoveQueue();
            }
        }
        else if (this.transform.position.y < (smoothHeight - smoothDistance))
        {
            // We want to rise up before we move sideways.
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position, 
                new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z), 
                ref velocity, 
                smoothTimeVertical);
        }
        else
        {
            // Normal movement (sideways)
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position, 
                new Vector3(targetPosition.x, smoothHeight, targetPosition.z), 
                ref velocity, 
                smoothTime);
        }

    }

    void AdvanceMoveQueue()
    {
        Debug.Log("AdvanceMoveQueue");
        Debug.Log($"moveQueueIndex = {moveQueueIndex}");

        if (moveQueue != null && moveQueueIndex < moveQueue.Length)
        {
            Debug.Log("AdvanceMoveQueue1");

            Tile nextTile = moveQueue[moveQueueIndex];
            if (nextTile == null)
            {
                Debug.Log("AdvanceMoveQueue2");

                // We are probably being scored
                // TODO: Move us to the scored pile
                Debug.Log("SCORING TILE!");
                SetNewTargetPosition(this.transform.position + Vector3.right * 10f);
            }
            else
            {
                Debug.Log("AdvanceMoveQueue3");

                SetNewTargetPosition(nextTile.transform.position);
                moveQueueIndex++;
            }
        }
        else
        {
            Debug.Log("AdvanceMoveQueue4");

            // The movement queue is empty, so we are done animating!
            Debug.Log("Done animating!");
            this.isAnimating = false;
            theStateManager.AnimationsPlaying--;

            // Are we on a roll again space?
            if(CurrentTile != null && CurrentTile.IsRollAgain)
            {
                Debug.Log("AdvanceMoveQueue5");

                theStateManager.RollAgain();
            }
        }

    }

    void SetNewTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
        velocity = Vector3.zero;
        isAnimating = true;
    }

    void OnMouseUp()
    {
        // TODO:  Is the mouse over a UI element? In which case, ignore this click.
        Debug.Log("OnMouseUp");

        MoveMe();

    }


    public void MoveMe2()
    {
        int spacesToMove = theStateManager.DiceTotal;
        Debug.Log("spacesToMove:" + spacesToMove);

        moveQueue = GetTilesAhead(spacesToMove);
        Debug.Log("moveQueue:" + moveQueue);

        Tile finalTile = moveQueue[moveQueue.Length - 1];
        Debug.Log("finalTile:" + finalTile);


    }
    public void MoveMe()
    {
        Debug.Log("MoveMe");

        // Is this the correct player?
        if (theStateManager.CurrentPlayerId != PlayerId)
        {
            return;
        }
        Debug.Log("MoveMe2");

        // Have we rolled the dice?
        if (theStateManager.IsDoneRolling == false)
        {
            // We can't move yet.
            return;
        }
        Debug.Log("MoveMe3");

        if (theStateManager.IsDoneClicking == true)
        {
            // We've already done a move!
            return;
        }
        Debug.Log("MoveMe4");

        int spacesToMove = theStateManager.DiceTotal;


        if (spacesToMove == 0)
        {
            return;
        }
        Debug.Log("MoveMe5");

        // Where should we end up?
        moveQueue = GetTilesAhead(spacesToMove);
        Tile finalTile = moveQueue[ moveQueue.Length-1 ];


        // TODO: Check to see if the destination is legal!
        Debug.Log("MoveMe6");

        if (finalTile == null)
        {
            // Hey, we're scoring this stone!
            scoreMe = true;
        }
        else
        {
            if(CanLegallyMoveTo(finalTile) == false)
            {
                // Not allowed!
                finalTile = CurrentTile;
                moveQueue = null;
                return;
            }

            // If there is an enemy tile in our legal space, the we kick it out.
            if(finalTile.PlayerStone != null)
            {
                //finalTile.PlayerStone.ReturnToStorage();
                //stoneToBop = finalTile.PlayerStone;
                //stoneToBop.position=this.position;
                
                // stoneToBop.CurrentTile.PlayerStone = null;
                // stoneToBop.CurrentTile = null;
                return;
            }
        }

        this.transform.SetParent(null); // Become Batman

        // Remove ourselves from our old tile
        if(CurrentTile != null)
        {
            CurrentTile.PlayerStone = null;
        }

        // Even before the animation is done, set our current tile to the new tile
        CurrentTile = finalTile;
        if( finalTile.IsScoringSpace == false )   // "Scoring" tiles are always "empty"
        {
            finalTile.PlayerStone = this;
        }

        moveQueueIndex = 0;

        theStateManager.IsDoneClicking = true;
        this.isAnimating = true;
        theStateManager.AnimationsPlaying++;
    }

    // Return the list of tiles __ moves ahead of us
    public Tile[] GetTilesAhead( int spacesToMove )
    {
        if (spacesToMove == 0)
        {
            return null;
        }

        // Where should we end up?

        Tile[] listOfTiles = new Tile[spacesToMove];
Debug.Log("ListofTiles"+listOfTiles);

        Tile finalTile = CurrentTile;
Debug.Log("finalTile"+finalTile);

        for (int i = 0; i < spacesToMove; i++)
        {
            if (finalTile == null)
            {
                finalTile = StartingTile;
            }
            else
            {
                if (finalTile.NextTiles == null || finalTile.NextTiles.Length == 0)
                {
                    // We are overshooting the victory -- so just return some nulls in the array
                    // Just break and we'll return the array, which is going to have nulls
                    // at the end.
                    break;
                }
                else if (finalTile.NextTiles.Length > 1)
                {
                    Debug.Log("moveme_GetTilesAhead: " + finalTile.NextTiles.Length.ToString());

                    // Branch based on player id
                    finalTile = finalTile.NextTiles[0];
                }
                else
                {
                    finalTile = finalTile.NextTiles[0];
                }
            }

            listOfTiles[i] = finalTile;
        }

        return listOfTiles;
    }

    public Tile GetTileAhead(  )
    {
        return GetTileAhead( theStateManager.DiceTotal );
    }


    // Return the final tile we'd land on if we moved __ spaces
    public Tile GetTileAhead( int spacesToMove )
    {
        //Debug.Log(spacesToMove);
        Tile[] tiles = GetTilesAhead( spacesToMove );

        if(tiles == null)
        {
            // We aren't moving at all, so just return our current tile?
            return CurrentTile;
        }

        return tiles[ tiles.Length-1 ];
    }

    public bool CanLegallyMoveAhead( int spacesToMove )
    {
        if( CurrentTile != null && CurrentTile.IsScoringSpace )
        {
            // This stone is already on a scoring tile, so we can't move.
            return false;
        }

        Tile theTile = GetTileAhead( spacesToMove );

        return CanLegallyMoveTo( theTile );
    }

    bool CanLegallyMoveTo( Tile destinationTile )
    {
        //Debug.Log("CanLegallyMoveTo: " + destinationTile);

        if(destinationTile == null)
        {
            // NOTE!  A null tile means we are overshooting the victory roll
            // and this is NOT legal (apparently) in the Royal Game of Ur
            return false;


            // We're trying to move off the board and score, which is legal
            //Debug.Log("We're trying to move off the board and score, which is legal");
            //return true;
        }

        // Is the tile empty?
        if(destinationTile.PlayerStone == null)
        {
            return true;
        }

        // Is it one of our stones?
        if(destinationTile.PlayerStone.PlayerId == this.PlayerId)
        {
            // We can't land on our own stone.
            return false;
        }

        // If it's an enemy stone, is it in a safe square?
        if( destinationTile.IsRollAgain == true )
        {
            // Can't bop someone on a safe tile!
            return false;
        }

        // If we've gotten here, it means we can legally land on the enemy stone and
        // kick it off the board.
        return true;
    }

    public void ReturnToStorage()
    {
        Debug.Log("ReturnToStorage");
        //currentTile.PlayerStone = null;
        //currentTile = null;

        this.isAnimating=true;
        theStateManager.AnimationsPlaying++;

        moveQueue = null;

        // Save our current position
        Vector3 savePosition = this.transform.position;

        MyStoneStorage.AddStoneToStorage( this.gameObject );

        // Set our new position to the animation target
        SetNewTargetPosition(this.transform.position);

        // Restore our saved position
        this.transform.position = savePosition;
    }

}
