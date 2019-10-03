
using System;
using System.Collections.Generic;

namespace ProjectB.Units.Actions
{
    public abstract class UnitAction
    {
        public string Name { get; set; }
        public ushort ID { get; set; }

        public UnitAction(string name)
        {
            this.Name = name;
        }

        public abstract string Run(Unit unit, object[] args);

        public static IEnumerable<UnitAction> MergeActions(IEnumerable<Unit> units)
        {
            usedIDS.Clear();

            foreach (var unit in units)
            {
                if (unit == null)
                    continue;

                foreach (var action in unit.GetAllActions())
                {
                    if (action.ValidID)
                    {
                        if (!usedIDS.Contains(action.ID))
                        {
                            usedIDS.Add(action.ID);
                            yield return action;
                        }
                    }
                }
            }
        }

        private static readonly HashSet<string> usedIDS = new HashSet<string>();
        public static IEnumerable<UnitAction> MergeActions(IEnumerable<IEnumerable<UnitAction>> actionSets)
        {
            usedIDS.Clear();

            foreach (var set in actionSets)
            {
                foreach (var action in set)
                {
                    if (action.ValidID)
                    {
                        if (!usedIDS.Contains(action.ID))
                        {
                            usedIDS.Add(action.ID);
                            yield return action;
                        }
                    }
                }
            }            
        }
    }
}
