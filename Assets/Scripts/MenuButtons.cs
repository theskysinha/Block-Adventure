using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    private void Awake(){
        if(!Application.isEditor){
            Debug.unityLogger.logEnabled = false;
        }
    }

    public void StartGame(string name){
        SceneManager.LoadScene(name);
    }

    public void Settings(GameObject settingsPanel){
        if(settingsPanel.activeSelf){
            settingsPanel.SetActive(false);
            settingsPanel.GetComponent<Animator>().Play("settingsPanelReverse");
            return;
        }
        settingsPanel.SetActive(true);
        gameObject.GetComponent<Animator>().Play("Settings");
        settingsPanel.GetComponent<Animator>().Play("settingsPanel");
    }
}