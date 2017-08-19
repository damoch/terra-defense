using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.Players
{
    public class AiPlayer : MonoBehaviour {
        public Aliens Aliens { get; set; }
        private void Start ()
        {
            Aliens = FindObjectOfType<Aliens>();
            Debug.Log(Aliens);
            InvokeRepeating("MakeNextMove",1f,1f);
        }

        private void MakeNextMove()
        {
            Debug.Log("Deciding move");
            Debug.Log(Aliens.GetPlayerControllableUnits());
        }

    }
}
