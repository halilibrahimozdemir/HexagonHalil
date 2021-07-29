using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random =UnityEngine.Random;

public class Hexagon : MonoBehaviour
{
    public bool lerp = false;
    public Vector2 lerpPosition;
    public float rotateLerpSpeed = 5f;
    public float downLerpSpeed = 20f;
    public float rotateThreshold = 0.01f;
    public Color color;
    public int x;
    public int y;
    public Hexagon[] neighbours;

    private Hexagon upNeighbour;
    private Hexagon upRightNeighbour;
    private Hexagon upLeftNeighbour;
    private Hexagon downNeighbour;
    private Hexagon downRightNeighbour;
    private Hexagon downLeftNeighbour;
    public bool movingDown = false;
    public bool movingTop = false;
    public Vector2 movingTopDestination;
    public bool amIBomb = false;
    public int bombCount;
    public int lastMoveCount;
    public bool checkedOnce = false;
    private void Start()
    {
        lastMoveCount = GameManager.MyInstance.moves;
        neighbours = new Hexagon[6];
        color = GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        if (amIBomb && !checkedOnce)
        {
            GameManager.MyInstance.bombObj = this;
            bombCount = Random.Range(7, 15);
            checkedOnce = true;
        }
        if (lerp)
        {
            Board.MyInstance.rotating = true;
            float newX = Mathf.Lerp(transform.position.x, lerpPosition.x, Time.deltaTime * rotateLerpSpeed);
            float newY = Mathf.Lerp(transform.position.y, lerpPosition.y, Time.deltaTime * rotateLerpSpeed);
            transform.position = new Vector2(newX, newY);


            if (Vector3.Distance(transform.position, lerpPosition) < rotateThreshold)
            {
                transform.position = lerpPosition;
                lerp = false;
                Board.MyInstance.rotating = false;
            }
        }
        
        if (!Board.MyInstance.rotating && !Board.MyInstance.constructing)
        {
            FindNeighbours();
            CheckNeighbours();
        }
        
        if (!movingTop && y > 0)
        {
            if (Board.MyInstance.hexagons[x, y - 1] == null)
            {
                movingDown = true;
            }
        }

        if (movingDown)
        {
            MoveLerp(this, Board.MyInstance.hexagonHolders[x, y - 1].transform.position);
        }

        if (movingTop)
        {
            MoveToTop(this,movingTopDestination);
        }

        if (amIBomb)
        {
            if (bombCount == 0)
            {
                GameManager.MyInstance.gameOver = true;
            }
            if (GameManager.MyInstance.moves > lastMoveCount)
            {
                bombCount--;
            }
            GameManager.MyInstance.bombText.text = bombCount.ToString();
            lastMoveCount = GameManager.MyInstance.moves; 
        }
    }
    


    /* Function to save rotate changes */
    public void Rotate(int newX, int newY, Vector2 newPos)
    {
        x = newX;
        y = newY;
        lerpPosition = newPos;
        lerp = true;
    }

    private void MoveLerp(Hexagon hex, Vector2 destination)
    {
        float newX = Mathf.Lerp(hex.transform.position.x, destination.x, Time.deltaTime * downLerpSpeed);
        float newY = Mathf.Lerp(hex.transform.position.y, destination.y, Time.deltaTime * downLerpSpeed);
        hex.transform.position = new Vector2(newX, newY);


        if (Vector3.Distance(hex.transform.position, destination) < rotateThreshold)
        {
            hex.transform.position = destination;
            Board.MyInstance.hexagons[x, y] = null;
            Board.MyInstance.hexagons[x, y - 1] = gameObject;
            y -= 1;
            movingDown = false;
        }
    }

    public void MoveToTop(Hexagon hex, Vector2 destination)
    {
        float newX = Mathf.Lerp(hex.transform.position.x, destination.x, Time.deltaTime * downLerpSpeed);
        float newY = Mathf.Lerp(hex.transform.position.y, destination.y, Time.deltaTime * downLerpSpeed);
        hex.transform.position = new Vector2(newX, newY);


        if (Vector3.Distance(hex.transform.position, destination) < rotateThreshold)
        {
            hex.transform.position = destination;
            movingTop = false;
            GameManager.MyInstance.score += 5;
        }
    }

    public void CheckNeighbours()
    {
        for (int i = 0; i < 5; i++)
        {
            if (neighbours[i] != null && neighbours[i + 1] != null)
            {
                if (neighbours[i].color == color)
                {
                    if (neighbours[i + 1].color == color)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (Controls.MyInstance._selected3Hexagons[0] != null)
                            {
                                if (neighbours[i] ==
                                    Controls.MyInstance._selected3Hexagons[j].GetComponent<Hexagon>() ||
                                    neighbours[i+1] == Controls.MyInstance._selected3Hexagons[j].GetComponent<Hexagon>() ||
                                    this == Controls.MyInstance._selected3Hexagons[j].GetComponent<Hexagon>())
                                {
                                    if (GameManager.MyInstance.timeSinceLastMove>0.5f)
                                    {
                                        GameManager.MyInstance.moves++;
                                        GameManager.MyInstance.timeSinceLastMove = 0f;
                                    }
                                } 
                            }
                        }
                        Destroy(gameObject);
                        Board.MyInstance.hexagons[x, y] = null;
                        Destroy(neighbours[i].gameObject);
                        Board.MyInstance.hexagons[neighbours[i].x, neighbours[i].y] = null;
                        Destroy(neighbours[i + 1].gameObject);
                        Board.MyInstance.hexagons[neighbours[i + 1].x, neighbours[i + 1].y] = null;
                        Board.MyInstance.exploded=true;
                    }
                }
            }
        }
    }

    private void FindNeighbours()
    {
        var direction = Vector2.up;
        GetComponent<CircleCollider2D>().enabled = false;
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0, 0, -60) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f);
            if (hit.collider != null)
            {
                if (Mathf.Abs(hit.collider.GetComponent<Hexagon>().y - y) <= 1 ||
                    Mathf.Abs(hit.collider.GetComponent<Hexagon>().x - x) <= 1)
                {
                    neighbours[i] = hit.collider.GetComponent<Hexagon>();
                }
            }
            else
            {
                neighbours[i] = null;
            }
        }
        GetComponent<CircleCollider2D>().enabled = true;
    }
}