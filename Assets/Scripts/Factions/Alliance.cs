using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Factions
{
    public class Alliance : UnitOwner, ITimeAffected
    {
        public string Name;
        public List<Country> Countries;
        private void Start () {
		
        }

        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() == typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            //Czy sojusz powinien mieć jednostki?
            return FindObjectsOfType<Unit>().Where(u => u.Owner.Equals(this)).ToList();
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            if (AvaibleUnits[0].Cost <= Credits)
            {
                Credits -= AvaibleUnits[0].Cost;
                var instance = Instantiate(AvaibleUnits[0].gameObject, spawnPosition, Quaternion.identity);
                instance.GetComponent<Unit>().Owner = this;
                return instance;
            }
            return null;
        }

        public void HourEvent()
        {
            Debug.Log(Name);
        }

        public void SetupTimeValues()
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }

        public void RequestDonation(Country country)
        {
            Debug.Log(country.Name + " is requesting donation");
        }
    }
}
