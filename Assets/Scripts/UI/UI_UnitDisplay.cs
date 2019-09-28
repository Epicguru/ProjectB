
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

            UI.Label($"Selected: {(units.Count == 1 ? units[0].Name : (units.Count + " units"))}.");
            UI.Label("Available actions:");

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));

            int index = 0;
            foreach (var action in units.Count != 1 ? UnitAction.CompileRunnableActionList(units) : units[0].GetAllRunnableActions())
            {
                if (UI.Button($"{action.Name} [{ActionKeys.Get(index)}]") || Input.GetKeyDown(ActionKeys.Get(index)))
                {
                    if(units.Count == 1)
                    {
                        units[0].RunAction(action.ID);
                    }
                    else
                    {
                        foreach (var unit in units)
                        {
                            unit.RunAction(action.ID);
                        }
                    }
                }
                index++;
            }

            GUILayout.EndScrollView();

            if(units.Count == 1)
                DisplaySingleUnit(units[0]);
        }

        private Vector2 scrollPos;
        private Vector2 scrollPos2;

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

            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.ExpandHeight(false));
            foreach (var part in unit.Health.GetAllHealthParts())
            {
                GUILayout.BeginHorizontal();
                UI.Label($"{part.Name}: ", GUILayout.Width(Width * 0.5f));

                UI.Alignment = TextAnchor.UpperRight;
                float p = part.Health / part.MaxHealth;
                UI.Label($"{p*100f:F0}%".InColour(p >= 0.75f ? Color.green : p > 0.40f ? Color.yellow : Color.red).InBold());
                UI.PreviousAlignment();

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}
