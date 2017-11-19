using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Aliens : UnitOwner {
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
            var friends = pu.Units;

            var enemyAircraftsCount = enemies.Count(x => x.UnitType == UnitType.Air);

            var totalAirAttack = friends.Sum(x => x.AirAttackValue);

            if (totalAirAttack < enemyAircraftsCount * AvgAirAttackVal)
            {
                var airUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Air).ToList();
                airUnits.Sort((x,y) => x.AirAttackValue.CompareTo(y.AirAttackValue));
                instance = Instantiate(airUnits[0].gameObject, spawnPosition, Quaternion.identity);
            }
            else
            {
                var groundUnits = AvaibleUnits.Where(x => x.UnitType == UnitType.Ground).ToList();
                groundUnits.Sort((x,y) => x.AttackValue.CompareTo(y.AttackValue));
                instance = Instantiate(groundUnits[0].gameObject, spawnPosition, Quaternion.identity);
            }
            instance.GetComponent<Unit>().Owner = this;
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
            throw new System.NotImplementedException();
        }
    }
}
