using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Players;
using Assets.TerraDefense.Implementations.UI;
using Assets.TerraDefense.Implementations.Units;
using GController = Assets.TerraDefense.Implementations.Controllers.GameController;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Random = UnityEngine.Random;
using System.Text;

namespace Assets.TerraDefense.Implementations.World
{
    public class MapGenerator : MonoBehaviour
    {
        public int NumberOfInvaders;
        public int NumberOfCountries;
        public int NumberOfStartUnits;
        public float LengthOfHour;
        public double ProvincesPerCountry;

        public GameObject PlayerGameObject;
        public GameObject AiPlayerGameObject;
        public GameObject ClockGameObject;
        public GameObject AliensGameObject;
        public GameObject AllianceGameObject;
        public GameObject CountryGameObject;
        public GameObject ProvinceGameObject;
        public GameObject AlienPlatformGameObject;

        public int MapSquareHeight { get; private set; }
        public int MapSquareLength { get; private set; }

        private int _countryWidth;
        private float _provinceHeight;
        private float _provinceWidth;
        private Dictionary<int, Dictionary<int, Province>> _provincesMap;

        public Player Player { get; set; }
        public Alliance AllianceInstance { get; set; }
        public Aliens Aliens { get; set; }
        public Clock Clock { get; set; }
        public AIPlayer AiPlayer { get; set; }

        public GameObject UnitTriggerObject;
        private List<Province> _generatedProvinces;
        public Camera MinimapCamera;
        public List<string> NamesList;
        public List<Color> AvaibleColors;
        private void Start()
        {
            
            SetupGameData();
            _countryWidth = (int)(Math.Sqrt(ProvincesPerCountry));


            var mapArea = NumberOfCountries * ProvincesPerCountry;
            if (NumberOfCountries == 1)
            {
                MapSquareLength = MapSquareHeight = _countryWidth;
            }
            else
            {
                MapSquareLength = (NumberOfCountries / 2) * _countryWidth;
                MapSquareHeight = 2 * _countryWidth;
            }

            _provinceHeight = ProvinceGameObject.GetComponent<BoxCollider2D>().size.y * ProvinceGameObject.transform.localScale.y;
            _provinceWidth = ProvinceGameObject.GetComponent<BoxCollider2D>().size.x * ProvinceGameObject.transform.localScale.x;
            _provincesMap = new Dictionary<int, Dictionary<int, Province>>();



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
            for (var x = 0; x < MapSquareLength; x++)
            {
                _provincesMap.Add(x, new Dictionary<int, Province>());
                for (var y = 0; y < MapSquareHeight; y++)
                {
                    var province = Instantiate(ProvinceGameObject).GetComponent<Province>();
                    province.gameObject.transform.position = new Vector2(x * _provinceWidth, -y * _provinceHeight);
                    province.OnOwnerChange += uiController.UpdateAliensVictoryProgressText;
                    province.Name = GenerateName();
                    _provincesMap[x].Add(y, province);
                }
            }

            Player.CameraBoundUpLeft = _provincesMap[0][0].gameObject.transform.position;
            Player.CameraBoundDownRight = _provincesMap[MapSquareLength - 1][MapSquareHeight - 1].transform.position;
        }

        private void CreateInvaders()
        {
            for (var i = 0; i < NumberOfInvaders; i++)
            {
                var x = Random.Range(0, MapSquareLength);
                var y = Random.Range(0, MapSquareHeight);
                var province = _provincesMap[x][y];
                var platform = GController.GetUnitInstance(AlienPlatformGameObject, Vector2.zero).GetComponent<PlatformUnit>();
                platform.AliensOwner = Aliens;
                platform.transform.position = province.transform.position;
                platform.OnDestroyed += FindObjectOfType<UIController>().UpdateHumanVictoryProgressText;
                var trigger = Instantiate(UnitTriggerObject, platform.gameObject.transform.position, Quaternion.identity);
                trigger.transform.parent = platform.gameObject.transform;
            }
        }

