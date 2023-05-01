using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class GameIntro : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectWallet ();


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

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    ConnectWallet();
#endif

            anim.SetTrigger("Active");


    }






}
