using System;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private static Controls instance;
    public static Controls MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Controls>();
            }

            return instance;
        }
    }
    public float radius = 8f;
    [SerializeField] private Camera mainCamera;

    private Transform[] _nearest3Hexagons = new Transform[3];
    public Transform[] _selected3Hexagons = new Transform[3];

    private Vector2 _touchStartPosition;
    private Vector2 _touchFinishPosition;
    
    public bool touching = false;
    public bool rotationEnabled = false;
    public int rotationCount = 0;
    
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_selected3Hexagons[i] == null)
            {
                DeSelect();
            }
        }

        if (!Board.MyInstance.rotating && !Board.MyInstance.constructing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touching = true;
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                _touchStartPosition = (Vector2) mouseWorldPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                touching = false;
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                _touchFinishPosition = (Vector2) mouseWorldPosition;
                CheckRotation();
            }

            if (rotationCount == 3)
            {
                rotationCount = 0;
                rotationEnabled = false;
            }
            if (rotationEnabled)
            {
                Rotate();
                rotationCount++;
            }
        }
    }

    Transform[] GetClosestHexagons(Vector3 position)
    {
        Transform[] nearest3Hexagons = new Transform[3];
        int k = 0;
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(position.x, position.y), radius);
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
        for (i = 0; i < 3; i++)
        {
            for (int j = 0; j < transforms.Count; j++)
            {
                if (Math.Abs(sortedDistances[i] - Vector2.Distance(position, transforms[j].position)) < 0.001f)
                {
                    if (k == 2)
                    {
                        if (nearest3Hexagons[0].GetComponent<Hexagon>().y ==
                            nearest3Hexagons[1].GetComponent<Hexagon>().y)
                        {
                            if (nearest3Hexagons[0].GetComponent<Hexagon>().y !=
                                transforms[j].GetComponent<Hexagon>().y)
                            {
                                nearest3Hexagons[k] = transforms[j];
                                k++;
                            }
                        }
                        else if(nearest3Hexagons[0].GetComponent<Hexagon>().x ==
                                nearest3Hexagons[1].GetComponent<Hexagon>().x)
                        {
                            if (nearest3Hexagons[0].GetComponent<Hexagon>().x !=
                                transforms[j].GetComponent<Hexagon>().x)
                            {
                                nearest3Hexagons[k] = transforms[j];
                                k++;
                            }
                        }
                        else
                        {
                            nearest3Hexagons[k] = transforms[j];
                            k++;  
                        }
                    }
                    if (k <= 1)
                    {
                        nearest3Hexagons[k] = transforms[j];
                        k++;
                    }
                }
            }
        }

        return nearest3Hexagons;
    }


    private void OutlineOn(Transform[] transforms)
    {
        foreach (var t in transforms)
        {
            SetActiveAllChildren(t,true);
        }
    }

    private void OutlineOff(Transform[] transforms)
    {
        foreach (var t in transforms)
        {
            if (t != null)
            {
                SetActiveAllChildren(t,false);
            }
        }
    }

    private void CheckRotation()
    {
        var distanceX = _touchFinishPosition.x - _touchStartPosition.x;
        var distanceY = _touchFinishPosition.y - _touchStartPosition.y;
        if (Mathf.Abs(distanceX) > 0.2f || Mathf.Abs(distanceY) > 0.2f)
        {
            rotationEnabled = true;
        }
        else
        {
            _nearest3Hexagons = GetClosestHexagons(_touchStartPosition);

            if (_selected3Hexagons[0]!=null)
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
        int x1, x2, x3, y1, y2, y3;
        Vector2 pos1, pos2, pos3;
        Transform first, second, third;

        if (_selected3Hexagons[0] != null)
        {
            first = _selected3Hexagons[0];
            second = _selected3Hexagons[1];
            third = _selected3Hexagons[2];

            /* Taking each position to local variables to prevent data loss during rotation */


            x1 = first.GetComponent<Hexagon>().x;
            x2 = second.GetComponent<Hexagon>().x;
            x3 = third.GetComponent<Hexagon>().x;

            y1 = first.GetComponent<Hexagon>().y;
            y2 = second.GetComponent<Hexagon>().y;
            y3 = third.GetComponent<Hexagon>().y;

            pos1 = first.transform.position;
            pos2 = second.transform.position;
            pos3 = third.transform.position;


            /* If rotation is clokwise, rotate to the position of element on next index, else rotate to previous index */
            first.GetComponent<Hexagon>().Rotate(x2, y2, pos2);
            Board.MyInstance.hexagons[x2,y2] = first.gameObject;

            second.GetComponent<Hexagon>().Rotate(x3, y3, pos3);
            Board.MyInstance.hexagons[x3,y3] = second.gameObject;

            third.GetComponent<Hexagon>().Rotate(x1, y1, pos1);
            Board.MyInstance.hexagons[x1,y1] = third.gameObject;

        }
    }

    private Vector3 FindCenterPoint(GameObject[] gos)
    {
        if (gos.Length == 0)
            return Vector3.zero;
        if (gos.Length == 1)
            return gos[0].transform.position;
        var bounds = new Bounds(gos[0].transform.position, Vector3.zero);
        for (var i = 1; i < gos.Length; i++)
            bounds.Encapsulate(gos[i].transform.position);
        return bounds.center;
    }

    private void DeSelect()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_selected3Hexagons[i] != null)
            {
                SetActiveAllChildren(_selected3Hexagons[i],false);
                _selected3Hexagons[i] = null;
            }
        }
    }

    private void SetActiveAllChildren(Transform t,bool value)
    {
        if (t != null)
        {
            for (int j = 0; j < t.childCount; j++)
            {
                t.GetChild(j).gameObject.SetActive(value);
            }
        }
    }
}
