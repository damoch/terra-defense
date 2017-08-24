using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class PlatformUnit : Unit {
        public Aliens AliensOwner { get; set; }
        public List<GameObject> Units { get; set; }
        public Province TargetProvince { get; set; }
        public override void Start()
        {
            TargetProvince = null;
            Target = transform.position;
            Units = new List<GameObject>();
            Target = transform.position;
            AliensOwner = FindObjectOfType<Aliens>();
            if (AliensOwner == null)
            {
                throw new UnityException("No Aliens on scene!");
            }
            InvokeRepeating("DecideNextMove", 1f, 1f);
            SetupTimeValues();
        }

        private void DecideNextMove()
        {
            if (!ShouldMove() && Units.Count < 3)
            {
                Units.Add(AliensOwner.ProduceUnit(transform.position));
            }
            else if (TargetProvince == null || TargetProvince.Owner.Equals(AliensOwner))
            {
                FindTargetProvince();
            }
            else
            {
                for (var i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    if (unit != null)
                    {
                        unit.GetComponent<Unit>().SetNewTarget(TargetProvince.transform.position);
                    }
                    else
                    {
                        Units.RemoveAt(i);
                    }
                }
            }
        }

        private void FindTargetProvince()
        {
            var targetOptions = FindObjectsOfType<Province>().Where(t => !t.Owner.Equals(AliensOwner)).ToList();

            TargetProvince = targetOptions[0];
            var currentDist = Vector2.Distance(transform.position, TargetProvince.transform.position);
            foreach (var targetOption in targetOptions)
            {
                if (Vector2.Distance(transform.position, targetOption.transform.position) < currentDist)
                {
                    currentDist = Vector2.Distance(transform.position, targetOption.transform.position);
                    TargetProvince = targetOption;
                }
            }

        }

        public override void Update()
        {
            if (ShouldMove())
            {
                MoveTowardsTarget();
            }
        }

    }
}
