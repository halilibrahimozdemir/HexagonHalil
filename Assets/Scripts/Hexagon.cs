using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        neighbours = new Hexagon[6];
        color = GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
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
        
        // else
        // {
        //     for (int i = 0; i < 6; i++)
        //     {
        //         neighbours[i] = null;
        //     }
        // }
        
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

        /*if (neighbours[5] != null && neighbours[0] != null)  // unnecessary control
        {
            if (neighbours[5].color == color)
            {
                if (neighbours[0].color == color)
                {
                    Destroy(gameObject);
                    board.hexagons[x, y] = null;
                    Destroy(neighbours[5].gameObject);
                    board.hexagons[neighbours[5].x, neighbours[5].y] = null;
                    Destroy(neighbours[0].gameObject);
                    board.hexagons[neighbours[0].x, neighbours[0].y] = null;
                }
            }
        }*/ 
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