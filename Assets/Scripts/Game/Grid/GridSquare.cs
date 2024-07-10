using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image squareImage;
    public Image hoverImage;
    public Image activeImage;
    public List<Sprite> squareSprites;
    private Config.SquareColor currentSquareColor = Config.SquareColor.NotSet;
    public Config.SquareColor GetCurrentColor()
    {
        return currentSquareColor;
    }

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool IsOccupied { get; set; }

    public void Awake()
    {
        Selected = false;
        IsOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        squareImage.GetComponent<Image>().sprite = setFirstImage ? squareSprites[1] : squareSprites[0];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsOccupied){
            Selected = true;
            hoverImage.gameObject.SetActive(true);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        Selected = true;
        if(!IsOccupied) hoverImage.gameObject.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(!IsOccupied){
            Selected = false;
            hoverImage.gameObject.SetActive(false);
        }
    }

    public void ActivateImage()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        IsOccupied = true;
    }

    public void Deactivate(){
        currentSquareColor = Config.SquareColor.NotSet;
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied(){
        currentSquareColor = Config.SquareColor.NotSet;
        IsOccupied = false;
        Selected = false;
    }

    public void PlaceShape(Config.SquareColor color)
    {
        currentSquareColor = color;
        ActivateImage();
    }

}
