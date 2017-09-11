using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Factions
{
    public class Country : UnitOwner, ITimeAffected {
        public string Name;
        public Alliance Alliance;
        private Dictionary<Province, int> _threatenedProvinces;
        private void Start () {
            _threatenedProvinces = new Dictionary<Province, int>();
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
            var instance = Instantiate(AvaibleUnits[0].gameObject, spawnPosition, Quaternion.identity);
            instance.GetComponent<Unit>().Owner = this;
            return instance;
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if(province == null)return;

            if (!_threatenedProvinces.ContainsKey(province))
            {
                _threatenedProvinces.Add(province,0);
            }
            _threatenedProvinces[province]++;
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            throw new System.NotImplementedException();
        }

        public void HourEvent()
        {
            var provinceInNeed = GetProvinceThatNeedsHelp();
            if (provinceInNeed != null)
            {
                var attackStrength = provinceInNeed.EnemyUnits.Sum(a => a.AttackValue);
                if (provinceInNeed.DefenseValue <= attackStrength && GetPlayerControllableUnits().Sum(a => a.DefenceValue) < attackStrength)
                {
                    var retreatProvince = UtilsAndTools.FindNearestProvince(provinceInNeed, this);
                    ProduceUnit(retreatProvince.GetRandomPosition());
                    foreach (var alliedUnit in provinceInNeed.AlliedUnits)
                    {
                        alliedUnit.SetNewTarget(retreatProvince.GetRandomPosition());
                    }
                }
            }
        }

        private Province GetProvinceThatNeedsHelp()
        {
            return _threatenedProvinces.Keys.Count == 0 ? null : _threatenedProvinces.First(a => a.Value == _threatenedProvinces.Values.Max()).Key;
        }

        public void SetupTimeValues()
        {
            throw new System.NotImplementedException();
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if (province == null || !_threatenedProvinces.ContainsKey(province)) return;
            _threatenedProvinces[province]--;

            if (_threatenedProvinces[province] <= 0)
            {
                _threatenedProvinces.Remove(province);
            }
        }
    }
}
