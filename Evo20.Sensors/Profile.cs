using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo20.Sensors
{
    public class Profile
    {
        private ProfilePart[] profile;

        public ProfilePart[] ProfilePartArray
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
            }
        }

        public Profile(ProfilePart[] profile)
        {
            this.profile = profile;
        }
    }
}
