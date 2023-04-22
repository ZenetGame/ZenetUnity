using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameIntro : MonoBehaviour
{

    public Button player1Button;
    public Button player2Button;
    StateManager theStateManager;
    Animator anim;
    GameObject mainCamera;




    void Start()
    {
        Debug.Log("GAME_START");

        player1Button.onClick.AddListener(GamePlay);
        player2Button.onClick.AddListener(GamePlay);
        theStateManager = GameObject.FindObjectOfType<StateManager>();

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");



        anim = mainCamera.GetComponent<Animator>();
    }

    public void GamePlay()
    {


            anim.SetTrigger("Active");
    }






}
