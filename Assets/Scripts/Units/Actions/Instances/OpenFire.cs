
namespace ProjectB.Units.Actions
{
    [LoadedAction]
    public class OpenFireAction : UnitAction
    {
        public OpenFireAction() : base("Open fire")
        {
            ExpectedArgumentCount = 1;
        }

        public override bool CanRunOn(Unit unit, object[] args, out string reasonInvalid)
        {
            if(!unit.IsVehicle)
            {
                reasonInvalid = "Unit is not vehicle";
                return false;
            }

            reasonInvalid = null;
            return true;
        }

        protected override string Run(Unit unit, object[] args)
        {
            var weapons = unit.Vehicle.MountedWeapons;
            for (int i = 0; i < weapons.SpotCount; i++)
            {
                var weapon = weapons.GetWeapon(i);
                if(weapon != null)
                    weapon.Fire = true;
            }

            return null;
        }
    }
}
