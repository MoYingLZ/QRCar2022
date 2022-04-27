using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyGameStudio.Curved
{
    public class Movement_control : MonoBehaviour
    {
        [Header("Character Controller")]
        public CharacterController character_controller;

        [Header("animator")]
        public Animator animator;

        // Update is called once per frame
        void Update()
        {
            this.character_controller.Move(new Vector3(0, 0, 8) * Time.deltaTime); 
        }
    }
}


