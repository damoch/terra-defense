using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Players;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TerraDefense.Implementations.UI
{
    // ReSharper disable once InconsistentNaming
    public class UIController : MonoBehaviour, ITimeAffected
    {
        public Text UnitNameText;
        public Text UnitFactionText;
        public Text DateTimeText;
        public Button AttackProvinceButton;
        public Button FortifyProvinceButton;
        public Button SendHelpToButton;
        public GameObject UnitInfoPanel;
        public GameObject OrderPanel;
        public Player Player { get; set; }
        public Clock Clock { get; set; }
        public Dictionary<Button, OrderType> OrderTypesForButtons { get; set; }
        private void Start ()
        {
            Clock = FindObjectOfType<Clock>();
            Player = FindObjectOfType<Player>();
            OrderTypesForButtons = new Dictionary<Button, OrderType>
            {
                {AttackProvinceButton, OrderType.AttackProvince},
                {FortifyProvinceButton, OrderType.FortifyProvince},
                {SendHelpToButton, OrderType.SendHelpTo}
            };
            HourEvent();
            DisableUnitInfoPanel();
        }

        public void HourEvent()
        {
            var value = Clock.GameDateTime;
            DateTimeText.text = value.Hour + ":00, " + value.Day + "/" + value.Month + "/" + value.Year;
        }

        public void SetUnitInfo(Unit unit)
        {
            UnitInfoPanel.SetActive(true);
            UnitNameText.text = unit.name;
            UnitFactionText.text = unit.Owner.name;
        }

        public void DisableUnitInfoPanel()
        {
            if (!UnitInfoPanel.activeInHierarchy) return;
            UnitInfoPanel.SetActive(false);
            UnitNameText.text = "";
            UnitFactionText.text = "";
        }

        public void SetupTimeValues()
        {
            throw new System.NotImplementedException();
        }

        public void ShowCommandPanel(bool show)
        {
            OrderPanel.SetActive(show);
            foreach (var button in OrderTypesForButtons.Keys)
            {
                button.gameObject.SetActive(show);
            }
        }

        public void SetOrderType(Button sender)
        {
            Player.OrderType = OrderTypesForButtons[sender];

            foreach (var button in OrderTypesForButtons.Keys.Where(a => !a.Equals(sender)))
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
