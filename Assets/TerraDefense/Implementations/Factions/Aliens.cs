using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Aliens : UnitOwner, ISaveLoad, ITimeAffected {
        public static Aliens Instance;
        private List<Unit> _airUnits;
        private List<Unit> _groundUnits;
        public List<Unit> UnitsInReserve;
        public bool ProduceReserves { get { return _airUnits?.Count > 0 && _groundUnits?.Count > 0 && _groundUnits.Sum(x => x.Cost) < Credits; } }
        public bool CallingAnotherAliensWave { get; set; }
        public int HoursUntilSecondWave;
        private int _hoursUntilSecondWavePassed;
        private void Start()
        {
            Instance = this;
            UnitsInReserve = new List<Unit>();
            CallingAnotherAliensWave = false;
            _airUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Air).ToList();
            _airUnits.Sort((x, y) => x.AirAttackValue.CompareTo(y.AirAttackValue));

            _groundUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Ground).ToList();
            _groundUnits.Sort((x, y) => x.AttackValue.CompareTo(y.AttackValue));
        }
        public float AvgAirAttackVal { get
        {
            return AvaibleUnits.Where(x => x.UnitType == UnitType.Air).Average(y => y.AirAttackValue);
        } }

        public float AvgGroundAttackVal
        {
            get
            {
                return AvaibleUnits.Where(x => x.UnitType == UnitType.Ground).Average(y => y.AttackValue);
            }
        }


        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() != typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            var result = new List<Unit>();
            var units =  FindObjectsOfType<PlatformUnit>().ToList();
            foreach (var platformUnit in units)
            {
                result.Add(platformUnit);
            }
            return result;
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            throw new System.Exception("This method shall not be called!");
        }

        public GameObject ProduceUnit(PlatformUnit pu)
        {
            GameObject instance;
            var spawnPosition = pu.gameObject.transform.position;
            if (!pu.TargetProvince) return null;
            var enemies = pu.TargetProvince.AlliedUnits;

            if (pu.Units == null)
                pu.Units = new List<Unit>();

            var friends = pu.Units;

            var enemyAircraftsCount = enemies.Count(x => x.UnitType == UnitType.Air);

            var totalAirAttack = friends.Where(x => x != null).Sum(x => x.AirAttackValue);

            if (totalAirAttack < enemyAircraftsCount * AvgAirAttackVal && _airUnits.Exists(x => x.Cost <= Credits))
            {
                var avaibleAirUnits = _airUnits.Where(x => x.Cost < Credits).ToList();
                if (avaibleAirUnits.Count == 0) return null;
                avaibleAirUnits.Sort((x, y) => x.AirAttackValue.CompareTo(y.AirAttackValue));
                instance = GameController.GetUnitInstance(avaibleAirUnits.First().gameObject, spawnPosition);//, Quaternion.identity);
            }
            else if(_groundUnits.Exists(x => x.Cost <= Credits))
            {
                var avGroundUnits = _groundUnits.Where(x => x.Cost < Credits).ToList();
                if (avGroundUnits.Count == 0) return null;
                avGroundUnits.Sort((x, y) => y.AttackValue.CompareTo(x.AttackValue));
                instance = GameController.GetUnitInstance(avGroundUnits.First().gameObject, spawnPosition);//Instantiate(_groundUnits.First(x => x.Cost <= Credits).gameObject, spawnPosition, Quaternion.identity);
            }
            else
            {
                if (UnitsInReserve.Count == 0) return null;
                var unit = UnitsInReserve.First();
                UnitsInReserve.Remove(unit);
                unit?.SetNewTarget(pu.transform.position);
                return unit.gameObject;
            }
            instance.GetComponent<Unit>().Owner = this;

            var trigger = Instantiate(UnitTriggerObject, instance.transform.position, Quaternion.identity);
            trigger.transform.parent = instance.transform;

            Credits -= instance.GetComponent<Unit>().Cost;
            return instance;
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            Debug.Log("Enemy is attacking");
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            Debug.Log("Enemy is close to property");
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            Debug.Log("Enemy is retreating");
        }
        public override void PropertyChangesOwner(Province province, bool isLost)
        {
            //throw new System.NotImplementedException();
        }

        public override Dictionary<string, string> GetSavableData()
        {
            var dictionary = base.GetSavableData();
            return dictionary;
        }

        public override void SetSavableData(Dictionary<string, string> json)
        {
            base.SetSavableData(json);
        }

        public void HourEvent()
        {
            if (!CallingAnotherAliensWave) return;
            if (_hoursUntilSecondWavePassed++ > HoursUntilSecondWave)
            {
                var allProvinces = FindObjectsOfType<Province>();
                var provCount = allProvinces.Count();
                var alienProvs = allProvinces.Count(x => x.Owner == this);

                if (alienProvs * 2 > provCount)
                    FindObjectOfType<GameController>().Generator.StartSecondWave();
                else Destroy(gameObject);
            }

        }

        public void SetupTimeValues(float hourLength)
        {
            //throw new System.NotImplementedException();
        }
    }
}
