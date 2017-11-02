using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Aliens : UnitOwner {
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
           
        }
    }
}
