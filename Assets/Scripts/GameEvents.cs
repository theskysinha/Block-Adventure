using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static Action<bool> GameOver;
    public static Action<int> AddScores;
    public static Action CheckIfShapeCanBePlaced;

    public static Action MoveShapeToStorage;

    public static Action RequestNewShapes;

    public static Action SetShapeInactive;
    
    public static Action<int, int> UpdateBestScore;
    public static Action<Config.SquareColor> UpdateSquareTexture;
    public static Action Combo;
    public static Action<Config.SquareColor> Bonus;
}
