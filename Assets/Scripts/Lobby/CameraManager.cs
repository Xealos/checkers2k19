using System;
using UnityEngine;

namespace Lobby
{
    public class CameraManager : MonoBehaviour
    {
        public GameObject checkerBoard;
        public GameObject mainCamera; 
        
        private void Update()
        {
            mainCamera.transform.RotateAround(checkerBoard.transform.position, checkerBoard.transform.forward,
                Time.deltaTime * 3.0f);           
        }
    }
}