using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abstractions.Factions;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Implementations.Utils;
using Assets.Scripts.Implementations.World;
using Assets.Scripts.Interfaces.World;
using UnityEngine;

namespace Assets.Scripts.Implementations.Factions
{
    public class Country : UnitOwner, ITimeAffected {
        public string Name;
        public Alliance Alliance;
        private Dictionary<Province, int> _provincesUnderAttack;
        private Dictionary<Province, int> _threatenedProvinces;
        private readonly CountryEventsHandler _handler;

        public Country()
        {
            _handler = new CountryEventsHandler(this);
        }

        public CountryEventsHandler CountryEventsHandler
        {
            get { return _handler; }
        }

        private void Start () {
            _provincesUnderAttack = new Dictionary<Province, int>();
            _threatenedProvinces = new Dictionary<Province, int>();
            LostProvinces = new List<Province>();
        }

        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() == typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            return FindObjectsOfType<Unit>().Where(u => u.Owner.Equals(this)).ToList();
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            if (AvaibleUnits[0].Cost <= Credits)
            {
                Credits -= AvaibleUnits[0].Cost;
                var instance = Instantiate(AvaibleUnits[0].gameObject, spawnPosition, Quaternion.identity);
                instance.GetComponent<Unit>().Owner = this;
                Debug.Log(instance.GetComponent<Unit>().Owner);
                Debug.Log(this);
                return instance;
            }
            Alliance.RequestDonation(this);
            return null;
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if(province == null)return;

            if (!_provincesUnderAttack.ContainsKey(province))
            {
                _provincesUnderAttack.Add(province,0);
            }
            _provincesUnderAttack[province]++;
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if (province == null) return;

            if (!_threatenedProvinces.ContainsKey(province))
            {
                _threatenedProvinces.Add(province, 0);
            }
            _threatenedProvinces[province]++;
        }

        public void HourEvent()
        {
            CheckProvincesStatus();
            var playerUnits = GetPlayerControllableUnits();
            var provinceUnderAttack = GetProvinceWithHighestValue(_provincesUnderAttack);
            var provinceWithEnemiesNear = GetProvinceWithHighestValue(_threatenedProvinces);
            var lostProvince = LostProvinces.Count > 0 ? LostProvinces[0] : null;

            if (lostProvince != null)
            {
                _handler.HandleLostProvince(lostProvince, playerUnits);
                return;
            }
             if (provinceUnderAttack != null)
            {
                _handler.HandleProvinceUnderAttack(provinceUnderAttack, playerUnits);
                return;
            }

            if (provinceWithEnemiesNear != null) 
                _handler.HandleProvinceWithEnemiesNear(provinceWithEnemiesNear);
            
        }

        private void CheckProvincesStatus()
        {
            foreach (var i in _provincesUnderAttack.Keys)
            {
                if (i.Owner == this) _provincesUnderAttack.Remove(i);
            }

            foreach (var i in _threatenedProvinces.Keys)
            {
                if (i.Owner == this) _provincesUnderAttack.Remove(i);
            }
        }

        private Province GetProvinceWithHighestValue(Dictionary<Province,int> dictToSearch)
        {
            return dictToSearch.Keys.Count == 0 ? null : dictToSearch.First(a => a.Value == dictToSearch.Values.Max()).Key;
        }

        public void SetupTimeValues()
        {
            //throw new System.NotImplementedException();
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if (province == null || !_provincesUnderAttack.ContainsKey(province)) return;
            _provincesUnderAttack[province]--;

            if (_provincesUnderAttack[province] <= 0)
            {
                _provincesUnderAttack.Remove(province);
            }
        }
    }
}
