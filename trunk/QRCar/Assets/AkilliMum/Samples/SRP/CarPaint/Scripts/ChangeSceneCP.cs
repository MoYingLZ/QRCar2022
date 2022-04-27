using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AkilliMum.SRP.CarPaint
{
    public class ChangeSceneCP : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GoToSamplesScene()
        {
            SceneManager.LoadScene("Samples");
        }
        public void GoToRealtimeScene()
        {
            SceneManager.LoadScene("Realtime");
        }
        public void GoToRealtimeMixProbesScene()
        {
            SceneManager.LoadScene("Realtime Mix Probes");
        }
    }
}