using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Fractions
{
    public class Alliance : UnitOwner
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
    }
}
