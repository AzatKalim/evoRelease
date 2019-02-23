namespace Evo20.Sensors
{
    public class Profile
    {
        public ProfilePart[] ProfilePartArray { get; set; }

        public Profile(ProfilePart[] profile)
        {
            ProfilePartArray = profile;
        }
    }
}
