using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class Province : MonoBehaviour, ITimeAffected, ISaveLoad
    {
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
        private void Start ()
        {
            if (Owner == null) Owner = UnitOwner.GetByName(_ownerName);
            if (_originaOwnerName != null) _originalOwner = UnitOwner.GetByName(_originaOwnerName);
            SetupTimeValues();
           
            IsBattle = false;
            #if UNITY_EDITOR
            if (BattleDelay == 0)
            {
                BattleDelay = 1f;
            }
            #endif

            _originalOwner = Owner;
            InvokeRepeating("CheckSurrondings", AlertDelay, AlertDelay);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = Owner.Color;
            BattleHandler = new BattleHandler();
        }

        private void CheckSurrondings()
        {
            //StartCoroutine(ExecuteCheck());
            NeighborhoodChecker.AddJobToQueue(this);
        }

        private IEnumerator ExecuteCheck()
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, 100);

            foreach (var hitCollider in hitColliders)
            {
                var unit = hitCollider.gameObject.GetComponent<Unit>();
                try
                {
                    if (unit != null && Owner.IsEnemy(unit))
                    {
                        Owner.EnemyIsCloseToProperty(gameObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var unitComponent = other.gameObject.GetComponent<Unit>();
            if (unitComponent != null)
            {
                HandleUnitEnter(unitComponent);
            }
        }

        private void HandleUnitEnter(Unit unitComponent)
        {
            if(AlliedUnits == null)AlliedUnits = new List<Unit>();
            if(EnemyUnits == null)EnemyUnits = new List<Unit>();
            if(AlliedUnits.Contains(unitComponent) || 
                EnemyUnits.Contains(unitComponent))return;

            if (unitComponent.GetType() == typeof(PlatformUnit))
            {
                var unit = (PlatformUnit) unitComponent;
                unit.CurrentProvince = this;
            }

            if (Owner.IsEnemy(unitComponent))
            {
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
                AlliedUnits.Add(unitComponent);
            }
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

            //if (_originalOwner == null && proposedOwner.GetType() == typeof(Country))
            //{
            //    _originalOwner = proposedOwner;
            //}
            //Owner = _originalOwner == null && proposedOwner.GetType() == typeof(Aliens) ? proposedOwner;
            _spriteRenderer.color = Owner.Color;
            AlliedUnits = winningArmy;
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
            if (unitComponent.GetType() == typeof(PlatformUnit))
            {
                var unit = (PlatformUnit)unitComponent;
                unit.CurrentProvince = null;
            }

            var listToRemove = Owner.IsEnemy(unitComponent) ? EnemyUnits : AlliedUnits;
            listToRemove.Remove(unitComponent);
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
                Owner.Credits += CreditsPerHour;
            }
           // Owner.Credits += CreditsPerHour;
        }

        public void SetupTimeValues()
        {
            AlertDelay = FindObjectOfType<Clock>().LengthOfHour / 4;
        }

        public Dictionary<string, string> GetSavableData()
        {
            return new Dictionary<string, string>
            {
                { "name", gameObject.name },
                { "type", GetType().FullName  },
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
    }
}
