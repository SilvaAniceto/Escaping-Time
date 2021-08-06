using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement_Script : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] float frequency;
    [SerializeField] float magnitude;
    [SerializeField] float offset;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + transform.up * Mathf.Sin(Time.time * frequency + offset) * magnitude;
    }
}