        private void CreateCountries()
        {
            _generatedProvinces = new List<Province>();
            var startX = 0;
            var startY = 0;

            for (var i = 0; i < NumberOfCountries; i++)
            {
                var provinces = new List<Province>();
                var country = Instantiate(CountryGameObject).GetComponent<Country>();
                country.UnitTriggerObject = UnitTriggerObject;
                country.Color = AvaibleColors[i];
                country.Credits = 100;
                country.Name = FixName(NamesList[i]);
#if UNITY_EDITOR
                country.gameObject.name = "Country" + i + country.Name;
#endif
                country.Alliance = AllianceInstance;
                AllianceInstance.Countries.Add(country);

                for (var x = startX; x < startX + _countryWidth; x++)
                {
                    for (var y = startY; y < startY + _countryWidth; y++)
                    {
                        _provincesMap[x][y].Owner = country;
                        _provincesMap[x][y].enabled = true;
                        provinces.Add(_provincesMap[x][y]);
#if UNITY_EDITOR
                        _provincesMap[x][y].gameObject.name = "Country" + i + "Province:" + x + y;
#endif
                    }

                }
                startX += _countryWidth;

                if (startX > (MapSquareLength - _countryWidth))
                {
                    startX = 0;
                    startY += _countryWidth;
                }

                for (var u = 0; u < NumberOfStartUnits; u++)
                {
                    var unit =
                        GController.GetUnitInstance(country.AvaibleUnits[Random.Range(0, country.AvaibleUnits.Count - 1)].gameObject, Vector2.zero)
                            .GetComponent<Unit>();
                    unit.Owner = country;
                    var province = provinces[Random.Range(0, provinces.Count - 1)];
                    if (province.AlliedUnits == null)
                        province.AlliedUnits = new List<Unit>();
                    province.AlliedUnits.Add(unit);
                    unit.SetNewTarget(province.gameObject.transform.position);
                    unit.gameObject.transform.position = province.gameObject.transform.position;

                    var trigger = Instantiate(UnitTriggerObject, unit.gameObject.transform.position, Quaternion.identity);
                    trigger.transform.parent = unit.gameObject.transform;
                }
            }

        }

