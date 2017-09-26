﻿using Assets.Scripts.Implementations.Factions;
using Assets.Scripts.Implementations.UI;
using Assets.Scripts.Implementations.Units;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.Implementations.Players
{
    public class Player : MonoBehaviour
    {
        public float ZoomMin;
        public float ZoomMax;
        public float ScrollingSpeed;
        public Alliance Alliance;
        public Unit SelectedUnit;
        public Camera Camera;
        public UIController UIController { get; set; }
        private void Start()
        {
            UIController = FindObjectOfType<UIController>();

            if (Camera == null)
            {
                Camera = Camera.main;
            }


            
            //Debug.Log();
        }
        private void Update()
        {
            if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                HandleLeftClick();
            }
            if (Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                HandleRightClick();
            }
            var size = Camera.orthographicSize;
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && size < ZoomMax)
            {
                Camera.orthographicSize++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && size > ZoomMin)
            {
                Camera.orthographicSize--;
            }

            if (Input.mousePosition.x > Screen.width - 50) Camera.transform.Translate(ScrollingSpeed, 0, 0, 0);
            if (Input.mousePosition.x < 50) Camera.transform.Translate(-ScrollingSpeed, 0, 0, 0);
            if (Input.mousePosition.y > Screen.height - 50) Camera.transform.Translate(0, ScrollingSpeed, 0, 0);
            if (Input.mousePosition.y < 50) Camera.transform.Translate(0, -ScrollingSpeed, 0, 0);
        }

        private void HandleRightClick()
        {
            if (SelectedUnit != null)
            {
                SelectedUnit.SetNewTarget(GetMouseToWorldCoordinates());
            }
        }

        private Vector2 GetMouseToWorldCoordinates()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            return ray.GetPoint(distance);
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
            else
            {
                UIController.DisableUnitInfoPanel();
            }

        }

        private void SelectUnit(Unit unitComponent)
        {
            UIController.SetUnitInfo(unitComponent);
            if (!Alliance.IsEnemy(unitComponent))
            {
                SelectedUnit = unitComponent;
            }
        }
    }
}