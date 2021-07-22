using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Controls : MonoBehaviour
{
    public float radius = 5f;
    [SerializeField] private Camera mainCamera;

    private Transform[] _nearest3Hexagons=new Transform[3];
    private Transform[] _selected3Hexagons=new Transform[3];

    private Vector2 _touchStartPosition;
    private Vector2 _touchFinishPosition;

    public GameObject selectedHexagonsGroup;

    private void Start()
    {
        _selected3Hexagons = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            _touchStartPosition = (Vector2) mouseWorldPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            _touchFinishPosition = (Vector2) mouseWorldPosition;
            CheckRotation();
        }
    }
    
    Transform[] GetClosestHexagons(Vector3 position)
    {
        Transform[] nearest3Hexagons = new Transform[3];
        int k = 0;
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(position.x,position.y),radius);
        List<Transform> transforms = new List<Transform>();
        foreach (var col in colliders)
        {
            transforms.Add(col.transform);
        }
        float[] distances = new float[transforms.Count];
        int i = 0;
        foreach (Transform t in transforms)
        {
            float dist = Vector2.Distance(t.position, position);
            distances[i] = dist;
            i++;
        }
        
        float[] sortedDistances = new float[distances.Length];
        sortedDistances = distances;
        Array.Sort(sortedDistances);
        for ( i = 0; i < 3; i++)
        {
            for (int j = 0; j < transforms.Count; j++)
            {
                if (sortedDistances[i] == Vector2.Distance(position,transforms[j].position))
                {
                    nearest3Hexagons[k] = transforms[j];
                    k++;
                }
            }
        }
      
        return nearest3Hexagons;
    }

    IEnumerator ChangeColors(Transform[] transforms)
    {
        Color[] colors = new Color[3];
        int i = 0;
        foreach (var t in transforms)
        {
            colors[i] = t.GetComponent<SpriteRenderer>().color;
            i++;
            t.GetComponent<SpriteRenderer>().color=Color.black;
        }
        yield return new WaitForSeconds(1f);
        i = 0;
        foreach (var t in transforms)
        {
            t.GetComponent<SpriteRenderer>().color=colors[i];
            i++;
        }
    }

    private void OutlineOn(Transform[] transforms)
    {
        foreach (var t in transforms)
        {
            t.GetComponent<Outline>().enabled = true;
        }
    }
    private void OutlineOff(Transform[] transforms)
    {
        foreach (var t in transforms)
        {
            t.GetComponent<Outline>().enabled = false;
        }
    }

    private void CheckRotation()
    {
        var distanceX = _touchFinishPosition.x - _touchStartPosition.x;
        var distanceY = _touchFinishPosition.y - _touchStartPosition.y;
        if (Mathf.Abs(distanceX) > 0.2f || Mathf.Abs(distanceY) > 0.2f)
        {
            Rotate();
        }
        else
        {
            _nearest3Hexagons=GetClosestHexagons(_touchStartPosition);
            
            if (_selected3Hexagons != null)
            {
                if (_nearest3Hexagons != _selected3Hexagons)
                {
                    OutlineOff(_selected3Hexagons);
                } 
            }
            
            //StartCoroutine(ChangeColors(_nearest3Hexagons));
            _selected3Hexagons = _nearest3Hexagons;
            
            OutlineOn(_selected3Hexagons);
        }
    }

    private void Rotate()
    {
        float x1, x2, x3, y1, y2, y3;
        Vector2 pos1, pos2, pos3;
        Transform first, second, third;
        
        /* Taking each position to local variables to prevent data loss during rotation */
        first = _selected3Hexagons[0];
        second = _selected3Hexagons[1];
        third = _selected3Hexagons[2];



        x1 = first.position.x;
        x2 = second.position.x;
        x3 = third.position.x;

        y1 = first.position.y;
        y2 = second.position.y;
        y3 = third.position.y;

        pos1 = first.transform.position;
        pos2 = second.transform.position;
        pos3 = third.transform.position;


        /* If rotation is clokwise, rotate to the position of element on next index, else rotate to previous index */
        first.GetComponent<Hexagon>().Rotate(x2,y2,pos2);
        //gameGrid[x2][y2] = first;

        second.GetComponent<Hexagon>().Rotate(x3,y3,pos3);
        //gameGrid[x3][y3] = second;

        third.GetComponent<Hexagon>().Rotate(x1,y1,pos1);
        //gameGrid[x1][y1] = third;
    }

    private Vector3 FindCenterPoint(GameObject[] gos){
        if (gos.Length == 0)
            return Vector3.zero;
        if (gos.Length == 1)
            return gos[0].transform.position;
        var bounds = new Bounds(gos[0].transform.position, Vector3.zero);
        for (var i = 1; i < gos.Length; i++)
            bounds.Encapsulate(gos[i].transform.position); 
        return bounds.center;
    }
}
