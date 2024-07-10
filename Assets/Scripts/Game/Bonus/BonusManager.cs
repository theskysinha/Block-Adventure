using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonuses;
    public AudioSource bonusSound;

    void Start(){
        GameEvents.Bonus += ShowBonus;
    }

    void OnDisable(){
        GameEvents.Bonus -= ShowBonus;
    }

    private void ShowBonus(Config.SquareColor color){
        foreach (var bonus in bonuses)
        {
            if(bonus.GetComponent<Bonus>().color == color){
                bonus.SetActive(true);
                StartCoroutine(DeactivateBonus(bonus));
                bonusSound.Play();
                return;
            }
        }
    }

    private IEnumerator DeactivateBonus(GameObject bonus){
        yield return new WaitForSeconds(2f);
        bonus.SetActive(false);
    }
}
