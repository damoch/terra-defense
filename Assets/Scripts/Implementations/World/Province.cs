using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abstractions.Factions;
using Assets.Scripts.Implementations.Factions;
using Assets.Scripts.Implementations.Units;
using Assets.Scripts.Interfaces.World;
using UnityEngine;

namespace Assets.Scripts.Implementations.World
{
    public class Province : MonoBehaviour, ITimeAffected
    {
        public float BattleDelay;
        public float AlertDelay { get; set; }
        public List<Unit> EnemyUnits { get; set; }
        public List<Unit> AlliedUnits { get; set; }

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
            var totalAttack = EnemyUnits.Sum(unit => unit.AttackValue);
            var totalDefense = AlliedUnits.Sum(unit => unit.DefenceValue);
            var damageValue = Math.Abs(totalDefense - totalAttack);
            var losingArmy = totalAttack > totalDefense ? AlliedUnits : EnemyUnits;
            var winningArmy = totalAttack <= totalDefense ? AlliedUnits : EnemyUnits;


            for (var i = 0; i < losingArmy.Count; i++)
            {
                try
                {
                    var unit = losingArmy[i];
                    unit.ModifyStatus(-damageValue);
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            damageValue *= 0.33f;

            for (var i = 0; i < winningArmy.Count; i++)
            {
                try
                {
                    var unit = winningArmy[i];
                    unit.ModifyStatus(-damageValue);
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                }
            }

            if (losingArmy.Count != 0) return;

            var proposedOwner = winningArmy.Count > 0 ? winningArmy[0].Owner : null;

            if(proposedOwner == null) return;

            ChangeOwner(proposedOwner, winningArmy);
        }

        private void ChangeOwner(UnitOwner proposedOwner, List<Unit> winningArmy)
        {
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
