using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class Province : MonoBehaviour, ITimeAffected, ISaveLoad
    {
        public int IndexX { get; set; }
        public int IndexY { get; set; }
        public float BattleDelay;
        public float AlertDelay;
        public List<Unit> EnemyUnits;
        public List<Unit> AlliedUnits;

        public float DefenseValue
        {
            get
            {
                return AlliedUnits != null ? AlliedUnits.Sum(a => a.DefenceValue) : 0;
            }
        }

        public bool IsBattle { get; set; }

        public BattleHandler BattleHandler { get; set; }

        public int Priority => 2;

        public UnitOwner Owner;
        private UnitOwner _originalOwner;
        public int CreditsPerHour;
        private SpriteRenderer _spriteRenderer;
        private string _originaOwnerName;
        private string _ownerName;
        public delegate void OwnerChangeDelegate();
        public OwnerChangeDelegate OnOwnerChange;
        public Action UiHandle;
        public string Name { get; set; }
        public float AttackValue
        {
            get
            {
                return EnemyUnits != null ? EnemyUnits.Sum(a => a.AttackValue) : 0;
            }
        }

        public float AirAttackValue
        {
            get
            {
                return EnemyUnits != null ? EnemyUnits.Sum(a => a.AirAttackValue) : 0;
            }
        }

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

        public float EnemyEnterDamageFactor;
        public float FriendlyStayRepairFactor;
        public Transform DefensePosition;
        public Transform AttackPosition;
        private Collider2D _provinceBounds;
        public float NewUnitOffset;
        private bool _isSetUp;

        private void Start ()
        {
            _provinceBounds = GetComponent<Collider2D>();
            if (Owner == null) Owner = UnitOwner.GetByName(_ownerName);
            if (_originaOwnerName != null) _originalOwner = UnitOwner.GetByName(_originaOwnerName);
           
            IsBattle = false;
            #if UNITY_EDITOR
            if (BattleDelay == 0)
            {
                BattleDelay = 1f;
            }
            #endif

            _originalOwner = Owner;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = Owner.Color;
            BattleHandler = new BattleHandler();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitEnter(unitComponent);
            }
            if (other.tag != "TriggerZone") return;

            var closeEnemy = other.gameObject.transform.parent.gameObject.GetComponent<Unit>();
            if (closeEnemy && Owner.IsEnemy(closeEnemy))
            {
                Debug.Log("close Enemy!");
                Owner.EnemyIsCloseToProperty(gameObject);
            }
        }

        private void HandleUnitEnter(Unit unitComponent)
        {
            if(AlliedUnits == null)AlliedUnits = new List<Unit>();
            if(EnemyUnits == null)EnemyUnits = new List<Unit>();
            if(AlliedUnits.Contains(unitComponent) || 
                EnemyUnits.Contains(unitComponent))return;
            unitComponent.transform.position = transform.position;
            if (unitComponent.GetType() == typeof(PlatformUnit))
            {
                var unit = (PlatformUnit) unitComponent;
                unit.CurrentProvince = this;
            }
            

            if(Owner == null)Owner = UnitOwner.GetByName(_ownerName);
            if (Owner.IsEnemy(unitComponent))
            {
                //unitComponent.SetNewTarget(GetNewUnitPosition(EnemyUnits.Count(), AttackPosition));
                if (unitComponent.UnitType == Enums.UnitType.Ground && unitComponent.Owner != _originalOwner)
                    unitComponent.ModifyStatus(unitComponent.Status * -EnemyEnterDamageFactor);
                EnemyUnits.Add(unitComponent);
                Owner.EnemyIsAttackingProperty(gameObject);
                if (!AlliedUnits.Any())
                {
                    ChangeOwner(unitComponent.Owner, EnemyUnits);
                    return;
                }
                if (!IsBattle)
                {
                    StartCoroutine("CommenceBattle");
                }
            }
            else 
            {
                //if (_provinceBounds.bounds.Contains(unitComponent.Target)) unitComponent.SetNewTarget(GetNewUnitPosition(AlliedUnits.Count(), DefensePosition));
                AlliedUnits.Add(unitComponent);
            }
            UiHandle?.Invoke();
        }

        public IEnumerator CommenceBattle()
        {
            IsBattle = true;
            yield return new WaitForSeconds(BattleDelay);
            yield return SetSkirmishResult();

            if (!IsBattle) yield break;
            StartCoroutine("CommenceBattle");
        }

        private IEnumerator SetSkirmishResult()
        {   yield return BattleHandler.SetSkirmishResult(AlliedUnits, EnemyUnits);
            var winner = BattleHandler.CurrentWinner;
            if (EnemyUnits.Count == 0 || !winner)yield break;
            var winningArmy = AlliedUnits.Count > 0 ? AlliedUnits : EnemyUnits;
            ChangeOwner(winner, winningArmy);
        }

        private void ChangeOwner(UnitOwner proposedOwner, List<Unit> winningArmy)
        {
            if (_originalOwner == null && proposedOwner.GetType() == typeof(Country))
            {
                _originalOwner = proposedOwner;
            }
            else if (_originalOwner != null)
            {
                _originalOwner.PropertyChangesOwner(this, proposedOwner != _originalOwner);
            }
            Owner = proposedOwner.GetType() == typeof(Aliens) ? proposedOwner : _originalOwner;
            
            UiHandle?.Invoke();
            _spriteRenderer.color = Owner.Color;
            AlliedUnits = winningArmy;
            var i = 0;
            foreach (var un in AlliedUnits)
            {
                un.SetNewTarget(GetNewUnitPosition(i++, DefensePosition));
            }
            EnemyUnits = new List<Unit>();
            IsBattle = false;
            OnOwnerChange?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitExit(unitComponent);
            }
        }

        private void HandleUnitExit(Unit unitComponent)
        {
            var listToRemove = Owner.IsEnemy(unitComponent) ? EnemyUnits : AlliedUnits;
            listToRemove.Remove(unitComponent);
            UiHandle?.Invoke();
        }

        public void HourEvent()
        {
            try
            {
                var country = (Country) Owner;
                if (country.PanicEffect)
                    country.Credits += CreditsPerHour / 2;
                else
                {
                    country.Credits += CreditsPerHour;
                }
                country.OnStatusUpdate?.Invoke();
            }
            catch (InvalidCastException e)
            {
                Owner.Credits += CreditsPerHour / 4;
            }
            if (IsBattle || AlliedUnits.Count == 0 || AlliedUnits[0].Owner == Aliens.Instance) return;
            AlliedUnits.Where(x => x.IsHurt).ToList().ForEach(y => y.ModifyStatus(y.Status * FriendlyStayRepairFactor));
            EnemyUnits.Where(x => 
            x.gameObject.GetComponent<PlatformUnit>() != null && x.gameObject.GetComponent<PlatformUnit>().CurrentProvince == null)
            .ToList().ForEach(y => y.gameObject.GetComponent<PlatformUnit>().CurrentProvince = this);
            AlliedUnits.Where(x => 
            x.gameObject.GetComponent<PlatformUnit>() != null && x.gameObject.GetComponent<PlatformUnit>().CurrentProvince == null)
            .ToList().ForEach(y => y.gameObject.GetComponent<PlatformUnit>().CurrentProvince = this);
            Owner.Credits += CreditsPerHour;
            if (AttackValue == 0 && AirAttackValue == 0) IsBattle = false;
        }

        public void SetupTimeValues(float seconds)
        {
            AlertDelay = seconds / 4;
            BattleDelay = seconds / 4;
            _isSetUp = true;
        }

        public Dictionary<string, string> GetSavableData()
        {
            return new Dictionary<string, string>
            {
                { "name", gameObject.name },
                { "originalOwner", _originalOwner.Name },
                { "owner", Owner.Name },
                { "position", JsonConvert.SerializeObject(transform.position) },
            };
        }

        public void SetSavableData(Dictionary<string, string> json)
        {
            _originaOwnerName = json["originalOwner"];
            _ownerName = json["owner"];
            transform.position = JsonConvert.DeserializeObject<Vector3>(json["position"]);
        }

        public static List<Province> FindProvincesFor(UnitOwner owner)
        {
            var all = FindObjectsOfType<Province>().ToList();
            if (owner == null) return all;
            return all.Where(x => x.Owner == owner).ToList();
        }

        private Vector3 GetNewUnitPosition(int unitCount, Transform originPoint)
        {
            var newPos = new Vector3(originPoint.position.x + NewUnitOffset * unitCount, originPoint.position.y);
            if(!_provinceBounds.bounds.Contains(newPos)) return new Vector3(originPoint.position.x + NewUnitOffset, originPoint.position.y);
            return newPos;
        }
    }
}
