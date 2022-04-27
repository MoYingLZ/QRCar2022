using UnityEngine;
using System.Collections;

namespace AkilliMum.SRP.CarPaint
{
    public class SimpleRotaterCP : MonoBehaviour
    {
        public float XSpeed = 0;
        public float YSpeed = 0;
        public float ZSpeed = 0;

        void FixedUpdate()
        {
            gameObject.transform.Rotate(new Vector3(XSpeed, YSpeed, ZSpeed),
                Space.Self);
        }
    }
}
