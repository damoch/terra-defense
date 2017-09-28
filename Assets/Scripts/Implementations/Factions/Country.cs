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
            var playerUnits = GetPlayerControllableUnits();
            var provinceUnderAttack = GetProvinceWithHighestValue(_provincesUnderAttack);
            var provinceWithEnemiesNear = GetProvinceWithHighestValue(_threatenedProvinces);
            if (provinceUnderAttack != null)
            {
                var attackStrength = provinceUnderAttack.EnemyUnits.Sum(a => a.AttackValue);
                if (provinceUnderAttack.DefenseValue <= attackStrength &&
                    playerUnits.Sum(a => a.DefenceValue) < attackStrength)
                {
                    var retreatProvince = UtilsAndTools.FindNearestProvince(provinceUnderAttack, this);
                    foreach (var alliedUnit in provinceUnderAttack.AlliedUnits)
                    {
                        alliedUnit.SetNewTarget(retreatProvince.GetRandomPosition());
                    }
                    var unit = ProduceUnit(retreatProvince.GetRandomPosition());

                    if (retreatProvince.Owner != this)
                    {
                        unit.transform.position = UtilsAndTools.FindNearestProvince(retreatProvince, this).GetRandomPosition();
                    }
                    return;
                }
                else
                {
                    var avgDist = UtilsAndTools.FindAverageDistance(provinceUnderAttack, playerUnits);
                    var hitColliders = Physics2D.OverlapCircleAll(provinceUnderAttack.transform.position, avgDist);

                    foreach (var hitCollider in hitColliders)
                    {
                        try
                        {
                            var unit = hitCollider.gameObject.GetComponent<Unit>();
                            if(!IsEnemy(unit))
                                unit.SetNewTarget(provinceUnderAttack.GetRandomPosition());
                        }
                        catch
                        {
                            //
                        }
                    }

                }
            }

            if (provinceWithEnemiesNear != null)
            {
                var supporter = UtilsAndTools.FindNearestProvince(provinceWithEnemiesNear, this);
                foreach (var supporterAlliedUnit in supporter.AlliedUnits)
                {
                    supporterAlliedUnit.SetNewTarget(provinceWithEnemiesNear.GetRandomPosition());
                }
                var unit = ProduceUnit(provinceWithEnemiesNear.GetRandomPosition());

                if (provinceWithEnemiesNear.Owner != this)
                {
                    unit.transform.position = UtilsAndTools.FindNearestProvince(provinceWithEnemiesNear, this).GetRandomPosition();
                }
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
