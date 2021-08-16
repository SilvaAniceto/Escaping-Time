using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter_Script : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] float auxTime;
    float shootTime;
    
    void Start()
    {
        shootTime = auxTime; 
    }
    // Update is called once per frame
    void Update()
    {
        shootTime -= Time.deltaTime;
        if (shootTime <= 0)
        {
            Instantiate(fireball, transform.position, transform.rotation);
            shootTime = auxTime;
        }
    }
}
