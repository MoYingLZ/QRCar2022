using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollorMoving : MonoBehaviour
{
    public Transform[] ground;
    public float groundMovingSpeed;
    public float groundLength;
    public Material[] shaders;
    private Material currentShader;
    // Start is called before the first frame update
    void Start()
    {
        currentShader = shaders[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (ground[0].localPosition.z < -32)
        {
            Transform temp = ground[0];
            if (ground[0].GetComponent<MeshRenderer>().material != currentShader)
            {
                ground[0].GetComponent<MeshRenderer>().material = currentShader;
            }
            for (int j = 0; j < ground.Length; j++)
            {
                if (j == ground.Length - 1)
                {
                    ground[j] = temp;
                }
                else
                {
                    ground[j] = ground[j + 1];
                }
            }
        }
        for (int i = 0; i < ground.Length; i++)
        {
            if (i == 0)
            {
                ground[i].transform.localPosition -= new Vector3(0, 0, groundMovingSpeed);
            }
            else
            {
                ground[i].localPosition = ground[i - 1].localPosition + new Vector3(0, 0, groundLength);
            }
        }
    }

    public void SwitchShader(int i)
    {
        currentShader = shaders[i];
    }
}
