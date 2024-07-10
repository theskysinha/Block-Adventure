using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject gameOverPopUp;
    public GameObject losePopUp;
    public GameObject newBestScorePopUp;

    void OnEnable(){
        GameEvents.GameOver += onGameOver;
    }

    void OnDisable(){
        GameEvents.GameOver -= onGameOver;
    }

    void Start(){
        gameOverPopUp.SetActive(false);
    }

    void onGameOver(bool newBestScore){
        gameOverPopUp.SetActive(true);
        if(!newBestScore){
            losePopUp.SetActive(true);
        }else{
            newBestScorePopUp.SetActive(true);
        }
    }
}
