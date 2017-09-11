using System.Linq;
using Assets.Scripts.Factions;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Utils
{
    public class UtilsAndTools : MonoBehaviour {
        public static Province FindNearestProvince(Province caller, UnitOwner owner)
        {
            var possibleRetreatTargets = FindObjectsOfType<Province>().Where(p => p.Owner.Equals(owner)).ToList();
            var currentTarget = possibleRetreatTargets[0];
            var currentDist = Vector2.Distance(caller.transform.position, currentTarget.transform.position);
            foreach (var targetOption in possibleRetreatTargets)
            {
                if (!(Vector2.Distance(caller.transform.position, targetOption.transform.position) < currentDist) || caller.Equals(targetOption)) continue;
                currentDist = Vector2.Distance(caller.transform.position, targetOption.transform.position);
                currentTarget = targetOption;
            }
            return currentTarget;
        }
    }
}
