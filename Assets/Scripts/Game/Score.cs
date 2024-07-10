using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BestScoreData{
    public int bestScore = 0;
}

public class Score : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    private bool _newBestScore = false;
    public BestScoreData bestScoreData = new BestScoreData();
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreTextGameOver;
    private int currentScore;
    private string bestScoreKey = "bsdat";

    void OnEnable(){
        GameEvents.AddScores += AddScore;
        GameEvents.GameOver += SaveBestScore;
    }

    void OnDisable(){
        GameEvents.AddScores -= AddScore;
        GameEvents.GameOver -= SaveBestScore;
    }

    public void SaveBestScore(bool newBestScore){
        BinaryDataStream.Save<BestScoreData>(bestScoreData, bestScoreKey);
    }

    void Awake(){
        if(BinaryDataStream.Exists(bestScoreKey)){
            StartCoroutine(LoadBestScore());
        }
    }

    IEnumerator LoadBestScore(){
        bestScoreData = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScore(currentScore, bestScoreData.bestScore);
    }

    void Start()
    {
        _newBestScore = false;
        currentScore = 0;
        squareTextureData.SetStartColor();
        UpdateScore();
    }

    void AddScore(int score){
        currentScore += score;
        if(currentScore > bestScoreData.bestScore){
            bestScoreData.bestScore = currentScore;
            _newBestScore = true;
            SaveBestScore(true);
        }
        UpdateScoreColor();
        GameEvents.UpdateBestScore(currentScore, bestScoreData.bestScore);
        UpdateScore();
    }

    private void UpdateScoreColor(){
        if(GameEvents.UpdateSquareTexture != null && currentScore >= squareTextureData.threshold){
            squareTextureData.UpdateColor(currentScore);
            GameEvents.UpdateSquareTexture(squareTextureData.currentColor);
        }
    }
    
    private void UpdateScore(){
        scoreText.text = currentScore.ToString();
        scoreTextGameOver.text = currentScore.ToString();
    }
}
