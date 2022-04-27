using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curved_control : MonoBehaviour
{
    [Range(-0.1f, 0.1f)]
    public float x;
    [Range(-0.1f, 0.1f)]
    public float y;

    public Material[] materials;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        foreach (var m in this.materials)
        {
            m.SetFloat("x", this.x);
            m.SetFloat("y", this.y);
        }
    }
}
