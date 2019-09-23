
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Units.Actions
{
    public abstract class UnitAction
    {
        public static readonly object[] EmptyArgs = new object[0];
        private static readonly Dictionary<Type, UnitAction> actionMap = new Dictionary<Type, UnitAction>();
        private static readonly List<UnitAction> allActions = new List<UnitAction>();
        private static ushort highestID = 1;

        /// <summary>
        /// Registers a unit action for use by all applicable units.
        /// </summary>
        /// <param name="action">The action instance. Should be null.</param>
        public static void RegisterAction(UnitAction action)
        {
            if (action == null)
            {
                Debug.LogError("Null action, will not register.");
                return;
            }

            Type type = action.GetType();
            if (actionMap.ContainsKey(type))
            {
                Debug.LogError($"Duplicate action: {type.FullName} ({action.Name}). Will not be registered.");
                return;
            }

            if (string.IsNullOrWhiteSpace(action.Name))
            {
                Debug.LogError($"Action name cannot be null or whitespace. Give it a name! Not registered. ({type.FullName})");
                return;
            }

            actionMap.Add(type, action);
            allActions.Add(action);
            action.ID = highestID;
            highestID++;
        }

        public static UnitAction Get(ushort id)
        {
            if (id == 0)
                return null;

            if (id > allActions.Count)
            {
                Debug.LogError($"ID {id} is out of range. Max ID is {allActions.Count}.");
                return null;
            }

            return allActions[id - 1];
        }

        public static UnitAction Get<T>() where T : UnitAction
        {
            Type type = typeof(T);
            if (actionMap.ContainsKey(type))
            {
                return actionMap[type];
            }
            else
            {
                Debug.LogError($"Could not find action for type {type.FullName}.");
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID of this unit action. The ID is guaranteed to be unique in this current session, but may be different
        /// in another.
        /// </summary>
        public ushort ID
        {
            get; private set;
        }

        /// <summary>
        /// The display name of this action.
        /// </summary>
        public string Name { get; protected set; } = "Unit Action";

        /// <summary>
        /// The number of arguments that the <see cref="CanRunOn(Unit, object[], out string)"/> and <see cref="Run(Unit, object[])"/> methods expect.
        /// </summary>
        public virtual int ExpectedArgumentCount
        {
            get; protected set;
        } = 0;

        /// <summary>
        /// Returns true if the unit passed into the method can currently have this action run on it.
        /// The Unit passed into this method should never be null.
        /// The default implementation of this will return false if the unit is null, the arguement array is null or if the number of arguments does not meet the expectations set in <see cref="ExpectedArgumentCount"/>.
        /// </summary>
        /// <param name="unit">The unit to validate.</param>
        /// <param name="args">The arguments that would be passed into the Run function. Should never be null, but may be empty.</param>
        /// <param name="reasonInvalid">A string that should give the reason why a unit cannot have this action run on it.
        /// For example, if this Action is 'Turn on engine' and the unit does not have an engine, the string would be: 'Missing engine!'. Leaving this null is allowed.</param>
        /// <returns>True if this action can currently be run on the unit, false if this action cannot be run on the unit.</returns>
        public virtual bool CanRunOn(Unit unit, object[] args, out string reasonInvalid)
        {
            if(unit == null)
            {
                reasonInvalid = "Unit is null.";
                return false;
            }

            if(args == null)
            {
                reasonInvalid = "Argument array is null.";
                return false;
            }

            if(args.Length != ExpectedArgumentCount)
            {
                reasonInvalid = $"Expected {ExpectedArgumentCount} arguments, got {args.Length}.";
                return false;
            }

            if (!unit.IsActionSupported(this.ID))
            {
                reasonInvalid = $"The unit does not support this action. See Unit.AddAction().";
                return false;
            }

            reasonInvalid = null;
            return true;
        }

        /// <summary>
        /// Trys to run this action on the supplied unit. The unit and argument array are allowed to be null.
        /// If the unit is null, this method does nothing. If the argument array is null, an empty array is passed into the run function.
        /// </summary>
        /// <param name="unit">The unit to run the action on, if possible.</param>
        /// <param name="args">The arguments to be used. If null, and empty array is used-</param>
        /// <param name="reasonFailed">The reason why the action cannot be run on the unit supplied. May be null if successful.</param>
        /// <returns>True if the action was run, false if it was not run.</returns>
        public bool TryRun(Unit unit, object[] args, out string reasonFailed)
        {
            if (args == null)
                args = EmptyArgs;

            if(!CanRunOn(unit, args, out reasonFailed))
            {
                return false;
            }

            Run(unit, args);

            reasonFailed = null;
            return true;
        }

        /// <summary>
        /// Runs this action on the target unit. The unit passed into this should never be null, and should have
        /// been validated using <see cref="CanRunOn(Unit, out string)"/>.
        /// </summary>
        /// <param name="unit">The target unit. Will not be null, and should have been validated using <see cref="CanRunOn(Unit, out string)"/>.</param>
        /// <param name="args">The arguments passed into this action. The array may be empty, but will normally never be null.</param>
        /// <returns>An optional string detailing how the action completed. Returning null is perfectly valid, and the action is assumed to have completed successfully.</returns>
        protected abstract string Run(Unit unit, object[] args);
    }
}
