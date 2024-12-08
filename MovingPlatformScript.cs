using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed, moveDistance;
    private float originalX;
    // Start is called before the first frame update
    void Start()
    {
        originalX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = originalX + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector2(newX, transform.position.y);   
    }
}
