using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject shapeImage;
    public Vector3 selectedShapeScale;

    [HideInInspector]
    public ShapeData currShapeData;
    private List<GameObject> currentShapes = new List<GameObject>();
    private Vector3 initialScale;
    private RectTransform selectedTransform;
    private bool shapeDraggable;
    private Canvas canvas;
    private Vector3 startPos;
    private bool isShapeActive = true;
    public int TotalSquares { get; set; }


    void OnEnable(){
        GameEvents.MoveShapeToStorage += MoveShapeToStorage;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    void OnDisable(){
        GameEvents.MoveShapeToStorage -= MoveShapeToStorage;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public void Awake(){
        selectedTransform = GetComponent<RectTransform>();
        initialScale = selectedTransform.localScale;
        canvas = GetComponentInParent<Canvas>();
        shapeDraggable = true;
        startPos = selectedTransform.localPosition;
        isShapeActive = true;
    }

    public bool IsOnStartPos(){
        return selectedTransform.localPosition == startPos;
    }

    public bool IsAnyOfShapesActive(){
        foreach(var shape in currentShapes){
            if(shape.activeSelf){
                return true;
            }
        }
        return false;
    }

    public void SetShapeInactive(){
        if(!IsOnStartPos() && IsAnyOfShapesActive()){
            foreach(var shape in currentShapes){
                shape.gameObject.SetActive(false);
            }
        }
    }

    public void DeactivateShape(){
        if(isShapeActive){
            foreach(var shape in currentShapes){
                shape?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        isShapeActive = false;
    }

    public void ActivateShape(){
        if(!isShapeActive){
            foreach(var shape in currentShapes){
                shape?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }
        isShapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData){
        transform.localPosition = startPos;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData){
        currShapeData = shapeData;
        TotalSquares = GetTotalSquares(shapeData);
        while(currentShapes.Count <= TotalSquares){
            currentShapes.Add(Instantiate(shapeImage, transform) as GameObject);
        }

        foreach(var shape in currentShapes){
            shape.gameObject.transform.position = Vector3.zero;
            shape.gameObject.SetActive(false);
        }

        var sqRect = shapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(sqRect.rect.width * sqRect.localScale.x, sqRect.rect.height * sqRect.localScale.y);
        int currIndex = 0;

        for(int row = 0; row < shapeData.rows; row++){
            for(int column = 0; column < shapeData.columns; column++){
                if(shapeData.rowsList[row].column[column]){
                    currentShapes[currIndex].SetActive(true);
                    currentShapes[currIndex].GetComponent<RectTransform>().localPosition = new Vector2(GetXPose(shapeData, column, moveDistance), GetYPose(shapeData, row, moveDistance));
                    currIndex++;
                }
            }
        }
    }

    public float GetYPose(ShapeData shapeData, int row, Vector2 moveDistance){
        float shift = 0;
        if(shapeData.rows > 1){
            if(shapeData.rows % 2 != 0){
                var middleSqIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;
                if(row < middleSqIndex){
                    shift = moveDistance.y;
                    shift *= multiplier;
                } 
                else if(row > middleSqIndex){
                    shift = -moveDistance.y;
                    shift *= multiplier;
                }
            } 
            else {
                var middleSqIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSqIndex1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 1);
                var multiplier = (shapeData.rows) / 2;

                if(row == middleSqIndex2 || row == middleSqIndex1){
                    if(row == middleSqIndex2){
                        shift = -moveDistance.y / 2;
                    }
                    if(row == middleSqIndex1){
                        shift = moveDistance.y / 2;
                    }
                }
                if(row < middleSqIndex2 && row < middleSqIndex1){
                    shift = moveDistance.y;
                    shift *= multiplier;
                } else if(row > middleSqIndex2 && row > middleSqIndex1){
                    shift = -moveDistance.y;
                    shift *= multiplier;
                }
            }
        }
        return shift;
    }
        
    public float GetXPose(ShapeData shapeData, int column, Vector2 moveDistance){
        float shift = 0;
        if(shapeData.columns > 1){
            if(shapeData.columns % 2 != 0){
                var middleSqIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if(column < middleSqIndex){
                    shift = -moveDistance.x;
                    shift *= multiplier;
                } else if(column > middleSqIndex){
                    shift = moveDistance.x;
                    shift *= multiplier;
                }
            } else {
                var middleSqIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSqIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 1);
                var multiplier = (shapeData.columns) / 2;
                
                if(column == middleSqIndex2 || column == middleSqIndex1){
                    if(column == middleSqIndex2){
                        shift = moveDistance.x / 2;
                    }
                    if(column == middleSqIndex1){
                        shift = -moveDistance.x / 2;
                    }
                }
                if(column < middleSqIndex2 && column < middleSqIndex1){
                    shift = -moveDistance.x;
                    shift *= multiplier;
                } else if(column > middleSqIndex2 && column > middleSqIndex1){
                    shift = moveDistance.x;
                    shift *= multiplier;
                }
            }
        }
        return shift;
    }

    private int GetTotalSquares(ShapeData shapeData){
        int ans = 0;
        foreach(var row in shapeData.rowsList){
            foreach(var active in row.column){
                if(active){
                    ans++;
                }
            }
        }
        return ans;
    }

    public void OnPointerClick(PointerEventData eventData){

    }

    public void OnPointerUp(PointerEventData eventData){

    }

    public void OnBeginDrag(PointerEventData eventData){
        this.GetComponent<RectTransform>().localScale = selectedShapeScale;
    }

    public void OnDrag(PointerEventData eventData){
        selectedTransform.anchorMax = canvas.GetComponent<RectTransform>().rect.max;
        selectedTransform.anchorMin = canvas.GetComponent<RectTransform>().rect.min;
        selectedTransform.pivot = new Vector2(0.5f, 0.5f);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, Camera.main, out pos);
        selectedTransform.localPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData){
        this.GetComponent<RectTransform>().localScale = initialScale;
        GameEvents.CheckIfShapeCanBePlaced();

    }

    public void OnPointerDown(PointerEventData eventData){

    }

    private void MoveShapeToStorage(){
        selectedTransform.transform.localPosition = startPos;
    }
}
