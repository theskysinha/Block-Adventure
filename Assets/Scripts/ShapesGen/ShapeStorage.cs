using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeDataList;
    public List<Shape> shapeList;

    void OnEnable(){
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    void OnDisable(){
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    void Start()
    {
        foreach(var shape in shapeList){
            shape.CreateShape(shapeDataList[Random.Range(0, shapeDataList.Count)]);
        }
    }

    public Shape GetCurrShapeSelected(){
        foreach(var s in shapeList){
            if(!s.IsOnStartPos() && s.IsAnyOfShapesActive()){
                return s;
            }
        }
        Debug.LogError("No shape selected");
        return null;
    }

    private void RequestNewShapes(){
        foreach(var shape in shapeList){
            var shapeIndex = Random.Range(0, shapeDataList.Count);
            shape.RequestNewShape(shapeDataList[shapeIndex]);
        }
    }
}
