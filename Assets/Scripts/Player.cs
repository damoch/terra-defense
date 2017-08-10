﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public List<Unit> Units;
        private void Start()
        {
            
        }
        private void Update()
        {
            if (Input.GetMouseButton(button: (int)MouseButton.RightMouse))
            {
                foreach (var unit in Units)
                {
                    Debug.Log("hahah");
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var Distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                    var rayPoint = ray.GetPoint(Distance);
                    var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    target.z = 0f;
                    unit.SetNewTarget(rayPoint);
                }
            }
        }
    
    }
}
