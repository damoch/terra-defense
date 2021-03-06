﻿using System.Collections.Generic;
using System.Linq;
using Assets.TerraDefense.Abstractions.IO;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using UnityEngine;

namespace Assets.TerraDefense.Abstractions.Factions
{
    public abstract class UnitOwner : MonoBehaviour, ISaveLoad
    {
        public Color Color;
        public int Credits;
        public List<Unit> AvaibleUnits;
        public string Name;

        public List<Province> LostProvinces { get; set; }

        public int Priority => 1;
        public GameObject UnitTriggerObject;
        public abstract bool IsEnemy(Unit unit);


        public abstract List<Unit> GetPlayerControllableUnits();

        public abstract GameObject ProduceUnit(Vector2 spawnPosition);

        public abstract void EnemyIsAttackingProperty(GameObject caller);

        public abstract void EnemyIsRetreatingFromProperty(GameObject caller);

        public abstract void EnemyIsCloseToProperty(GameObject caller);

        public abstract void PropertyChangesOwner(Province province, bool isLost);

        public static UnitOwner GetByName(string name)
        {
            var owners = FindObjectsOfType<UnitOwner>();
            return owners.FirstOrDefault(x => x.Name == name);
        }

        public virtual Dictionary<string, string> GetSavableData()
        {
            var resultDict = new Dictionary<string, string>
            {
                { "name", gameObject.name },
                { "countryName", Name },
                { "credits", Credits.ToString() },
                { "colorR", Color.r.ToString() },
                { "colorG", Color.g.ToString() },
                { "colorB", Color.b.ToString() },

            };
            return resultDict;
        }

        public virtual void SetSavableData(Dictionary<string, string> json)
        {
            Name = json["countryName"];
            Credits = int.Parse(json["credits"]);

            var colorR = float.Parse(json["colorR"]);
            var colorG = float.Parse(json["colorG"]);
            var colorB = float.Parse(json["colorB"]);
            Color = new Color(colorR, colorG, colorB);
            
        }
    }
}
