using Assets.TerraDefense.Implementations.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.TerraDefense.Implementations.UI
{
    public class NewGameOptionsController : MonoBehaviour
    {
        public InputField NumberOfInvadersInput;
        public Dropdown NumberOfCountriesInput;
        public InputField NumberOfStartUnitsInput;
        public InputField LengthOfHourInput;
        public string NewGameSceneName;
        public Toggle SecondWaveToggle;

        private void Start()
        {
            NumberOfInvadersInput.text = "0";
            //NumberOfCountriesInput.value. = "0";
            NumberOfStartUnitsInput.text = "0";
        }

        public void StartGame()
        {
            CollectDataFromInputs();
            SceneManager.LoadScene(NewGameSceneName);
        }

        private void CollectDataFromInputs()
        {
            NewGameData.NumberOfInvaders = Int32.Parse(NumberOfInvadersInput.text);
            NewGameData.NumberOfCountries = Int32.Parse(NumberOfCountriesInput.options[NumberOfCountriesInput.value].text);
            NewGameData.NumberOfStartUnits = Int32.Parse(NumberOfStartUnitsInput.text);
            NewGameData.LengthOfHour = float.Parse(LengthOfHourInput.text);
            NewGameData.SecondWave = SecondWaveToggle.isOn;
        }
    }
}

