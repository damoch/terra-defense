﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.IO;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public List<GameObject> UnitsInGame;
        private bool _gamePaused;
        private static Dictionary<string, Stack<GameObject>> _unitsPool;
        private static List<GameObject> _unitPrototypes;
        public int BasePoolSize;
        public bool SecondWaveOn { get; set; }
        public MapGenerator Generator;
        private static MapGenerator _generatorInstance;

        public Dictionary<int, Dictionary<int, Province>> ProvincesMap { get; set; }
        private void Start () {
            PrepareUnitPools();
            _generatorInstance = Generator;
            //PlayerPrefs.DeleteAll(); //fix loading problems
            if (PlayerPrefs.HasKey("StartInstruction"))
            {
                var option = (StartInstruction)Enum.Parse(typeof(StartInstruction), PlayerPrefs.GetString("StartInstruction"));
                switch (option)
                {
                    case StartInstruction.LoadGame:
                        if (SaveLoadManager.LoadGameNameKey == null) ThrowOnFailedLoad();
                        if (!PlayerPrefs.HasKey(SaveLoadManager.LoadGameNameKey))ThrowOnFailedLoad();
                        if (!SaveLoadManager.LoadGame(PlayerPrefs.GetString(SaveLoadManager.LoadGameNameKey))) ThrowOnFailedLoad();
                        break;
                    case StartInstruction.NewGame:
                    default:
                        Generator.enabled = true;
                        break;

                }
            }


            Generator.enabled = true;
            PlayerPrefs.DeleteKey("StartInstruction");
        }

        private void ThrowOnFailedLoad()
        {
            Debug.LogError("Load failed");
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("mainMenu");
        }

        private void PrepareUnitPools()
        {
            _unitsPool = new Dictionary<string, Stack<GameObject>>();
            _unitPrototypes = new List<GameObject>();
            foreach(var unitType in UnitsInGame)
            {
                if (!_unitsPool.Keys.Contains(unitType.GetComponent<Unit>().UnitName))
                {
                    _unitsPool.Add(unitType.GetComponent<Unit>().UnitName, new Stack<GameObject>());
                    _unitPrototypes.Add(unitType);
                }
                for(var i = 0; i < BasePoolSize; i++)
                {
                    var unit = Instantiate(unitType);
                    unit.SetActive(false);
                    _unitsPool[unitType.GetComponent<Unit>().UnitName].Push(unit);
                }

            }
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

        public static GameObject GetUnitInstance(GameObject prototype, Vector3 worldCoordinates)
        {
            var unitName = prototype.GetComponent<Unit>().UnitName;
            GameObject result;
            if (_unitsPool[unitName].Count == 0)result = Instantiate(_unitPrototypes.FirstOrDefault(x => x.GetComponent<Unit>().UnitName == unitName));
            else result = _unitsPool[prototype.GetComponent<Unit>().UnitName].Pop();
            if(_generatorInstance  && _generatorInstance.Clock)_generatorInstance.Clock.SetupTimeAffectedObject(result.GetComponent<Unit>());
            result.transform.position = worldCoordinates;
            result.GetComponent<Unit>().SetNewTarget(worldCoordinates);
            result.SetActive(true);
            return result;
        }

        public static void RemoveUnit(GameObject unit)
        {
            unit.SetActive(false);
            unit.transform.position = Vector2.zero;
            var prototype = _unitPrototypes.First(x => x.GetComponent<Unit>().UnitName == unit.GetComponent<Unit>().UnitName).GetComponent<Unit>();
            unit.GetComponent<Unit>().Status = prototype.Status;
            unit.GetComponent<Unit>().AirAttackValue = prototype.AirAttackValue;
            unit.GetComponent<Unit>().AttackValue = prototype.AttackValue;
            unit.GetComponent<Unit>().DefenceValue = prototype.DefenceValue;
            unit.GetComponent<Unit>().Owner = null;
            //unit.GetComponent<Unit>().SetNewTarget(Vector2.zero);
            _unitsPool[prototype.UnitName].Push(unit);
        }

        public static List<Province> FindProvincesNear(Province province)
        {
            var maxX = _generatorInstance.ProvincesMap.Keys.Count() - 1;
            var maxY = _generatorInstance.ProvincesMap.Values.Count() - 1;
            var result = new List<Province>();

            if(province.IndexY + 1 < maxY)
            {
                result.Add(_generatorInstance.ProvincesMap[province.IndexX][province.IndexY + 1]);
            }

            if (province.IndexY - 1 >= 0)
            {
                result.Add(_generatorInstance.ProvincesMap[province.IndexX][province.IndexY + 1]);
            }

            if (province.IndexX - 1 >= 0)
            {
                result.Add(_generatorInstance.ProvincesMap[province.IndexX - 1][province.IndexY]);
            }

            if (province.IndexX + 1 < maxX)
            {
                result.Add(_generatorInstance.ProvincesMap[province.IndexX + 1][province.IndexY]);
            }

            return result;
        }
    }
}