        internal void StartSecondWave()
        {
            NumberOfInvaders *= 2;
            CreateInvaders();
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

        private string GenerateName()
        {
            var nm1 = new string[] { "b", "c", "d", "f", "g", "h", "l", "m", "n", "p", "r", "s", "t", "w", "y", "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z", "bl", "br", "ch", "cl", "cr", "dr", "fl", "fr", "gl", "gr", "pl", "pr", "sc", "sh", "sk", "sl", "sm", "sn", "sp", "st", "sw", "tr", "tw", "wh", "wr", "sch", "scr", "sph", "shr", "spl", "spr", "str", "thr" };
            var nm2 = new string[] { "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "ai", "eo", "ea", "ee", "oo", "oa", "ia", "io" };
            var nm3 = new string[] { "br", "bl", "c", "ch", "cl", "ct", "ck", "cc", "d", "dg", "dw", "dr", "dl", "f", "g", "gg", "gl", "gw", "gr", "h", "k", "kr", "kw", "l", "ll", "lb", "ld", "lg", "lm", "ln", "lr", "lw", "lz", "m", "mr", "ml", "nw", "n", "nn", "ng", "nl", "p", "ph", "r", "rb", "rc", "rd", "rg", "rl", "rm", "rn", "rr", "rs", "rst", "rt", "rth", "rtr", "rw", "rv", "s", "ss", "sh", "st", "sth", "str", "sl", "sw", "t", "tb", "tl", "tg", "tm", "tn", "tw", "th", "tt", "v", "w", "wl", "wn", "x", "z" };
            var nm4 = new string[] { "c", "d", "f", "ff", "g", "gg", "h", "l", "ll", "m", "mm", "n", "nn", "p", "pp", "r", "rr", "s", "ss", "t", "tt", "w" };
            var nm5 = new string[] { "st", "sk", "sp", "nd", "nt", "nk", "mp", "rd", "ld", "lp", "rk", "lt", "lf", "pt", "ft", "ct", "t", "d", "k", "n", "p", "l", "g", "m", "s", "b", "c", "t", "d", "k", "n", "p", "l", "g", "m", "s", "b", "c" };
            var nm6 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "West", "East", "North", "South", "Little", "Upper", "Lower", "Fort", "Upper West", "Upper East", "Upper North", "Upper South", "Lower West", "Lower East", "Lower North", "Lower South", "Midtown", "Waterside", "Bayside", "Downtown" };
            var nm7 = new string[] { "", "Acre", "Avenue", "Bazaar", "Boulevard", "Center", "Circle", "Corner", "Cross", "District", "East", "Garden", "Grove", "Heights", "Hill", "Hills", "Market", "North", "Park", "Place", "Plaza", "Point", "Road", "Row", "Side", "South", "Square", "Street", "Town", "Vale", "Valley", "West", "Wood", "Yard" };

            int rnd, rnd2, rnd3, rnd4, rnd5, rnd6, rnd7 = 0;
            string names = "";
            var i = Random.Range(1, 10);
            rnd6 = (int)Math.Floor(Random.value * nm6.Length);
            rnd7 = (int)Math.Floor(Random.value * nm7.Length);

            if (rnd6 < 20)
            {
                while (rnd7 == 0)
                {
                    rnd7 = (int)Math.Floor(Random.value * nm7.Length);
                }
            }
            else
            {
                rnd7 = 0;
            }
            rnd = (int)Math.Floor(Random.value * nm1.Length);
            rnd2 = (int)Math.Floor(Random.value * nm2.Length);
            rnd5 = (int)Math.Floor(Random.value * nm5.Length);
            if (i < 2)
            {
                names = nm6[rnd6] + " " + nm1[rnd] + nm2[rnd2] + nm5[rnd5] + "  " + nm7[rnd7];
            }
            else if (i < 4)
            {
                rnd3 = (int)Math.Floor(Random.value * nm3.Length);
                rnd4 = (int)Math.Floor(Random.value * nm2.Length);
                names = nm6[rnd6] + " " + nm1[rnd] + nm2[rnd2] + nm3[rnd3] + nm2[rnd4] + nm5[rnd5] + "  " + nm7[rnd7];
            }
            else if (i < 8)
            {
                rnd3 = (int)Math.Floor(Random.value * nm4.Length);
                rnd4 = (int)Math.Floor(Random.value * nm2.Length);
                names = nm6[rnd6] + " " + nm1[rnd] + nm2[rnd2] + nm4[rnd3] + nm2[rnd4] + nm5[rnd5] + "  " + nm7[rnd7];
            }
            else
            {
                rnd3 = (int)Math.Floor(Random.value * nm3.Length);
                rnd4 = (int)Math.Floor(Random.value * nm2.Length);
                rnd6 = (int)Math.Floor(Random.value * nm4.Length);
                rnd7 = (int)Math.Floor(Random.value * nm2.Length);
                if (i < 8)
                {
                    names = nm6[rnd6] + " " + nm1[rnd] + nm2[rnd2] + nm3[rnd3] + nm2[rnd4] + nm4[rnd6] + nm2[rnd7] + nm5[rnd5] + "  " + nm7[rnd7];
                }
                else
                {
                    names = nm6[rnd6] + " " + nm1[rnd] + nm2[rnd2] + nm4[rnd6] + nm2[rnd4] + nm3[rnd3] + nm2[rnd7] + nm5[rnd5] + "  " + nm7[rnd7];
                }

            }
            return FixName(names);
        }

        private string FixName(string name)
        {
            var words = name.Split(' ');
            var sb = new StringBuilder();

            foreach (var word in words)
            {
                if (word == "") continue;
                sb.Append(word.First().ToString().ToUpper() + word.Substring(1));
                sb.Append(" ");
            }


            return sb.ToString();
        }

    }
}
