using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTextureSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool updateImage = false;

    private void OnEnable(){
        UpdateColorIfThreshold();
        if(updateImage){
            GameEvents.UpdateSquareTexture += UpdateSquareTexture;
        }
    }

    private void OnDisable(){
        if(updateImage){
            GameEvents.UpdateSquareTexture -= UpdateSquareTexture;
        }
    }

    void UpdateColorIfThreshold(){
        foreach (var squareTexture in squareTextureData.activeTextures)
        {
            if(squareTextureData.currentColor == squareTexture.color){
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }

    void UpdateSquareTexture(Config.SquareColor color){
        foreach (var squareTexture in squareTextureData.activeTextures)
        {
            if(color == squareTexture.color){
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }
}
