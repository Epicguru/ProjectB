
using ProjectB.Units;
using ProjectB.Units.Actions;
using ProjectB.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Interface
{
    public class UI_UnitDisplay : MonoBehaviour
    {
        public float Width = 200f, Height = 250f;

        private void Awake()
        {
            UI.AddDrawer(DrawUI);
        }

        public void DrawUI()
        {
            const float PADDING = 10f;
            var rect = new Rect(UI.ScreenWidth - Width - PADDING, PADDING, Width, Height);
            GUI.Box(rect.Inflate(5f), string.Empty);
            GUILayout.BeginArea(rect);

            UI.Alignment = TextAnchor.MiddleCenter;
            UI.Label("Selected Units");
            UI.Alignment = TextAnchor.UpperLeft;

            DisplayUnits();

            GUILayout.EndArea();
        }

        private List<Unit> units = new List<Unit>();
        private void DisplayUnits()
        {
            var selected = Unit.GetAllSelected();
            units.Clear();
            units.AddRange(selected);

            if (units.Count == 0)
                return;

            if(units.Count == 1)
            {
                // Only one selected.
                var unit = units[0];

                UI.Label($"Selected: {unit.Name}");
                UI.Label("Available actions:");

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(300));

                int index = 0;
                foreach (var action in unit.GetAllRunnableActions())
                {                    
                    if(UI.Button($"{action.Name} [{ActionKeys.Get(index)}]") || Input.GetKeyDown(ActionKeys.Get(index)))
                    {
                        unit.RunAction(action.ID);
                    }
                    index++;
                }

                GUILayout.EndScrollView();

                DisplaySingleUnit(unit);
            }
            else
            {
                // Multiple selected.
                UI.Label($"Selected {units.Count} units.");
                UI.Label("Available actions:");

                scrollPos = GUILayout.BeginScrollView(scrollPos);

                var actions = UnitAction.CompileRunnableActionList(units);
                int index = 0;
                foreach (var action in actions)
                {
                    if (UI.Button($"{action.Name} [{ActionKeys.Get(index)}]") || Input.GetKeyDown(ActionKeys.Get(index)))
                    {
                        foreach (var unit in units)
                        {
                            // Try to run the action on all units.
                            unit.RunAction(action.ID);
                        }
                    }
                    index++;
                }

                GUILayout.EndScrollView();
            }
        }
        private Vector2 scrollPos;

        private void DisplaySingleUnit(Unit unit)
        {
            if (unit.IsVehicle)
            {
                var vehicle = unit.Vehicle;
                UI.Label($"Speed: {vehicle.Body.velocity.magnitude * Utils.UNITS_TO_METERS * 3.6f:F1} kph.");
                UI.Label($"Weapon spots: {vehicle.MountedWeapons.SpotCount}");
                string target = "None";
                if (vehicle.Nav.HasPath)
                    target = vehicle.Nav.CurrentTargetPos.ToString();
                UI.Label($"Moving to: {target}");
            }
        }
    }
}
