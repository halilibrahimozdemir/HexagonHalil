using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public int score;
    public Text scoreText;
    public Text movesText;
    public int moves;
    public float timeSinceLastMove;
    public int scoreChange;
    public bool needBomb;
    public bool calledOnce;
    public RectTransform bombCountObj;
    public Text bombText;
    public Hexagon bombObj;
    public bool gameOver=false;
    public GameObject gameOverScreen;
    public static GameManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    private void Update()
    {
        if (gameOver)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            gameOverScreen.SetActive(false);
        }
        timeSinceLastMove += Time.deltaTime;
        scoreText.text = score.ToString();
        movesText.text = moves.ToString();
        if (scoreChange >=990)
        {
            needBomb = true;
        }
        else
        {
            needBomb = false;
        }
        scoreChange = score % 1000;
        if (bombObj != null)
        {
            bombCountObj.gameObject.SetActive(true);
            Vector3 viewportPoint=Camera.main.WorldToViewportPoint(bombObj.transform.position);
            bombCountObj.anchorMin = viewportPoint;
            bombCountObj.anchorMax = viewportPoint;
        }
        else
        {
            bombCountObj.gameObject.SetActive(false);
        }
    }
}
