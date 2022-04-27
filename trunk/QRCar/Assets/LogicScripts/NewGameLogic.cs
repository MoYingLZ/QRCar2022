using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameLogic : MonoBehaviour
{
    public GameObject UI1;
    public GameObject UI2;
    public GameObject FollowOpen;
    public GameObject FollowClose;
    public GameObject FollorObj;
    public GameObject[] cars;
    public GameObject[] humens;
    public Animator ani;
   // public FollorMoving moving;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Application.targetFrameRate = 60;
        yield return new WaitForSeconds(3.8f);
        UI1.SetActive(false);
        UI2.SetActive(true);
        yield return new WaitForSeconds(21.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("ViewCar");
        //FollowOpen.SetActive(false);
        //FollowClose.SetActive(true);
        //FollorObj.SetActive(false);
        //yield return new WaitForSeconds(1f);
        //ani.speed = 0;
        //yield return new WaitForSeconds(5f);
        //UI2.SetActive(false);
        //while (humens[0].transform.position.z > -5)
        //{
        //    for (int i = 0; i < humens.Length; i++)
        //    {
        //        humens[i].transform.Translate(new Vector3(0, 0, -0.18f));
        //    }
        //    yield return new WaitForEndOfFrame();
        //}

        //while (cars[7].transform.position.z > -10)
        //{
        //    for (int i = 0; i < cars.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            if (cars[7].transform.position.z > 10)
        //            {
        //                cars[i].transform.Translate(new Vector3(0, 0, -0.08f),Space.World);
        //            }
        //        }
        //        else
        //        {
        //            cars[i].transform.Translate(new Vector3(0, 0, -0.08f),Space.World);
        //        }
        //    }
        //    yield return new WaitForEndOfFrame();
        //}

        //while (cars[0].transform.position.z < 50)
        //{
        //    yield return new WaitForEndOfFrame();
        //    cars[0].transform.Translate(new Vector3(0, 0, 0.08f),Space.World);
        //}
        //ani.speed = 1;
        //yield return new WaitForSeconds(20f);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("ViewCar");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
