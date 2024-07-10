using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquareTextureData", menuName = "SquareTextureData")]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData{
        public Sprite texture;
        public Config.SquareColor color;
    }

    public int threshold = 100;
    private const int startThreshold = 10;
    public List<TextureData> activeTextures;
    public Config.SquareColor currentColor;
    public Config.SquareColor nextColor;

    public int GetCurrentColorIndex(){
        var currentColor = 0;
        for (int i = 0; i < activeTextures.Count; i++)
        {
            if(activeTextures[i].color == this.currentColor){
                currentColor = i;
            }
        }
        return currentColor;
    }
    
    public void UpdateColor(int currentScore){
        currentColor = nextColor;
        var currentColorIndex = GetCurrentColorIndex();

        if(currentColorIndex == activeTextures.Count - 1){
            nextColor = activeTextures[0].color;
        } else{
            nextColor = activeTextures[currentColorIndex + 1].color;
        }
        threshold = startThreshold + currentScore;
    }

    public void SetStartColor(){
        threshold = startThreshold;
        currentColor = activeTextures[0].color;
        nextColor = activeTextures[1].color;
    }

    private void Awake() {
        SetStartColor();
    }

    private void OnEnable() {
        SetStartColor();
    }
}
