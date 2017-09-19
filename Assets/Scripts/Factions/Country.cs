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
        private Dictionary<Province, int> _provincesUnderAttack;
        private Dictionary<Province, int> _threatenedProvinces;
        private void Start () {
            _provincesUnderAttack = new Dictionary<Province, int>();
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
            if (AvaibleUnits[0].Cost <= Credits)
            {
                Credits -= AvaibleUnits[0].Cost;
                var instance = Instantiate(AvaibleUnits[0].gameObject, spawnPosition, Quaternion.identity);
                instance.GetComponent<Unit>().Owner = this;
                return instance;
            }
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
            var provinceUnderAttack = GetProvinceWithHighestValue(_provincesUnderAttack);
            var provinceWithEnemiesNear = GetProvinceWithHighestValue(_threatenedProvinces);
            if (provinceUnderAttack != null)
            {
                var attackStrength = provinceUnderAttack.EnemyUnits.Sum(a => a.AttackValue);
                if (provinceUnderAttack.DefenseValue <= attackStrength &&
                    GetPlayerControllableUnits().Sum(a => a.DefenceValue) < attackStrength)
                {
                    var retreatProvince = UtilsAndTools.FindNearestProvince(provinceUnderAttack, this);
                    foreach (var alliedUnit in provinceUnderAttack.AlliedUnits)
                    {
                        alliedUnit.SetNewTarget(retreatProvince.GetRandomPosition());
                    }
                    ProduceUnit(retreatProvince.GetRandomPosition());
                    return;
                }
                else
                {
                    var playerUnits = GetPlayerControllableUnits();
                    var avgDist = UtilsAndTools.FindAverageDistance(provinceUnderAttack, playerUnits);

                }
            }

            if (provinceWithEnemiesNear != null)
            {
                var supporter = UtilsAndTools.FindNearestProvince(provinceWithEnemiesNear, this);
                foreach (var supporterAlliedUnit in supporter.AlliedUnits)
                {
                    supporterAlliedUnit.SetNewTarget(provinceWithEnemiesNear.GetRandomPosition());
                }
                ProduceUnit(provinceWithEnemiesNear.GetRandomPosition());
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
