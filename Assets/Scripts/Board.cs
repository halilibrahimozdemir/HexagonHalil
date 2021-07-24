using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    private static Board instance;
    public bool rotating = false;
    public bool constructing=false;
    public static Board MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Board>();
            }

            return instance;
        }
    }

    //x ve y koordinatlarý için altýgenin uzunluk deðerleri
    public float xOffSet = 1.05f;
    public float yOffSet = 1.21f;
    
 
    //satýr ve sütun deðerleri
    public int width;
    public int height;
    //hexagon prefabýnýn objesi tanýmlanýyor
    public GameObject hexagonPrefab;
    //background hexagonlarýmýzýn üstündeki renkli hexagonlarýn obje dizisi
    public GameObject[] colorfulHexagons;

    public GameObject[,] hexagons;
    public GameObject[,] hexagonHolders;

    // Start is called before the first frame update
    void Start()
    {
        hexagons = new GameObject[width, height];
        hexagonHolders = new GameObject[width, height];
        Setup();
    }

    private void Update()
    {
        for (int i = 0; i < width; i++) //top fill
        {
            if (hexagons[i, height - 1] == null)
            {
                Vector2 tempPosition = hexagonHolders[i, height-1].transform.position + new Vector3(0, 2, 0);
                int hexagonToUse = Random.Range(0, colorfulHexagons.Length);
                GameObject newHex = Instantiate(colorfulHexagons[hexagonToUse], tempPosition, Quaternion.identity);
                newHex.GetComponent<Hexagon>().x = i;
                newHex.GetComponent<Hexagon>().y = height-1;
                hexagons[i, height-1] = newHex;
                newHex.GetComponent<Hexagon>().movingTopDestination = hexagonHolders[i, height - 1].transform.position;
                newHex.GetComponent<Hexagon>().movingTop = true;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckConstructing();
    }

    private void CheckConstructing()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (hexagons[i, j] != null)
                {
                    if (hexagons[i, j].GetComponent<Hexagon>().movingDown ||
                        hexagons[i, j].GetComponent<Hexagon>().movingTop)
                    {
                        constructing = true;
                        return;
                    }
                }
            }
        }
        constructing = false;
    }

    //verilen satýr ve sütun sayýsýna göre hexagonlarý ekrana yerleþtiren fonksiyon
    private void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float yPos = j * yOffSet;


                if (i % 2 == 1)
                {
                    yPos += yOffSet / 2f;
                }

                //Background Hexagonlar
                Vector2 tempPosition0 = new Vector2((float)(i * xOffSet), (float)(yPos))+(Vector2)transform.position; // nereden başlayacağını Board Objesinin yeri ile belirliyoruz.
                GameObject hexagon = Instantiate(hexagonPrefab, tempPosition0, Quaternion.identity) as GameObject;
                hexagon.transform.parent = this.transform;
                hexagon.name = "(" + i + "," + j + ")";
                hexagonHolders[i, j] = hexagon;



                //Renkli Hexagonlar
                Vector2 tempPosition = new Vector2((float)(i * xOffSet), (float)(yPos))+(Vector2)transform.position; // nereden başlayacağını Board Objesinin yeri ile belirliyoruz.
                int hexagonToUse = Random.Range(0, colorfulHexagons.Length);
                GameObject colorfulHexagon = Instantiate(colorfulHexagons[hexagonToUse], tempPosition, Quaternion.identity);
                colorfulHexagon.transform.parent = this.transform;
                colorfulHexagon.name = i + "," + j;
                hexagons[i, j] = colorfulHexagon;
                colorfulHexagon.GetComponent<Hexagon>().x = i;
                colorfulHexagon.GetComponent<Hexagon>().y = j;
            }
        }
    }
}
