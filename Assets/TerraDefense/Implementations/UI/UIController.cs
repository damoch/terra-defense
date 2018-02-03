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
        public Text CountryName;
        public Text CountryBudget;
        public Text CountryPanic;
        public Text GlobalPanicText;
        public Text CountryPanicEffectText;
        public Button TakeControlButton;
        public Text AllianceFoundsText;
        public InputField CreditsInputField;
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

        private void FoundsUpdate(float value)
        {
            AllianceFoundsText.text = "Alliance fouds: " + value;
        }

        private void FixedUpdate()
        {
            GlobalPanicText.text = "Global panic value: " + ((int)Player?.Alliance?.AveragePanic).ToString();
        }

        public void HourEvent()
        {
            var value = Clock.GameDateTime;
            DateTimeText.text = value.Hour + ":00, " + value.Day + "/" + value.Month + "/" + value.Year;

            if(Player.Alliance && Player.Alliance.OnFoundsUpdate == null)
            {
                Player.Alliance.OnFoundsUpdate += FoundsUpdate;
                FoundsUpdate(Player.Alliance.Credits);
            }
        }

        public void SetUnitInfo(Unit unit)
        {
            UnitInfoPanel.SetActive(true);
            CountryOptionsButton.gameObject.SetActive(unit.Owner.GetType() == typeof(Country));
            TakeControlButton.gameObject.SetActive(unit.Owner.GetType() == typeof(Country));
            if (HandledCountry)
            {
                HandledCountry.OnStatusUpdate -= UpdateCountryValues;
                HandledCountry = null;
            }
            HandledCountry = unit.Owner.GetType() == typeof(Country) ? (Country)unit.Owner : null;
            if (HandledCountry)
            {
                HandledCountry.OnStatusUpdate += UpdateCountryValues;
                UpdateCountryValues();
            }
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
            UpdateCountryValues();
        }

        private void UpdateCountryValues()
        {
            if (!HandledCountry) return;
            CountryName.text = "Country name: " + HandledCountry.Name;
            CountryBudget.text = "Budget: " + HandledCountry.Credits;
            CountryPanic.text = "Panic level: " + HandledCountry.PanicLevel + (Player.Alliance.AveragePanic > 0 ? " (" + HandledCountry.PanicLevel / Player.Alliance.AveragePanic + "% of global)" : "");
            CountryPanicEffectText.gameObject.SetActive(HandledCountry.PanicEffect);
            SendHelpToButton.gameObject.SetActive(!HandledCountry.PanicEffect);
            FortifyProvinceButton.gameObject.SetActive(!HandledCountry.PanicEffect);
            AttackProvinceButton.gameObject.SetActive(!HandledCountry.PanicEffect);
        }
        public void TakeControlOverUnit()
        {
            Player.SelectedUnit.ChangeOwner(Player.Alliance);
            HandledCountry.OnStatusUpdate -= UpdateCountryValues;
            HandledCountry = null;
            TakeControlButton.gameObject.SetActive(false);
        }

        public void SendCreditsClicked()
        {
            var moneyToSend = int.Parse(CreditsInputField.text);
            if (moneyToSend > Player.Alliance.Credits) return;
            Player.Alliance.Credits -= moneyToSend;
            HandledCountry.ReceiveInternationalHelp(moneyToSend);
            FoundsUpdate(Player.Alliance.Credits);
        }
    }
}
