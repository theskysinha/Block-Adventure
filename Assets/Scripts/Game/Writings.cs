using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writings : MonoBehaviour
{
    public List<GameObject> writings;
    public AudioSource writingsSound;
    void Start(){
        GameEvents.Combo += ShowComboText;
    }

    void OnDisable(){
        GameEvents.Combo -= ShowComboText;
    }

    private void ShowComboText(){
        var index = Random.Range(0, writings.Count);
        writings[index].SetActive(true);
        
    }
}
