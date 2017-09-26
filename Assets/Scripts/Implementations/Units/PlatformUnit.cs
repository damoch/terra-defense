﻿using System.Collections.Generic;
using Assets.Scripts.Implementations.Factions;
using Assets.Scripts.Implementations.World;
using UnityEngine;

namespace Assets.Scripts.Implementations.Units
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
            GetComponent<SpriteRenderer>().color = AliensOwner.Color;
        }

        private void DecideNextMove()
        {
            if (ShouldBuildMoreUnits())
            {
                Units.Add(AliensOwner.ProduceUnit(transform.position));
            }
            else if(!ShouldMove())
            {
                for (var i = 0; i < Units.Count; i++)
                {
                    var unit = Units[i];
                    if (unit != null)
                    {
                        unit.GetComponent<Unit>().SetNewTarget(TargetProvince.GetRandomPosition());
                    }
                    else
                    {
                        Units.RemoveAt(i);
                    }
                }
            }
        }

        private bool ShouldBuildMoreUnits()
        {
            var sum = 0f;
            for (var i = 0; i < Units.Count; i++)
            {
                var unit = Units[i];
                if (unit != null)
                {
                    sum += unit.GetComponent<Unit>().AttackValue;
                }
                else
                {
                    Units.RemoveAt(i);
                }
            }
            return TargetProvince.DefenseValue + 1 > sum && !ShouldMove();
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