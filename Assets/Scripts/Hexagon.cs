using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public bool lerp = false;
    public Vector2 lerpPosition;
    public float rotateLerpSpeed = 5f;
    public float rotateThreshold = 0.01f;
    void Update() {
        if (lerp) {
            float newX = Mathf.Lerp(transform.position.x, lerpPosition.x, Time.deltaTime*rotateLerpSpeed);
            float newY = Mathf.Lerp(transform.position.y, lerpPosition.y, Time.deltaTime*rotateLerpSpeed);
            transform.position = new Vector2(newX, newY);

			
            if (Vector3.Distance(transform.position, lerpPosition) < rotateThreshold) {
                transform.position = lerpPosition;
                lerp = false;
            }
        }
    }


    /* Function to save rotate changes */
    public void Rotate(float newX, float newY, Vector2 newPos) {
        lerpPosition = newPos;
        lerp = true;
    }
}
