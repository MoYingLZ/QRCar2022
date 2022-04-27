using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadScene : MonoBehaviour
{
    public string loadingname;
    public int loadtime;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(loadtime);
        UnityEngine.SceneManagement.SceneManager.LoadScene(loadingname);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
