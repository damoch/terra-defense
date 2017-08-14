using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class UnitOwner : MonoBehaviour {
        public List<Unit> Units { get; set; }
        public abstract void AddUnit(Unit unit);
        public abstract void RemoveUnit(Unit unit);
    }
}
