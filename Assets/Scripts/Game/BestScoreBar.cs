using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestScoreBar : MonoBehaviour
{
    public Image fillInImage;
    public TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        GameEvents.UpdateBestScore += UpdateBestScore;
    }

    private void onDisable()
    {
        GameEvents.UpdateBestScore -= UpdateBestScore;
    }

    private void UpdateBestScore(int currentScore, int bestScore)
    {
        fillInImage.fillAmount = (float)currentScore / (float) bestScore;
        scoreText.text = bestScore.ToString();
    }
}
