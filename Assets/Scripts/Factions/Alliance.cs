using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Factions
{
    public class Alliance : UnitOwner, ITimeAffected
    {
        public string Name;
        public List<Country> Countries;
        private void Start () {
		
        }

        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() == typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            throw new NotImplementedException();
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            throw new NotImplementedException();
        }

        public void HourEvent()
        {
            Debug.Log(Name);
        }

        public void SetupTimeValues()
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsAttackingProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsCloseToProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }

        public override void EnemyIsRetreatingFromProperty(GameObject caller)
        {
            throw new NotImplementedException();
        }
    }
}
