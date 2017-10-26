using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Alliance : UnitOwner, ITimeAffected
    {
        public string Name;
        public List<Country> Countries;
        public double AveragePanic { get { return Countries.Average(a => a.PanicLevel); }  }
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
            //throw new NotImplementedException();
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            //throw new NotImplementedException();
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            //throw new NotImplementedException();
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            //throw new NotImplementedException();
        }

        public void RequestDonation(Country country)
        {
            Debug.Log(country.Name + " is requesting donation");
        }

        public override void PropertyChangesOwner(Province province, bool isLost)
        {
            //throw new NotImplementedException();
        }

        public void SendCommandToCountry(OrderType orderType, MonoBehaviour subject, Country handledCountry)
        {
            var command = new Order
            {
                OrderType = orderType,
                Subject = subject
            };
            handledCountry.ReceiveOrder(command);
        }

    }
}
