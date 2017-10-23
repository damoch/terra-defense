using System;
using Assets.Scripts.Abstractions.World;
using Assets.Scripts.Enums;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Implementations.World;
using Assets.Scripts.Implementations.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Implementations.UI
{
    // ReSharper disable once InconsistentNaming
    public class UIController : MonoBehaviour, ITimeAffected
    {
        public Text UnitNameText;
        public Text UnitFactionText;
        public Text DateTimeText;
        public Button AttackProvinceButton;
        public GameObject UnitInfoPanel;
        public Player Player { get; set; }
        public Clock Clock { get; set; }
        private void Start ()
        {
            Clock = FindObjectOfType<Clock>();
            Player = FindObjectOfType<Player>();
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

        public void ShowCommandPanel()
        {
        }

        public void SetOrderType()
        {
            Player.OrderType = OrderType.AttackProvince;
        }
    }
}
