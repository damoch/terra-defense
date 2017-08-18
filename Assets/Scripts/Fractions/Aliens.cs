using Assets.Scripts.World;

namespace Assets.Scripts.Fractions
{
    public class Aliens : UnitOwner {
        public override bool IsEnemy(Unit unit)
        {
            return unit.Owner.GetType() != typeof(Aliens);
        }

    }
}
