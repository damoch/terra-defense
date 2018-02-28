using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Alliance : UnitOwner, ITimeAffected
    {
        public List<Country> Countries;
        public double AveragePanic { get { return Countries != null && Countries.Count > 0 ? Countries.Average(a => a.PanicLevel) : 0; }  }
        private List<string> _countryNames;
        public delegate void UpdateAllianceFoundsDelegate(float value);
        public UpdateAllianceFoundsDelegate OnFoundsUpdate;
        private void Start () {
		    if(Countries == null || Countries.Count == 0)
            {
                Countries = new List<Country>();
                foreach (var countr in _countryNames)
                {
                    Countries.Add((Country)GetByName(countr));
                }
                _countryNames = null;
            }
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
                var instance = GameController.GetUnitInstance(AvaibleUnits[0].gameObject, spawnPosition);//, Quaternion.identity);
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

        public override Dictionary<string, string> GetSavableData()
        {
            var dictionary =  base.GetSavableData();

            var countries = Countries.Select(x => x.Name).ToList();
            dictionary.Add("countries", JsonConvert.SerializeObject(countries));

            return dictionary;
        }

        public override void SetSavableData(Dictionary<string, string> json)
        {
            base.SetSavableData(json);
            _countryNames = JsonConvert.DeserializeObject<List<string>>(json["countries"]);

        }

        internal void PayTax(float value)
        {
            Credits += (int)value;
            OnFoundsUpdate(Credits);
        }
    }
}
