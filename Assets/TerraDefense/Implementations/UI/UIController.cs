using System.Collections.Generic;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Controllers;
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

        public bool IsSetUp
        {
            get
            {
                return _isSetUp;
            }

            set
            {
                _isSetUp = value;
            }
        }

        public Text UnitTypeText;
        public Text UnitAttackText;
        public Text UnitDefenceText;
        public Text UnitAntiAirText;
        public Text CountryName;
        public Text CountryBudget;
        public Text CountryPanic;
        public Text GlobalPanicText;
        public Text CountryPanicEffectText;
        public Button TakeControlButton;
        public Text AllianceFoundsText;
        public InputField CreditsInputField;
        public Text AliensVictoryProgressText;
        private int _numberOfAliens;
        private int _destroyedInvaders = -1;
        public Text HumansVictoryProgressText;
        private Province _provinceHandled;
        public Text ProvinceNameText;
        public Text ProvinceDefenseValueText;
        public Text ProvinceAttackValueText;
        public GameObject ProvinceDataPanel;
        private bool _isSetUp;

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
            _numberOfAliens = FindObjectOfType<GameController>().Generator.NumberOfInvaders;
            UpdateHumanVictoryProgressText();
            
        }

        private void FoundsUpdate(float value)
        {
            AllianceFoundsText.text = "Alliance fouds: " + value;
        }

        private void FixedUpdate()
        {
            if(GlobalPanicText && Player && Player.Alliance)
                GlobalPanicText.text = "Global panic value: " + ((int)Player?.Alliance?.AveragePanic).ToString() ?? "";
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

        public void SetupTimeValues(float seconds)
        {
            HourEvent();
            _isSetUp = true;
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

        internal void UpdateUnitStatus(Unit unit)
        {
            if(unit.Status <= 0)
            {
                DisableUnitInfoPanel();
                Player.DeselectUnit();
                return;
            }
            UnitNameText.text = unit.UnitName;
            UnitFactionText.text = unit.Owner.Name;
            UnitStatusText.text = "Status: " + unit.Status;
            UnitTypeText.text = "Type: " + unit.UnitType;
            UnitDefenceText.text = "Defence value: " + unit.DefenceValue;
            UnitAttackText.text = "Attack value: " + unit.AttackValue;
            UnitAntiAirText.text = "Anti air value: " + unit.AirAttackValue;
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

            if(Player.Alliance == null)
            {
                Player.Alliance = FindObjectOfType<Alliance>();
            }

            CountryPanic.text = "Panic level: " + HandledCountry.PanicLevel + (Player.Alliance.AveragePanic > 0 ? " (" + ((HandledCountry.PanicLevel / Player.Alliance.AveragePanic) * 100).ToString().Split(',')[0] + "% of global)" : "");
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

        public void UpdateAliensVictoryProgressText()
        {
            var percentage = (int)((Province.FindProvincesFor(Aliens.Instance).Count / (float)Province.FindProvincesFor(null).Count) * 100);
            AliensVictoryProgressText.text = "Aliens conquered " + percentage + "% of planet";
        }

        public void UpdateHumanVictoryProgressText()
        {
            _destroyedInvaders++;
            HumansVictoryProgressText.text = "Destroyed motherships: " + _destroyedInvaders + "/" + _numberOfAliens;
        }

        public void SendCreditsClicked()
        {
            var moneyToSend = int.Parse(CreditsInputField.text);
            if (moneyToSend > Player.Alliance.Credits)
            {
                CreditsInputField.text = Player.Alliance.Credits.ToString();
                return;
            }
            Player.Alliance.Credits -= moneyToSend;
            HandledCountry.ReceiveInternationalHelp(moneyToSend);
            FoundsUpdate(Player.Alliance.Credits);
            CreditsInputField.text = "0";
        }

        public void SetProvinceData(Province province)
        {
            if (_provinceHandled) UnsetProvince();
            _provinceHandled = province;
            _provinceHandled.UiHandle += UpdateProvinceData;

            if (HandledCountry)
            {
                HandledCountry.OnStatusUpdate -= UpdateCountryValues;
                HandledCountry = null;
            }
            HandledCountry = province.Owner.GetType() == typeof(Country) ? (Country)province.Owner : null;
            if (HandledCountry)
            {
                HandledCountry.OnStatusUpdate += UpdateCountryValues;
                UpdateCountryValues();
            }

            UpdateProvinceData();
        }

        private void UnsetProvince()
        {
            _provinceHandled.UiHandle -= UpdateProvinceData;
            _provinceHandled = null;

            ProvinceDataPanel.SetActive(false);
        }

        private void UpdateProvinceData()
        {
            if (!_provinceHandled) return;
            ProvinceDataPanel.SetActive(true);
            ProvinceNameText.text = "Name: " + _provinceHandled.Name;
            ProvinceDefenseValueText.text = "Defense strength: " + _provinceHandled.DefenseValue;
            ProvinceAttackValueText.gameObject.SetActive(_provinceHandled.AttackValue > 0);
            ProvinceAttackValueText.text = "Attack strength: " + _provinceHandled.AttackValue;
        }

        public void CloseProvincePanelClicked()
        {
            UnsetProvince();
        }
    }
}
