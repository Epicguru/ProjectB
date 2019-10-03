using System.Collections.Generic;

namespace ProjectB.Units.Actions
{
    public interface IActionProvider
    {
        IEnumerable<UnitAction> GetActions(Unit unit);
    }
}
