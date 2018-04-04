namespace Assets.TerraDefense.Abstractions.World
{
    public interface ITimeAffected
    {
        void HourEvent();
        void SetupTimeValues(float hourLength);
        bool IsSetUp { get; set; }
    }
}
