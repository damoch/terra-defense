using System.Collections.Generic;
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
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                    var rayPoint = ray.GetPoint(distance);
                    unit.SetNewTarget(rayPoint);
                }
            }
        }
    }
}
