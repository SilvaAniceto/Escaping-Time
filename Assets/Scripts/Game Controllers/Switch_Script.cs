using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Script : MonoBehaviour
{
    public static Switch_Script instance;

    [SerializeField] GameObject flame;
    [SerializeField] int code;
    public bool canClick;
    public bool isActive = false;

    Animator anim;
    AudioSource audioSource;
    void Awake()
    {
        instance = this;   
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        flame.SetActive(false);
    }

    void Update()
    {
        if (canClick)
            if (!isActive)
                if (Character_Moviment.moveInstance.Interact)
                    if (Game_Controller.controllerInstance.auxFlames > 0)
                    {
                        if (!audioSource.isPlaying)
                        {
                            audioSource.PlayOneShot(audioSource.clip);
                            StopCoroutine("StopAudio");
                            StartCoroutine("StopAudio");
                        }

                        Game_Controller.controllerInstance.ChangeFlames(-1);
                        anim.SetBool("On", true);
                        Door_Script.instance.password += code;
                        isActive = true;
                        flame.SetActive(true);
                    }
    }

    IEnumerator StopAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        audioSource.Stop();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            canClick = true;            
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            canClick = false;
    }

    public void ResetSwitch()
    {
        GameObject[] auxSwitch = GameObject.FindGameObjectsWithTag("Switches");
        foreach (GameObject item in auxSwitch)
        {
            item.GetComponent<Switch_Script>().isActive = false;
        }        
    }
}
