using Assets.Scripts.Fractions;
using UnityEngine;

namespace Assets.Scripts.Players
{
    public class AiPlayer : MonoBehaviour {
        public Aliens Aliens { get; set; }
        private void Start () {
		    Aliens = new Aliens();
        }

    }
}
