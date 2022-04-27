using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToOpenDoor : MonoBehaviour
{
    private bool isOpen;
    private bool opening;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (!opening)
        {
            StartCoroutine(ChangeDoorStatus());
        }
    }
    IEnumerator ChangeDoorStatus()
    {
        opening = true;
        if (isOpen)
        {
            while (transform.localEulerAngles.y > 0.1)
            {
                yield return new WaitForEndOfFrame();
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y - 0.2f, 0);
            }
            isOpen = false;
        }
        else
        {
            while (transform.localEulerAngles.y < 45)
            {
                yield return new WaitForEndOfFrame();
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 0.2f, 0);
            }
            isOpen = true;
        }
        opening = false;
    }

}
