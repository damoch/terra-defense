namespace Assets.Scripts.World
{
    public class PlatformUnit : Unit {
        public override void Start()
        {
            Target = transform.position;
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
