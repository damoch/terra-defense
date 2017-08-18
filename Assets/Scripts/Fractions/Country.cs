using System.Collections.Generic;
using Assets.Scripts.World;

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
    }
}
