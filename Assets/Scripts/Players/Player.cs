using System.Collections.Generic;
using Assets.Scripts.Fractions;
using Assets.Scripts.World;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.Players
{
    public class Player : MonoBehaviour
    {
        public Alliance Alliance;
        public Unit SelectedUnit;
        private void Start()
        {
            
        }
        private void Update()
        {
            if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                HandleLeftClick();
            }
        }

        private void HandleLeftClick()
        {
            var screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(new Vector2(screenToWorld.x, screenToWorld.y), Vector2.zero, 0f);
            if (hit.transform != null)
            {
                var unitComponent = hit.transform.gameObject.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    SelectUnit(unitComponent);
                }
            }

        }

        private void SelectUnit(Unit unitComponent)
        {
            if (Alliance.IsUnitAllied(unitComponent))
            {
                SelectedUnit = unitComponent;
            }
        }
    }
}
