using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.UI;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.TerraDefense.Implementations.Players
{
    public class Player : MonoBehaviour
    {
        public float ZoomMin;
        public float ZoomMax;
        public float ScrollingSpeed;
        public Alliance Alliance;
        public Unit SelectedUnit;
        public Camera Camera;
        private Country _handledCountry;
        public OrderType OrderType { get; set; }
        public MainMenuController MenuController;
        
        public UIController UIController { get; set; }
        private void Start()
        {
            Camera = Camera.main;
            UIController = FindObjectOfType<UIController>();
            MenuController = FindObjectOfType<MainMenuController>();
            MenuController.TurnOffMenu();


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
                var clickedObject = hit.transform.gameObject;
                var unit = clickedObject.GetComponent<Unit>();
                if (unit)
                {
                    SelectUnit(unit);
                    return;
                }

                var province = clickedObject.GetComponent<Province>();
                if (province)
                {
                    if (!_handledCountry)
                    {
                        try
                        {
                            _handledCountry = (Country) province.Owner;
                            UIController.ShowCommandPanel(true);
                        }
                        catch
                        {
                            
                        }
                    }
                    else if(_handledCountry && OrderType != OrderType.None)
                    {
                        Alliance.SendCommandToCountry(OrderType, province.gameObject.GetComponent<MonoBehaviour>(), _handledCountry);
                        OrderType = OrderType.None;
                        UIController.ShowCommandPanel(false);
                        _handledCountry = null;
                    }
                    return;
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
