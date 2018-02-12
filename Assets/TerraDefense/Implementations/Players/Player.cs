using System;
using System.Collections.Generic;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.UI;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.TerraDefense.Implementations.Players
{
    public class Player : MonoBehaviour, ISaveLoad
    {
        public float ZoomMin;
        public float ZoomMax;
        public float ScrollingSpeed;
        public Alliance Alliance;
        public Unit SelectedUnit;
        public Camera Camera;
        public OrderType OrderType { get; set; }
        public MainMenuController MenuController;
        private string _allianceName;

        public UIController UIController { get; set; }

        public int Priority => 0;

        private void Start()
        {
            Camera = Camera.main;
            MenuController = FindObjectOfType<MainMenuController>();
            MenuController.TurnOffMenu();
            Alliance = FindObjectOfType<Alliance>();
            if(Alliance == null)
            {
                Alliance = FindObjectOfType<Alliance>();
            }

            //if (Camera == null)
            //{
            //}

            OrderType = OrderType.None;
            
            //Debug.Log();
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && MenuController.gameObject.activeInHierarchy)
                MenuController.TurnOffMenu();

            if (Input.GetKey(KeyCode.Escape) && !MenuController.gameObject.activeInHierarchy)
                MenuController.BringUpMainMenu();



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
#if !UNITY_EDITOR
            if (Input.mousePosition.x > Screen.width - 50 || Input.GetKey(KeyCode.RightArrow))
                Camera.transform.Translate(ScrollingSpeed, 0, 0, 0);

            if (Input.mousePosition.x < 50 || Input.GetKey(KeyCode.LeftArrow))
                Camera.transform.Translate(-ScrollingSpeed, 0, 0, 0);

            if (Input.mousePosition.y > Screen.height - 50 || Input.GetKey(KeyCode.UpArrow))
                Camera.transform.Translate(0, ScrollingSpeed, 0, 0);

            if (Input.mousePosition.y < 50 || Input.GetKey(KeyCode.DownArrow))
                Camera.transform.Translate(0, -ScrollingSpeed, 0, 0);
#else
            if (Input.GetKey(KeyCode.RightArrow))
                Camera.transform.Translate(ScrollingSpeed, 0, 0, 0);

            if (Input.GetKey(KeyCode.LeftArrow))
                Camera.transform.Translate(-ScrollingSpeed, 0, 0, 0);

            if (Input.GetKey(KeyCode.UpArrow))
                Camera.transform.Translate(0, ScrollingSpeed, 0, 0);

            if (Input.GetKey(KeyCode.DownArrow))
                Camera.transform.Translate(0, -ScrollingSpeed, 0, 0);
#endif
        }

        private void HandleRightClick()
        {
            var gameObject = GetClickedObject();
            if (!gameObject) return;
            var province = gameObject.GetComponent<Province>();

            if (province && OrderType != OrderType.None)
            {
                Alliance.SendCommandToCountry(OrderType, province, UIController.HandledCountry);
                OrderType = OrderType.None;
                DeselectUnit();
                return;
            }
            if (SelectedUnit != null && SelectedUnit.Owner == Alliance)
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
            var gameObject = GetClickedObject();
            if (gameObject)
            {
                var unit = gameObject.GetComponent<Unit>();
                if (unit)
                {
                    SelectUnit(unit);
                    return;
                }

                var province = gameObject.GetComponent<Province>();
                if (province)
                {
                    SelectProvince(province);
                    return;
                }
            }
        }

        public void DeselectUnit()
        {
            if (!SelectedUnit) return;
            
            SelectedUnit.OnStatusUpdate -= UIController.UpdateUnitStatus;
            SelectedUnit = null;
            UIController.DisableUnitInfoPanel();
            
        }

        private void SelectUnit(Unit unitComponent)
        {
            if (UIController.OrderPanelActive) return;
            DeselectUnit();

            UIController.SetUnitInfo(unitComponent);
            unitComponent.OnStatusUpdate += UIController.UpdateUnitStatus;
            if (!Alliance.IsEnemy(unitComponent))
            {
                SelectedUnit = unitComponent;
                
            }
        }

        private void SelectProvince(Province province)
        {
            UIController.SetProvinceData(province);
        }

        public Dictionary<string, string> GetSavableData()
        {
            return new Dictionary<string, string>
            {
                { "type", GetType().FullName },
                { "name", gameObject.name },
                { "alliance", Alliance.Name }
            };
        }

        public void SetSavableData(Dictionary<string, string> json)
        {
            _allianceName = json["alliance"];
        }

        private GameObject GetClickedObject()
        {
            var screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(new Vector2(screenToWorld.x, screenToWorld.y), Vector2.zero, 0f);

            return hit.transform?.gameObject;
            
        }
    }
}
