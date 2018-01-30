using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.TerraDefense.Abstractions.Factions;
using Assets.TerraDefense.Enums;
using Assets.TerraDefense.Implementations.Factions;
using Assets.TerraDefense.Implementations.Units;
using Assets.TerraDefense.Implementations.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Editor
{
    public class BattleHandlerTest
    {
        public GameObject Country;
        public GameObject Aliens;
        public GameObject Alliance;
        public GameObject Province;

        private Unit CreateUnit(UnitOwner owner, UnitType type, float def, float att, float airAtt, float status)
        {
            var gameObjec = new GameObject();
            var result = gameObjec.AddComponent<Unit>();
            result.AirAttackValue = airAtt;
            result.Owner = owner;
            result.UnitType = type;
            result.DefenceValue = def;
            result.AttackValue = att;
            result.Status = status;
            return result;
        }

       // [Test]
        public void HumanVictoryTest() {
            var country = new Country();
            var aliens = new Aliens();
            var alliance = new Alliance(); 
            country.Alliance = alliance;
            alliance.Countries = new List<Country> {country};
            var alliedUnits = new List<Unit>()
            {
                CreateUnit(country, UnitType.Ground, 5,15,0, 10),
                CreateUnit(country, UnitType.Ground, 5,15,0, 10),
                CreateUnit(country, UnitType.Ground, 5,15,0, 10),
                CreateUnit(country, UnitType.Ground, 5,15,0, 10),
                CreateUnit(country, UnitType.Ground, 5,15,0, 10)
            };

            var enemyUnits = new List<Unit>()
            {
                CreateUnit(aliens, UnitType.Ground, 5,1,0, 1),
                CreateUnit(aliens, UnitType.Ground, 5,1,0, 1),

            };
           
            var province = new Province
            {
                Owner = country,
                
            };
            country.Name = "c1";
            aliens.Name = "aliens";
            var battleHandler = new BattleHandler();
            var winner = battleHandler.SetSkirmishResult(alliedUnits, enemyUnits);
            //Assert.AreEqual(winner.Name, country.Name);

        }
    }
}
