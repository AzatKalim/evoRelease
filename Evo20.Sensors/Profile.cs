namespace Evo20.Sensors
{
    public class Profile
    {
        public Position[] PositionArray { get; set; }

        public Profile(Position[] profile)
        {
            PositionArray = profile;
        }
    }
}
