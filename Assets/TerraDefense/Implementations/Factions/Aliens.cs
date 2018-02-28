using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Aliens : UnitOwner, ISaveLoad {
        public static Aliens Instance;
        private List<Unit> _airUnits;
        private List<Unit> _groundUnits;
        private void Start()
        {
            Instance = this;

            _airUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Air).ToList();
            _groundUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Ground).ToList();
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
            var instance = Instantiate(AvaibleUnits[0].gameObject, spawnPosition, Quaternion.identity);
            instance.GetComponent<Unit>().Owner = this;
            return instance;
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
                _airUnits.Sort((x,y) => x.AirAttackValue.CompareTo(y.AirAttackValue));
                instance = GameController.GetUnitInstance(_airUnits.First(x => x.Cost <= Credits).gameObject, spawnPosition);//, Quaternion.identity);
            }
            else if(_groundUnits.Exists(x => x.Cost <= Credits))
            {
                _groundUnits.Sort((x,y) => x.AttackValue.CompareTo(y.AttackValue));
                instance = GameController.GetUnitInstance(_groundUnits.First(x => x.Cost <= Credits).gameObject, spawnPosition);//Instantiate(_groundUnits.First(x => x.Cost <= Credits).gameObject, spawnPosition, Quaternion.identity);
            }
            else
            {
                return null;
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
    }
}
