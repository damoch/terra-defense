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

        public bool IsUnitAllied(Unit unitComponent)
        {
            foreach (var country in Countries)
            {
                if (unitComponent.Owner.Equals(country)) return true;
            }
            return false;
        }
    }
}
