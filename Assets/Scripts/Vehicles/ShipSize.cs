
namespace ProjectB.Vehicles
{
    public enum ShipSize
    {
        /// <summary>
        /// For ships of less than 10 meters (1 unit) long.
        /// </summary>
        Tiny = 0,
        /// <summary>
        /// For ships less than 25 meters (2.5 units) long.
        /// </summary>
        Small = 1,
        /// <summary>
        /// For ships less than 50 meters (5 units) long.
        /// </summary>
        Medium = 2,
        /// <summary>
        /// For ships over 50 meters (5 units) long.
        /// </summary>
        Large = 3
    }

    public static class ShipSizeUtils
    {
        public static bool IsLargerOrEqual(this ShipSize self, ShipSize other)
        {
            return (int)self >= (int)other;
        }

        public static bool IsSmallerOrEqual(this ShipSize self, ShipSize other)
        {
            return (int)self <= (int)other;
        }
    }
}
