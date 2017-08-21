using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Fractions
{
    public class Country : UnitOwner {
        public string Name;
        public Alliance Alliance;
        private void Start () {
        }

        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() == typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            throw new System.NotImplementedException();
        }

        public override GameObject ProduceUnit(Vector2 spawnPosition)
        {
            throw new System.NotImplementedException();
        }
    }
}
