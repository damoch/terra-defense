using System.Collections.Generic;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;

namespace Assets.TerraDefense.Abstarctions.Factions
{
    public interface ICountryEventsHandler
    {
        void AttackProvince(Province lostProvince, List<Unit> playerUnits);
        void DonateWithUnits(Country orderSubject, List<Unit> playerUnits);
        void FortifyProvince(Province province, List<Unit> playerUnits);
        void HandleProvinceUnderAttack(Province provinceUnderAttack, List<Unit> playerUnits);
        void HandleProvinceWithEnemiesNear(Province provinceWithEnemiesNear);
        void CheckBattleGroups(List<Unit> playerUnits);
    }
}