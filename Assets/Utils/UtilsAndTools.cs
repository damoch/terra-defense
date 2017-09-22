using System.Linq;
using Assets.Scripts.Factions;
using Assets.Scripts.World;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Utils
{
    public class UtilsAndTools : MonoBehaviour {
        public static Province FindNearestProvince(MonoBehaviour caller, UnitOwner owner)
        {
            var possibleTargets = FindObjectsOfType<Province>().Where(p => p.Owner.Equals(owner)).ToList();
            return FindNearestProvince(caller, possibleTargets);
        }

        public static Province FindNearestProvince(MonoBehaviour caller, List<Province> possibleTargets)
        {
            if (possibleTargets.Count == 0) return null;
            var currentTarget = possibleTargets[0];
            var currentDist = Vector2.Distance(caller.transform.position, currentTarget.transform.position);
            foreach (var targetOption in possibleTargets)
            {
                if (!(Vector2.Distance(caller.transform.position, targetOption.transform.position) < currentDist) || caller.Equals(targetOption)) continue;
                currentDist = Vector2.Distance(caller.transform.position, targetOption.transform.position);
                currentTarget = targetOption;
            }
            return currentTarget;
        }

        public static float FindAverageDistance(MonoBehaviour destination, List<MonoBehaviour> sources)
        {
            var distances = new List<float>();
            foreach (var source in sources)
            {
                distances.Add(Vector2.Distance(destination.transform.position, source.transform.position));
            }
            return distances.Average();
        }

        public static float FindAverageDistance(MonoBehaviour destination, List<Unit> sources)//Unit implementuje interfejs, więc nie jest kompatybilny z MonoBehavior
        {
            var newSources = sources.Cast<MonoBehaviour>().ToList();
            return FindAverageDistance(destination, newSources);
        }
    }

}
