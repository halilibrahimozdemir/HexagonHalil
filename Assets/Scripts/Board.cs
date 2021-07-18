using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
   private static Board instance;
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
    public float xOffSet = 0.78f;
    public float yOffSet = 0.88f;
    
 
    //satýr ve sütun deðerleri
    public int width;
    public int height;
    //hexagon prefabýnýn objesi tanýmlanýyor
    public GameObject hexagonPrefab;
    //background hexagonlarýmýzýn üstündeki renkli hexagonlarýn obje dizisi
    public GameObject[] colorfulHexagons;

    public GameObject[,] allColorfulHexagons;

    // Start is called before the first frame update
    void Start()
    {
        allColorfulHexagons = new GameObject[width, height];
        Setup();
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
                Vector2 tempPosition = new Vector2((float)(i * xOffSet), (float)(yPos))+(Vector2)transform.position; // nereden başlayacağını Board Objesinin yeri ile belirliyoruz.
                GameObject hexagons = Instantiate(hexagonPrefab, tempPosition, Quaternion.identity) as GameObject;
                hexagons.transform.parent = this.transform;
                hexagons.name = "(" + i + "," + j + ")";


                //Renkli Hexagonlar
                int hexagonToUse = Random.Range(0, colorfulHexagons.Length);
                GameObject colorfulHexagon = Instantiate(colorfulHexagons[hexagonToUse], tempPosition, Quaternion.identity);
                colorfulHexagon.transform.parent = this.transform;
                colorfulHexagon.name = i + "," + j;
                allColorfulHexagons[i, j] = colorfulHexagon;
            }
        }
    }
}
