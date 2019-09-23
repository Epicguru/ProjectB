
namespace ProjectB.Units.Actions
{
    public class ExampleAction : UnitAction
    {
        // This example action causes a vehicle to gain a lot of forward velocity.

        public override bool CanRunOn(Unit unit, object[] args, out string reasonInvalid)
        {
            if(!base.CanRunOn(unit, args, out reasonInvalid))
            {
                return false;
            }

            // Only runs on vehicles.
            if (!unit.IsVehicle)
            {
                reasonInvalid = "Not a vehicle.";
            }

            reasonInvalid = null;
            return true;
        }

        protected override string Run(Unit unit, object[] args)
        {
            // Unit should already be valid.

            // Increases its velocity by 20 meters per second, in the forward direction.

            unit.Vehicle.Body.velocity += unit.ForwardVector * 20f * Utils.METERS_TO_UNITS;
            return "Increased velocity.";
        }
    }
}
