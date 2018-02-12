﻿using System;
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
using Random = UnityEngine.Random;

namespace Assets.TerraDefense.Implementations.Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject PlayerGameObject;
        public GameObject AiPlayerGameObject;
        public GameObject ClockGameObject;
        public GameObject AliensGameObject;
        public GameObject AllianceGameObject;
        public GameObject CountryGameObject;
        public GameObject ProvinceGameObject;
        public GameObject AlienPlatformGameObject;
        public int MapSquareWidth;

        public int NumberOfCountries;
        public int NumberOfInvaders;
        public int NumberOfStartUnits;
        public float LengthOfHour;

        public KeyCode PauseKey;
        public GameObject Menu;
        public GameObject Options;
        public GameObject NewGameOptions;
        public SaveLoadManager SaveLoadManager;

        private float _provinceHeight;
        private float _provinceWidth;
        private List<Province> _generatedProvinces;
        private int _provincesPerCountry;
        private Dictionary<int, Dictionary<int, Province>> _provincesMap;
        private bool _gamePaused;

        public Player Player { get; set; }
        public Alliance AllianceInstance { get; set; }
        public Aliens Aliens { get; set; }
        public Clock Clock { get; set; }
        public AIPlayer AiPlayer { get; set; }

        public GameObject UnitTriggerObject;

        private void Start () {
            _provinceHeight = ProvinceGameObject.GetComponent<BoxCollider2D>().size.y * ProvinceGameObject.transform.localScale.y;
            _provinceWidth = ProvinceGameObject.GetComponent<BoxCollider2D>().size.x * ProvinceGameObject.transform.localScale.x;
            _provincesMap = new Dictionary<int, Dictionary<int, Province>>();
            _provincesPerCountry = Convert.ToInt32(Math.Pow(MapSquareWidth, 2) / NumberOfCountries);

            if (PlayerPrefs.HasKey("StartInstruction"))
            {
                var option = (StartInstruction)Enum.Parse(typeof(StartInstruction), PlayerPrefs.GetString("StartInstruction"));
                switch (option)
                {
                    case StartInstruction.LoadGame:
                        SaveLoadManager.LoadGame();
                        return;
                }
            }

            SetupGameData();
            Initialize();
            CreateProvincesMap();
            CreateCountries();
            Invoke("CreateInvaders", 2f);
        }

        private void SetupGameData()
        {
            if (NewGameData.NumberOfInvaders > 0)
                NumberOfInvaders = NewGameData.NumberOfInvaders;

            if (NewGameData.NumberOfCountries > 0)
                NumberOfCountries = NewGameData.NumberOfCountries;

            if (NewGameData.NumberOfStartUnits > 0)
                NumberOfStartUnits = NewGameData.NumberOfStartUnits;

            if (NewGameData.LengthOfHour > 0)
                LengthOfHour = NewGameData.LengthOfHour;
        }

        private void CreateProvincesMap()
        {
            var uiController = FindObjectOfType<UIController>();
            for (var x = 0; x < MapSquareWidth; x++)
            {
                _provincesMap.Add(x, new Dictionary<int, Province>());
                for (var y = 0; y < MapSquareWidth; y++)
                {
                    var province = Instantiate(ProvinceGameObject).GetComponent<Province>();
                    province.gameObject.transform.position = new Vector2(x * _provinceWidth, -y * _provinceHeight);
                    province.OnOwnerChange += uiController.UpdateAliensVictoryProgressText;
                    _provincesMap[x].Add(y, province);
                }
            }
        }

        private void CreateInvaders()
        {
            for (var i = 0; i < NumberOfInvaders; i++)
            {
                var x = Random.Range(0, MapSquareWidth);
                var y = Random.Range(0, MapSquareWidth);
                var province = _provincesMap[x][y];
                var platform = Instantiate(AlienPlatformGameObject).GetComponent<PlatformUnit>();
                platform.AliensOwner = Aliens;
                platform.transform.position = province.transform.position;
                platform.OnDestroyed += FindObjectOfType<UI.UIController>().UpdateHumanVictoryProgressText;
                var trigger = Instantiate(UnitTriggerObject, platform.gameObject.transform.position, Quaternion.identity);
                trigger.transform.parent = platform.gameObject.transform;
            }
        }

        private void CreateCountries()
        {
            _generatedProvinces = new List<Province>();
            var startX = 0;
            var startY = 0;
            var countryWidth = Convert.ToInt32(Math.Sqrt(_provincesPerCountry));

            for (var i = 0; i < NumberOfCountries; i++)
            {
                var color = new Color(Random.Range(0,255)/(float)255, Random.Range(0, 255) / (float)255, Random.Range(0, 255) / (float)255);
                var unitColor = new Color(Random.Range(0, 255) / (float)255, Random.Range(0, 255) / (float)255, Random.Range(0, 255) / (float)255);
                var provinces = new List<Province>();
                var country = Instantiate(CountryGameObject).GetComponent<Country>();
                country.UnitTriggerObject = UnitTriggerObject;
                country.Color = color;
                country.UnitColor = unitColor;
                country.Credits = 100;
                country.Name = "Country" + i;
#if UNITY_EDITOR
                country.gameObject.name = "Country" + i;
#endif
                country.Alliance = AllianceInstance;
                AllianceInstance.Countries.Add(country);

                for (var x = startX; x < startX + countryWidth; x++)
                {
                    for (var y = startY; y < startY + countryWidth; y++)
                    {
                        _provincesMap[x][y].Owner = country;
                        _provincesMap[x][y].enabled = true;
                        provinces.Add(_provincesMap[x][y]);
#if UNITY_EDITOR
                        _provincesMap[x][y].gameObject.name = "Country" + i + "Province:" + x + y;
#endif
                    }

                }
                if (startX < MapSquareWidth - startX) startX += countryWidth;
                else
                {
                    startX = 0;
                    startY += countryWidth;
                }

                for (var u = 0; u < NumberOfStartUnits; u++)
                {
                    var unit =
                        Instantiate(country.AvaibleUnits[Random.Range(0, country.AvaibleUnits.Count - 1)].gameObject)
                            .GetComponent<Unit>();
                    unit.Owner = country;
                    var province = provinces[Random.Range(0, provinces.Count - 1)];
                    if(province.AlliedUnits == null)
                        province.AlliedUnits = new List<Unit>();
                    province.AlliedUnits.Add(unit);
                    unit.gameObject.transform.position = province.gameObject.transform.position;

                    var trigger = Instantiate(UnitTriggerObject, unit.gameObject.transform.position, Quaternion.identity);
                    trigger.transform.parent = unit.gameObject.transform;
                }
            }

        }

        private void Initialize()
        {
            Player = Instantiate(PlayerGameObject).GetComponent<Player>();
            AiPlayer = Instantiate(AiPlayerGameObject).GetComponent<AIPlayer>();
            Clock = Instantiate(ClockGameObject).GetComponent<Clock>();
            Clock.LengthOfHour = LengthOfHour;
            Aliens = Instantiate(AliensGameObject).GetComponent<Aliens>();
            Aliens.UnitTriggerObject = UnitTriggerObject;
            AiPlayer.Aliens = Aliens;
            AllianceInstance = Instantiate(AllianceGameObject).GetComponent<Alliance>();
            AllianceInstance.UnitTriggerObject = UnitTriggerObject;
            FindObjectOfType<UI.UIController>().Setup();
        }

        private bool IsPowerOfTwo(int x)
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
