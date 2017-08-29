using Assets.Scripts.World;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    // ReSharper disable once InconsistentNaming
    public class UIController : MonoBehaviour, ITimeAffected
    {
        public Text UnitNameText;
        public Text UnitFactionText;
        public Text DateTimeText;
        public GameObject UnitInfoPanel;
        public Clock Clock { get; set; }
        private void Start ()
        {
            Clock = FindObjectOfType<Clock>();
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
    }
}
