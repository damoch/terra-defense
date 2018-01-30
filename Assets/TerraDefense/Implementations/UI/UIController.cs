using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Factions;
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
        public Text UnitStatusText;
        public Player Player { get; set; }
        public Clock Clock { get; set; }
        public Dictionary<Button, OrderType> OrderTypesForButtons { get; set; }
        public Button CountryOptionsButton;
        public Country HandledCountry { get; set; }
        public GameObject HandledObject { get; set; }
        public Button CancelButton;
        public bool OrderPanelActive { get { return OrderPanel.activeInHierarchy; }  }
        public void Setup ()
        {
            Clock = FindObjectOfType<Clock>();
            Player = FindObjectOfType<Player>();

            if (!Player.UIController)
            {
                Player.UIController = this;
            }

            OrderTypesForButtons = new Dictionary<Button, OrderType>
            {
                {AttackProvinceButton, OrderType.AttackProvince},
                {FortifyProvinceButton, OrderType.FortifyProvince},
                {SendHelpToButton, OrderType.SendHelpTo},
                {CancelButton, OrderType.None }
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
            CountryOptionsButton.gameObject.SetActive(unit.Owner.GetType() == typeof(Country));
            UpdateUnitStatus(unit);
        }

        public void DisableUnitInfoPanel()
        {
            if (!UnitInfoPanel.activeInHierarchy) return;
            UnitInfoPanel.SetActive(false);
            UnitNameText.text = "";
            UnitFactionText.text = "";
            UnitStatusText.text = "";
        }

        public void SetupTimeValues()
        {
            HourEvent();
        }

        public void ShowCommandPanel(bool show)
        {
            OrderPanel.SetActive(show);
            if (UnitInfoPanel.activeInHierarchy) DisableUnitInfoPanel();
            
            foreach (var button in OrderTypesForButtons.Keys)
            {
                button.gameObject.SetActive(show);
            }
        }

        public void SetOrderType(Button sender)
        {
            Player.OrderType = OrderTypesForButtons[sender];

            ShowCommandPanel(false);
        }

        internal void ShowTeritoryPanel(Province province)
        {
            //throw new NotImplementedException();
        }

        internal void UpdateUnitStatus(Unit unit)
        {
            if(unit.Status <= 0)
            {
                DisableUnitInfoPanel();
                Player.DeselectUnit();
                return;
            }
            UnitNameText.text = unit.name;
            UnitFactionText.text = unit.Owner.name;
            UnitStatusText.text = "Status: " + unit.Status;
        }

        public void CountryOptionsClicked()
        {
            ShowCommandPanel(true);
        }
    }
}
