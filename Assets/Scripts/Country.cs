using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Country : UnitOwner {
        public string Name;
        public void Start () {
		
        }
	
        public override void AddUnit(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUnit(Unit unit)
        {
            throw new System.NotImplementedException();
        }
    }
}
