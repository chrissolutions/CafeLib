using DroneLander.Common;

namespace DroneLander.Models
{
    public enum LandingResultType { Landed, Kaboom, }
    public class LandingParameters
    {
        public LandingParameters()
        {
            Altitude = Constants.StartingAltitude;
            Velocity = Constants.StartingVelocity;
            Fuel = Constants.StartingFuel;
            Thrust = Constants.StartingThrust;
        }

        public double Altitude;
        public double Velocity;
        public double Fuel;
        public double Thrust;
    }
}