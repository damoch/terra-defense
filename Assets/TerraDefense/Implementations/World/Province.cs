using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Abstractions.World;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using UnityEngine;

namespace Assets.TerraDefense.Implementations.World
{
    public class Province : MonoBehaviour, ITimeAffected
    {
        public float BattleDelay;
        public float AlertDelay { get; set; }
        public List<Unit> EnemyUnits { get; set; }
        public List<Unit> AlliedUnits { get; set; }
        public bool IsTerraformed { get; set; }
        public int TimeOccupied { get; set; }

        private BattleHandler _battleHandler;

        public float DefenseValue
        {
            get
            {
                return AlliedUnits != null ? AlliedUnits.Sum(a => a.DefenceValue) : 0;
            }
        }

        public bool IsBattle { get; set; }
        public UnitOwner Owner;
        private UnitOwner _originalOwner;
        public int CreditsPerHour;
        private SpriteRenderer _spriteRenderer;
        private void Start ()
        {
            SetupTimeValues();

            TimeOccupied = 0;
            IsBattle = false;
            EnemyUnits = new List<Unit>();
            AlliedUnits = new List<Unit>();
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
            _battleHandler = new BattleHandler();
        }

        private void CheckSurrondings()
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
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

            }
            
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
            if(AlliedUnits.Contains(unitComponent) || EnemyUnits.Contains(unitComponent))return;

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
            SetSkirmishResult();

            if (IsBattle)
            {
                Debug.Log("Battle rages on");
                StartCoroutine("CommenceBattle");
            }

        }

        private void SetSkirmishResult()
        {
            var winner = _battleHandler.SetSkirmishResult(AlliedUnits, EnemyUnits, IsTerraformed);
            if (EnemyUnits.Count == 0 || !winner)return;
            var winningArmy = AlliedUnits.Count > 0 ? AlliedUnits : EnemyUnits;
            ChangeOwner(winner, winningArmy);
        }

        private void ChangeOwner(UnitOwner proposedOwner, List<Unit> winningArmy)
        {
            TimeOccupied = 0;
            Owner = proposedOwner.GetType() == typeof(Aliens) ? proposedOwner : _originalOwner;
            _originalOwner.PropertyChangesOwner(this, Owner != _originalOwner);
            _spriteRenderer.color = Owner.Color;
            AlliedUnits = winningArmy;
            EnemyUnits = new List<Unit>();
            IsBattle = false;
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
            TimeOccupied++;
            try
            {
                var country = (Country) Owner;
                if (country.PanicEffect)
                    country.Credits += CreditsPerHour / 2;
                else
                {
                    country.Credits += CreditsPerHour;
                }
            }
            catch (InvalidCastException e)
            {
                Owner.Credits += CreditsPerHour;
            }
            Owner.Credits += CreditsPerHour;
        }

        public void SetupTimeValues()
        {
            AlertDelay = FindObjectOfType<Clock>().LengthOfHour / 4;
        }
    }
}
