using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Controllers;
using Assets.TerraDefense.Implementations.Data;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.Factions
{
    public class Country : UnitOwner, ITimeAffected, ISaveLoad {
        public Alliance Alliance;
        private Dictionary<Province, int> _provincesUnderAttack;
        private Dictionary<Province, int> _threatenedProvinces;
        private readonly CountryEventsHandler _handler;
        public int EnemyCloseToProvincePanicValue;
        public int EnemyAttackingProvincePanicValue;
        public int ProvinceLostPanic;
        public int PanicLevel { get
            {
                return _panicLevel;
            } set
            {
                if (_panicLevel + value < 0)
                    _panicLevel = 0;
                else
                    _panicLevel = value;
            }
        }
        public bool PanicEffect { get { return PanicLevel > Alliance.AveragePanic; } }

        public bool IsSetUp
        {
            get
            {
                return _isSetUp;
            }

            set
            {
                _isSetUp = value;
            }
        }

        private string _allianceName;
        public Action OnStatusUpdate;
        private int _hourNumber;
        public int TaxHour;
        public float TaxValue;
        private int _panicLevel;
        public int MinimalUnitCount;
        public int MinimalUnitCountOffset;
        private bool _isSetUp;

        public Country()
        {
            _handler = new CountryEventsHandler(this);
        }

        private void Start () {
            if(Alliance == null)
            {
                Alliance = (Alliance)GetByName(_allianceName);
            }
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
            return Alliance != null && Alliance.IsEnemy(unit);
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
            var instance = GameController.GetUnitInstance(unit.gameObject, spawnPosition);
            instance.GetComponent<Unit>().Owner = this;
            PanicLevel -= (int)(instance.GetComponent<Unit>().DefenceValue / 10);
            var trigger = Instantiate(UnitTriggerObject, spawnPosition, Quaternion.identity);
            trigger.transform.parent = instance.transform;
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
                OnStatusUpdate?.Invoke();
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
                OnStatusUpdate?.Invoke();
            }
            _threatenedProvinces[province]++;
        }

        public void HourEvent()
        {
            _hourNumber++;
            if (_hourNumber > TaxHour)
            {
                _hourNumber = 0;
                if (!PanicEffect)
                {
                    Alliance.PayTax(Credits * (TaxValue / 100));
                }
            }
            CheckProvincesStatus();
            var playerUnits = GetPlayerControllableUnits();
            var provinceUnderAttack = GetProvinceWithHighestValue(_provincesUnderAttack);
            var provinceWithEnemiesNear = GetProvinceWithHighestValue(_threatenedProvinces);
            var lostProvince = LostProvinces.Count > 0 ? LostProvinces[0] : null;

            if (playerUnits.Count() < MinimalUnitCount + UnityEngine.Random.Range(0, MinimalUnitCountOffset))
            {
                ProduceUnit(Utils.UtilsAndTools.FindNearestProvince(this).transform.position);
                return;
            }

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
            if (!LostProvinces.Any()) PanicLevel -= ProvinceLostPanic * 2;
        }

        private Province GetProvinceWithHighestValue(Dictionary<Province,int> dictToSearch)
        {
            return dictToSearch.Keys.Count == 0 ? null : dictToSearch.First(a => a.Value == dictToSearch.Values.Max()).Key;
        }

        public void SetupTimeValues(float seconds)
        {
            _isSetUp = true;
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
                OnStatusUpdate?.Invoke();
            }
        }

        public override void PropertyChangesOwner(Province province, bool isLost)
        {
            Debug.Log("provs left" + FindObjectsOfType<Province>().Where(x => x.Owner == this).Count());
            if (FindObjectsOfType<Province>().Where(x => x.Owner == this).Count() == 0)
            {
                Alliance.Countries.Remove(this);
                var units = FindObjectsOfType<Unit>().Where(x => x.Owner == this).ToList();
                for (var index = 0; index < units.Count; index++)
                {
                    var unit = units[index];
                    GameController.RemoveUnit(unit.gameObject);
                }
                Destroy(gameObject);
            }

            if(isLost) LostProvinces.Add(province);
            else LostProvinces.Remove(province);

            PanicLevel += isLost ? ProvinceLostPanic : -ProvinceLostPanic;
            OnStatusUpdate?.Invoke();
            if (_threatenedProvinces.Keys.Contains(province)) _threatenedProvinces.Remove(province);
            if (_provincesUnderAttack.Keys.Contains(province)) _provincesUnderAttack.Remove(province);
        }

        public override Dictionary<string, string> GetSavableData()
        {
            var dictionary = base.GetSavableData();
            dictionary.Add("panic", PanicLevel.ToString());
            dictionary.Add("alliance", Alliance.Name);
            return dictionary;
        }

        public override void SetSavableData(Dictionary<string, string> json)
        {
            base.SetSavableData(json);
            PanicLevel = int.Parse(json["panic"]);
            _allianceName = json["alliance"]; 
        }


        public void ReceiveInternationalHelp(int value)
        {
            var panicDropValue = PanicLevel * (float)value / Credits;
            Credits += value;
            PanicLevel -= (int)panicDropValue;
            OnStatusUpdate();
        }
    }
}
