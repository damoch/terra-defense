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
    public class Country : UnitOwner, ITimeAffected {
        public string Name;
        public Alliance Alliance;
        private Dictionary<Province, int> _provincesUnderAttack;
        private Dictionary<Province, int> _threatenedProvinces;
        private readonly CountryEventsHandler _handler;
        public int EnemyCloseToProvincePanicValue;
        public int EnemyAttackingProvincePanicValue;
        public int ProvinceLostPanic;
        public int PanicLevel { get; set; }
        public bool PanicEffect { get { return PanicLevel > Alliance.AveragePanic; } }
        public Country()
        {
            _handler = new CountryEventsHandler(this);
        }

        private void Start () {
            _provincesUnderAttack = new Dictionary<Province, int>();
            _threatenedProvinces = new Dictionary<Province, int>();
            LostProvinces = new List<Province>();
        }

        public bool ReceiveOrder(Order order)
        {
            var playerUnits = GetPlayerControllableUnits();
            try
            {
                switch (order.OrderType)
                {
                    case OrderType.AttackProvince:
                        _handler.AttackProvince((Province) order.Subject, playerUnits);
                        break;
                    case OrderType.FortifyProvince:
                        _handler.FortifyProvince((Province) order.Subject, playerUnits);
                        break;
                    case OrderType.SendHelpTo:
                        _handler.DonateWithUnits((Country) order.Subject.gameObject.GetComponent<Province>().Owner,
                            playerUnits);
                        break;
                    case OrderType.None:
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() == typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            return FindObjectsOfType<Unit>().Where(u => u.Owner.Equals(this)).ToList();
        }

        private Unit SelectUnitToProduce()
        {
            Unit result = null;
            foreach (var avaibleUnit in AvaibleUnits)
            {
                if (avaibleUnit.Cost > Credits)
                    return result;
                result = avaibleUnit;
            }
            return result;
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            var unit = SelectUnitToProduce();
            if (!unit)
            {
                Alliance.RequestDonation(this);
                return null;
            }
            Credits -= unit.Cost;
            var instance = Instantiate(unit.gameObject, spawnPosition, Quaternion.identity);
            instance.GetComponent<Unit>().Owner = this;
            return instance;
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            var province = caller.GetComponent<Province>();
            if(province == null)return;

            if (!_provincesUnderAttack.ContainsKey(province))
            {
                _provincesUnderAttack.Add(province,0);
                PanicLevel += EnemyAttackingProvincePanicValue;
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
                PanicLevel += EnemyCloseToProvincePanicValue;
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
                _handler.AttackProvince(lostProvince, playerUnits);
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
            foreach (var lostProvince in LostProvinces)
            {
                if (lostProvince.Owner == this) LostProvinces.Remove(lostProvince);
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
                PanicLevel -= EnemyAttackingProvincePanicValue;
            }
        }

        public override void PropertyChangesOwner(Province province, bool isLost)
        {
            if(isLost) LostProvinces.Add(province);
            else LostProvinces.Remove(province);

            PanicLevel += isLost ? ProvinceLostPanic : -ProvinceLostPanic;

            if (_threatenedProvinces.Keys.Contains(province)) _threatenedProvinces.Remove(province);
            if (_provincesUnderAttack.Keys.Contains(province)) _provincesUnderAttack.Remove(province);
        }
    }
}
