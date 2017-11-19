using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Players;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject PlayerGameObject;
        public GameObject ClockGameObject;
        public GameObject AliensGameObject;
        public GameObject AllianceGameObject;
        public GameObject CountryGameObject;
        public GameObject ProvinceGameObject;
        public float OriginX;
        public float OriginY;

        public int NumberOfCountries;
        public int ProvincesPerCountry;

        private Vector2 _pointOfOrigin;
        private float _provinceHeight;
        private float _provinceWidth;

        public Player Player { get; set; }
        public Alliance Alliance { get; set; }
        public Aliens Aliens { get; set; }
        public Clock Clock { get; set; }
        private void Start () {
            _pointOfOrigin = new Vector2(OriginX, OriginY);
            _provinceHeight = ProvinceGameObject.GetComponent<BoxCollider2D>().size.y;
            _provinceWidth = ProvinceGameObject.GetComponent<BoxCollider2D>().size.x;

            Initialize();
            CreateCountries();
        }

        private void CreateCountries()
        {
            for (var i = 0; i < NumberOfCountries; i++)
            {
                var color = new Color(Random.Range(0,255)/(float)255, Random.Range(0, 1) / (float)255, Random.Range(0, 1) / (float)255);
                
                var country = Instantiate(CountryGameObject).GetComponent<Country>();
                country.Color = color;
                country.Name = "Country" + i;
                country.Alliance = Alliance;
                for (var j = 0; j < ProvincesPerCountry; j++)
                {
                    var province = Instantiate(ProvinceGameObject).GetComponent<Province>();
                    province.gameObject.transform.position = new Vector2(j * _provinceWidth, i * _provinceHeight);
                    province.Owner = country;
                    
                }
            }
        }

        private void Initialize()
        {
            Player = Instantiate(PlayerGameObject).GetComponent<Player>();
            Clock = Instantiate(ClockGameObject).GetComponent<Clock>();
            Aliens = Instantiate(AliensGameObject).GetComponent<Aliens>();
            Alliance = Instantiate(AllianceGameObject).GetComponent<Alliance>();
        }
    }
}
