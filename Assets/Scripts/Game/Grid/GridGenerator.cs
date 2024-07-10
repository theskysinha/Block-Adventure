using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float gaps = 0.1f;
    public GameObject squarePrefab;
    public Vector2 origin = new Vector2(0.0f, 0.0f);
    public float squareSize = 0.5f;
    public float gridOffset = 0.0f;
    public AudioSource comboSound;
    public AudioSource squarePlacedSound;
    public SquareTextureData squareTextureData;
    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _grid = new List<GameObject>();
    private LineIndicator _lineIndicator;
    private Config.SquareColor _currentColor = Config.SquareColor.NotSet;
    private List<Config.SquareColor> _colorsInGrid = new List<Config.SquareColor>();

    private void OnEnable() {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareTexture += OnUpdateSquareColor;
    }

    private void OnDisable() {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareTexture -= OnUpdateSquareColor;
    }


    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        _currentColor = squareTextureData.activeTextures[0].color;
    }

    private void OnUpdateSquareColor(Config.SquareColor color){
        _currentColor = color;
    }

    private List<Config.SquareColor> GetColorsInGrid(){
        List<Config.SquareColor> colors = new List<Config.SquareColor>();
        foreach(var square in _grid){
            var gridSq = square.GetComponent<GridSquare>();
            if(gridSq.IsOccupied){
                var col = gridSq.GetCurrentColor();
                if(!colors.Contains(col)){
                    colors.Add(col);
                }
            }
        }
        return colors;
    }

    private void CreateGrid(){
        SpawnGrid();
        PositionGrid();
    }

    private void SpawnGrid(){
        int index = 0;
        for (int i = 0; i < rows; i++){
            for (int j = 0; j < columns; j++){
                _grid.Add(Instantiate(squarePrefab) as GameObject);
                _grid[_grid.Count - 1].GetComponent<GridSquare>().SquareIndex = index;
                _grid[_grid.Count - 1].transform.SetParent(this.transform);
                _grid[_grid.Count - 1].transform.localScale = new Vector3(squareSize, squareSize, squareSize);
                _grid[_grid.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSq(index) % 2 == 0);
                index++;
            }
        }
    }

    private void PositionGrid(){
        int colNumber = 0;
        int rowNumber = 0;
        Vector2 sqGap = new Vector2(0 , 0);
        bool rowMoved = false;
        RectTransform rt = _grid[0].GetComponent<RectTransform>();

        _offset.x = rt.rect.width * rt.transform.localScale.x + gridOffset;
        _offset.y = rt.rect.height * rt.transform.localScale.y + gridOffset;

        foreach(GameObject square in _grid){
            if(colNumber + 1 > columns){
                sqGap.x = 0;
                colNumber = 0;
                rowNumber++;
                rowMoved = false;
            }

            var postionXOffset = _offset.x * colNumber + (sqGap.x * gaps);
            var postionYOffset = _offset.y * rowNumber + (sqGap.y * gaps);

            if(colNumber > 0 && colNumber % 3 == 0){
                sqGap.x++;
                postionXOffset += gaps;
            }

            if(rowNumber > 0 && rowNumber % 3 == 0 && !rowMoved){
                rowMoved = true;
                sqGap.y++;
                postionYOffset += gaps;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(origin.x + postionXOffset, origin.y - postionYOffset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(origin.x + postionXOffset, origin.y - postionYOffset, 0);
            colNumber++;
        }
    }

    private void CheckIfShapeCanBePlaced(){
        var sqIndexes = new List<int>();
        foreach(GameObject square in _grid){
            var gridSq = square.GetComponent<GridSquare>();
            if(gridSq.Selected && !gridSq.IsOccupied){
                sqIndexes.Add(gridSq.SquareIndex);
                gridSq.Selected = false;   
            }
        }
        var currShape = shapeStorage.GetCurrShapeSelected();
        if(currShape == null){
            return;
        }

        if(currShape.TotalSquares == sqIndexes.Count){
            foreach(var sqIndex in sqIndexes){
                _grid[sqIndex].GetComponent<GridSquare>().PlaceShape(_currentColor);
            }
            squarePlacedSound.Play();
            
            var shapeLeft = 0;
            foreach(var shape in shapeStorage.shapeList){
                if(shape.IsOnStartPos() && shape.IsAnyOfShapesActive()){
                    shapeLeft++;
                }
            }
            
            if(shapeLeft == 0){
                GameEvents.RequestNewShapes();
            } else{
                GameEvents.SetShapeInactive();
            }
            CheckIfCompletedLine();
        }
        else{
            GameEvents.MoveShapeToStorage();
        }
    }

    void CheckIfCompletedLine(){
        List<int[]> lines = new List<int[]>();
        foreach(var cols in _lineIndicator.columnIndexes){
            lines.Add(_lineIndicator.GetVerticalLine(cols));
        }
        for(var row = 0; row < 9; row++){
            List<int> data = new List<int>(9);
            for(var index = 0; index < 9; index++){
                data.Add(_lineIndicator.lineData[row, index]);
            }
            lines.Add(data.ToArray());
        }

        //This function needs to be called before checkifsquaresarecompleted
        _colorsInGrid = GetColorsInGrid();

// // might remove as this adds the logic for squares completed
//         for(var square = 0; square < 9; square++){
//             var data = new List<int>(9);
//             for(var index = 0; index < 9; index++){
//                 data.Add(_lineIndicator.sqData[square, index]);
//             }
//             lines.Add(data.ToArray());
//         }
//     //till here

        var completedLines = CheckIfCompletedSquare(lines);

        if(completedLines >= 2){
            GameEvents.Combo();
            squarePlacedSound.Play();
        }
        var totalScore = 10 * completedLines;
        var bonusScore = ShouldPlayBonusAnimation();
        totalScore += bonusScore;
        GameEvents.AddScores(totalScore);
        CheckIfPlayerLost();
    }

    private int ShouldPlayBonusAnimation(){
        var colors = GetColorsInGrid();
        Config.SquareColor colorOfBonus = Config.SquareColor.NotSet;
        foreach(var color in _colorsInGrid){
            if(!colors.Contains(color)){
                colorOfBonus = color;
            }
        }
        if(colorOfBonus == Config.SquareColor.NotSet){
            Debug.Log("No color found for bonus");
            return 0;
        }
        if(colorOfBonus == _currentColor){
            return 0;
        }

        GameEvents.Bonus(colorOfBonus);
        return 50;
    }

    private int CheckIfCompletedSquare(List<int[]> data){
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;
        foreach(var line in data){
            var lineCompleted = true;
            foreach(var sqIndex in line){
                var comp = _grid[sqIndex].GetComponent<GridSquare>();
                if(!comp.IsOccupied){
                    lineCompleted = false;
                }
            }
            if(lineCompleted){
                completedLines.Add(line);
            }
        }
        foreach(var line in completedLines){
            var completed = false;
            foreach(var sqIndex in line){
                var comp = _grid[sqIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach(var sqIndex in line){
                var comp = _grid[sqIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if(completed){
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckIfPlayerLost(){
        var validShapes = 0;
        for(var index = 0; index < shapeStorage.shapeList.Count; index++){
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapesActive();
            if(CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]) && isShapeActive){
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }
        }
        // if no valid shapes are left and player's score < bestscore, the player has lost
        if(validShapes == 0){
            GameEvents.GameOver(false);
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currShape){
        var currShapeData = currShape.currShapeData;
        var shapeColumns = currShapeData.columns;
        var shapeRows = currShapeData.rows;
        List<int> originalShapeFilledUpSquares = new List<int>();
        var shapeIndex = 0;
        for(var row = 0; row < shapeRows; row++){
            for(var col = 0; col < shapeColumns; col++){
                if(currShapeData.rowsList[row].column[col]){
                    originalShapeFilledUpSquares.Add(shapeIndex);
                }
                shapeIndex++;
            }
        }
        if(currShape.TotalSquares != originalShapeFilledUpSquares.Count){
            Debug.LogError("Shape not placed correctly");
        }

        var squareList = GetAllSquareCombos(shapeColumns, shapeRows);

        bool canBePlaced = false;
        foreach(var number in squareList){
            bool shapeCanBePlanedOnGrid = true;
            foreach(var sqIndexToCheck in originalShapeFilledUpSquares){
                var comp = _grid[number[sqIndexToCheck]].GetComponent<GridSquare>();
                if(comp.IsOccupied){
                    shapeCanBePlanedOnGrid = false;
                }
            }
            if(shapeCanBePlanedOnGrid){
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }

    private List<int[]> GetAllSquareCombos(int columns, int rows){
        List<int[]> squareList = new List<int[]>();
        var lastColumn = 0;
        var lastRow = 0;

        int safeIndex = 0;

        while(lastRow + (rows - 1) < 9){
            var rowData = new List<int>();
            for(var row = lastRow; row < lastRow + rows; row++){
                for(var col = lastColumn; col < lastColumn + columns; col++){
                    rowData.Add(_lineIndicator.lineData[row, col]);
                }
            }
            squareList.Add(rowData.ToArray());
            lastColumn++;
            if(lastColumn + (columns - 1) >= 9){
                lastColumn = 0;
                lastRow++;
            }
            safeIndex++;
            if(safeIndex > 100){
                break;
            }
        }
        return squareList;
    }
}
