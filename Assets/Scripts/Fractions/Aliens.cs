using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World;

namespace Assets.Scripts.Fractions
{
    public class Aliens : UnitOwner {
        public List<Unit> PlatformUnits { get; set; }
        private void Start()
        {
            //PlatformUnits = FindObjectsOfType<PlatformUnit>().ToList();
        }

        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() != typeof(Aliens);
        }

        public override List<Unit> GetPlayerControllableUnits()
        {
            var result = new List<Unit>();
            var units =  FindObjectsOfType<PlatformUnit>().ToList();
            foreach (var platformUnit in units)
            {
                result.Add(platformUnit);
            }
            return result;
        }
    }
}
