namespace Evo20.Sensors
{

    public struct ProfilePart
    {
        public int FirstPosition { set; get; }

        public int SecondPosition { set; get; }

        public int SpeedFirst { set; get; }

        public int SpeedSecond { set; get; }

        public ProfilePart(int firstPosition, int secondPosition, int speedFirst, int speedSecond)
        {
           FirstPosition = firstPosition;
           SecondPosition = secondPosition;
           SpeedFirst = speedFirst;
           SpeedSecond = speedSecond;
        }
        public ProfilePart(int firstPosition, int secondPosition)
            : this(firstPosition, secondPosition, 0, 0) { }
    }
}
