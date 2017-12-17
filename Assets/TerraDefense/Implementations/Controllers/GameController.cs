using System;
using System.Collections.Generic;
using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Players;
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
        
        private float _provinceHeight;
        private float _provinceWidth;
        private List<Province> _generatedProvinces;
        private int _provincesPerCountry;
        private Dictionary<int, Dictionary<int, Province>> _provincesMap;

        public Player Player { get; set; }
        public Alliance Alliance { get; set; }
        public Aliens Aliens { get; set; }
        public Clock Clock { get; set; }
        public AIPlayer AiPlayer { get; set; }
        private void Start () {
            _provinceHeight = ProvinceGameObject.GetComponent<BoxCollider2D>().size.y * ProvinceGameObject.transform.localScale.y;
            _provinceWidth = ProvinceGameObject.GetComponent<BoxCollider2D>().size.x * ProvinceGameObject.transform.localScale.x;
            _provincesMap = new Dictionary<int, Dictionary<int, Province>>();
            _provincesPerCountry = Convert.ToInt32(Math.Pow(MapSquareWidth, 2) / NumberOfCountries);

            SetupGameData();
            Initialize();
            CreateProvincesMap();
            CreateCountries();
            Invoke("CreateInvaders", 2f);
        }

        private void SetupGameData()
        {
            if (NewGameData.NumberOfInvaders != 0)
                NumberOfInvaders = NewGameData.NumberOfInvaders;
        }

        private void CreateProvincesMap()
        {
            for (var x = 0; x < MapSquareWidth; x++)
            {
                _provincesMap.Add(x, new Dictionary<int, Province>());
                for (var y = 0; y < MapSquareWidth; y++)
                {
                    var province = Instantiate(ProvinceGameObject).GetComponent<Province>();
                    province.gameObject.transform.position = new Vector2(x * _provinceWidth, -y * _provinceHeight);
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
                country.Color = color;
                country.UnitColor = unitColor;
                country.Credits = 100;
                country.Name = "Country" + i;
#if UNITY_EDITOR
                country.gameObject.name = "Country" + i;
#endif
                country.Alliance = Alliance;
                Alliance.Countries.Add(country);

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
                }
            }

        }

        private void Initialize()
        {
            Player = Instantiate(PlayerGameObject).GetComponent<Player>();
            AiPlayer = Instantiate(AiPlayerGameObject).GetComponent<AIPlayer>();
            Clock = Instantiate(ClockGameObject).GetComponent<Clock>();
            Aliens = Instantiate(AliensGameObject).GetComponent<Aliens>();
            AiPlayer.Aliens = Aliens;
            Alliance = Instantiate(AllianceGameObject).GetComponent<Alliance>();
        }

        private bool IsPowerOfTwo(int x)
        {
            while (((x % 2) == 0) && x > 1)
            {
                x /= 2;
            }
            return (x == 1);
        }
    }
}
