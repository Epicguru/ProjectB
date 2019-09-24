
using ProjectB.Units.Actions;
using System.Collections.Generic;

namespace ProjectB.Units
{
    public partial class Unit
    {
        private HashSet<ushort> bannedActions = new HashSet<ushort>();

        /// <summary>
        /// Adds the action to the list of banned actions for this unit.
        /// Banned actions will not be allowed to run by default on this unit.
        /// </summary>
        /// <param name="id">The ID of the action. 0 is invalid.</param>
        public void BanAction(ushort id)
        {
            if (bannedActions.Contains(id))
                return;

            var action = UnitAction.Get(id);
            if (action == null)
                return;

            bannedActions.Add(id);
        }

        /// <summary>
        /// Adds the action to the list of banned actions for this unit.
        /// Banned actions will not be allowed to run by default on this unit.
        /// </summary>
        /// <typeparam name="T">The type of the action. Must inherit from UnitAction.</typeparam>
        public void Banaction<T>() where T : UnitAction
        {
            var action = UnitAction.Get<T>();
            if (action != null)
                this.bannedActions.Add(action.ID);
        }

        /// <summary>
        /// Returns true if the actiion is supported by this unit. Remember that even if an action is supported, it may not
        /// be able to run at any given moment.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsActionBanned(ushort id)
        {
            return bannedActions.Contains(id);
        }

        public bool CanRunAction(ushort id, out string invalidString, params object[] args)
        {
            // Check if it is actually supported by this unit.
            if (IsActionBanned(id))
            {
                invalidString = "That action is banned on this unit.";
                return false;
            }

            invalidString = "Failed to find action for id.";
            return UnitAction.Get(id)?.CanRunOn(this, args, out invalidString) ?? false;
        }

        public bool CanRunAction<T>(out string invalidString, params object[] args) where T : UnitAction
        {
            var action = UnitAction.Get<T>();
            if(action == null)
            {
                invalidString = "Failed to find action for id.";
                return false;
            }

            return this.CanRunAction(action.ID, out invalidString, args);
        }

        public bool CanRunAction(ushort id, params object[] args)
        {
            return this.CanRunAction(id, out string s, args);
        }

        public bool CanRunAction<T>(params object[] args) where T : UnitAction
        {
            var action = UnitAction.Get<T>();
            if (action == null)
            {
                return false;
            }

            return this.CanRunAction(action.ID, out string s, args);
        }

        public bool RunAction<T>(params object[] args) where T : UnitAction
        {
            var action = UnitAction.Get<T>();
            if(action != null)
            {
                return action.TryRun(this, args, out string s);
            }
            else
            {
                return false;
            }
        }

        public bool RunAction(ushort id, params object[] args)
        {
            var action = UnitAction.Get(id);
            if (action != null)
                return action.TryRun(this, args, out string s);
            else
                return false;
        }

        public IEnumerable<UnitAction> GetAllRunnableActions(params object[] args)
        {
            for (int i = 0; i <= UnitAction.RegisteredCount; i++)
            {
                var action = UnitAction.Get((ushort)i);
                if(action != null)
                {
                    if(action.CanRunOn(this, args, out string s))
                    {
                        yield return action;
                    }
                }
            }
        }
    }
}