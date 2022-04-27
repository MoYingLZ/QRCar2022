using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyGameStudio.Curved;

public class GameLogic : MonoBehaviour
{
    public GameObject Car1;
    public GameObject Human1;
    public GameObject UI;
    public Curved_control control;
    float x;
    public AudioSource source;
    public Animator an;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameLogicCro());
    }

    IEnumerator GameLogicCro()
    {
        yield return new WaitForSeconds(5f);
        Car1.SetActive(false);
        Human1.SetActive(false);
        UI.SetActive(false);
        while (x<0.016f)
        {
            yield return new WaitForEndOfFrame();
            x += 0.00001f;
            control.x = x;
        }
        while (x> -0.016f)
        {
            yield return new WaitForEndOfFrame();
            x -= 0.00001f;
            control.x = x;
        }
        while (x < 0)
        {
            yield return new WaitForEndOfFrame();
            x += 0.00001f;
            control.x = x;
        }
        yield return new WaitForSeconds(10);
        source.Stop();
        an.speed = 0;
        an.gameObject.GetComponent<Movement_control>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
