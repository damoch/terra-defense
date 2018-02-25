using System;
using System.Collections.Generic;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.IO;
using Assets.TerraDefense.Implementations.Players;
using Assets.TerraDefense.Implementations.UI;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Controllers
{
    public class GameController : MonoBehaviour
    {
        public KeyCode PauseKey;
        public GameObject Menu;
        public GameObject Options;
        public GameObject NewGameOptions;
        public SaveLoadManager SaveLoadManager;
        
        public int ProvincesPerCountry;
        private bool _gamePaused;
        
        public MapGenerator Generator;
        private void Start () {

            if (PlayerPrefs.HasKey("StartInstruction"))
            {
                var option = (StartInstruction)Enum.Parse(typeof(StartInstruction), PlayerPrefs.GetString("StartInstruction"));
                switch (option)
                {
                    case StartInstruction.LoadGame:
                        if (!PlayerPrefs.HasKey(SaveLoadManager.LoadGameNameKey)) return;
                        SaveLoadManager.LoadGame(PlayerPrefs.GetString(SaveLoadManager.LoadGameNameKey));
                        return;
                }

            }
            Generator.enabled = true;//start generation
           
        }


    
        private bool IsPowerOfTwo(double x)
        {
            while (((x % 2) == 0) && x > 1)
            {
                x /= 2;
            }
            return (x == 1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(PauseKey) && !_gamePaused)
            {
                Time.timeScale = 0;
                _gamePaused = true;
                Menu.SetActive(true);
            }
            else if(Input.GetKeyDown(PauseKey) && _gamePaused && !Options.activeInHierarchy && !NewGameOptions.activeInHierarchy)
            {
                Time.timeScale = 1;
                _gamePaused = false;
                Menu.SetActive(false);
            }
        }

    }
}
