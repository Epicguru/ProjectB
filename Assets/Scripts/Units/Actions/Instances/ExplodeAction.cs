
namespace ProjectB.Units.Actions
{
    [LoadedAction]
    public class ExplodeAction : UnitAction
    {
        public ExplodeAction() : base("Explode")
        {
            ExpectedArgumentCount = 0;
        }

        public override bool CanRunOn(Unit unit, object[] args, out string reasonInvalid)
        {
            if (!base.CanRunOn(unit, args, out reasonInvalid))
                return false;

            if (!unit.IsVehicle)
            {
                reasonInvalid = "Unit is not vehicle";
                return false;
            }

            return true;
        }

        protected override string Run(Unit unit, object[] args)
        {
            unit.Vehicle.Health.ChangeHealth(unit.Vehicle.Health.GetHealthPart(HealthPartID.HULL).Collider, -9999);
            return "Boom.";
        }
    }
}
