using System.Collections.Generic;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Players;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

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
        public float OriginX;
        public float OriginY;

        public int NumberOfCountries;
        public int ProvincesPerCountry;
        public int NumberOfInvaders;

        private Vector2 _pointOfOrigin;
        private float _provinceHeight;
        private float _provinceWidth;
        private List<Province> _generatedProvinces;

        public Player Player { get; set; }
        public Alliance Alliance { get; set; }
        public Aliens Aliens { get; set; }
        public Clock Clock { get; set; }
        public AIPlayer AIPlayer { get; set; }
        private void Start () {
            _pointOfOrigin = new Vector2(OriginX, OriginY);
            _provinceHeight = ProvinceGameObject.GetComponent<BoxCollider2D>().size.y * ProvinceGameObject.transform.localScale.y;
            _provinceWidth = ProvinceGameObject.GetComponent<BoxCollider2D>().size.x * ProvinceGameObject.transform.localScale.x;

            Initialize();
            CreateCountries();
            Invoke("CreateInvaders", 2f);
        }

        private void CreateInvaders()
        {
            for (int i = 0; i < NumberOfInvaders; i++)
            {
                var index = Random.Range(0, _generatedProvinces.Count);
                var province = _generatedProvinces[index];
                var platform = Instantiate(AlienPlatformGameObject).GetComponent<PlatformUnit>();
                platform.AliensOwner = Aliens;
                platform.transform.position = province.transform.position;
            }
        }

        private void CreateCountries()
        {
            _generatedProvinces = new List<Province>();


            for (var i = 0; i < NumberOfCountries; i++)
            {
                var color = new Color(Random.Range(0,255)/(float)255, Random.Range(0, 1) / (float)255, Random.Range(0, 1) / (float)255);
                var unitColor = new Color(Random.Range(0, 255) / (float)255, Random.Range(0, 1) / (float)255, Random.Range(0, 1) / (float)255);

                var country = Instantiate(CountryGameObject).GetComponent<Country>();
                country.Color = color;
                country.UnitColor = unitColor;
                country.Name = "Country" + i;
                country.Alliance = Alliance;
                for (var j = 0; j < ProvincesPerCountry; j++)
                {
                    var province = Instantiate(ProvinceGameObject).GetComponent<Province>();
                    province.gameObject.transform.position = new Vector2(j * _provinceWidth, i * _provinceHeight);
                    province.Owner = country;
                    _generatedProvinces.Add(province);
                }
            }

        }

        private void Initialize()
        {
            Player = Instantiate(PlayerGameObject).GetComponent<Player>();
            AIPlayer = Instantiate(AiPlayerGameObject).GetComponent<AIPlayer>();
            Clock = Instantiate(ClockGameObject).GetComponent<Clock>();
            Aliens = Instantiate(AliensGameObject).GetComponent<Aliens>();
            AIPlayer.Aliens = Aliens;
            Alliance = Instantiate(AllianceGameObject).GetComponent<Alliance>();
        }
    }
}
