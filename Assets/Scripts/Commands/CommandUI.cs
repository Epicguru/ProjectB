
using Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class CommandUI : MonoBehaviour
{
    [GameVar]
    public static bool GVWriteCompare = true;

    [GameVar]
    public static float TimeScale
    {
        get
        {
            return Time.timeScale;
        }
        set
        {
            Time.timeScale = value;
        }
    }

    private const int MAX_HISTORY = 30;
    private const float LOG_AREA = 0.7f;
    // Super confusing note - the class in [Command] and Command are not the same. [Command] is actually [CommandAttribute] and the other is Command.cs
    public static SortedDictionary<string, Command> Commands = new SortedDictionary<string, Command>();
    public static SortedDictionary<string, GameVar> GameVars = new SortedDictionary<string, GameVar>();
    public static List<CommandDrawable> Log = new List<CommandDrawable>();
    private static StringBuilder str = new StringBuilder();

    public IMGUIWindow Input;
    public int Width = 400, Height = 300;

    public string CurrentCommand = string.Empty;
    public List<string> PreviousCommands = new List<string>();
    public List<string> PinnedCommands = new List<string>();
    public Texture Bin, Pin;
    public GUISkin CustomSkin;

    private int historyIndex = 0;
    private Vector2 scroll;
    private Vector2 scroll2;
    private bool open = false;

    private void Awake()
    {
        LoadCommands();

        Input = new IMGUIWindow(0, new Rect(100, 100, Width, Height), "Console", () =>
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return && !string.IsNullOrWhiteSpace(CurrentCommand))
            {
                SubmitCommand(CurrentCommand);
                CurrentCommand = string.Empty;
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow)
            {
                BrowseHistory(1);
            }
            else if(e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow)
            {
                BrowseHistory(-1);
            }

            var items = GetSuggestions(CurrentCommand);
            string suggestions = GetSuggestionString(items);
            bool tabbed = false;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab)
            {
                if (items.Count > 0)
                {
                    tabbed = true;
                }
            }

            float SafeZone = 15f;
            var previous = GUI.skin;
            GUI.skin = CustomSkin;
            
            GUILayout.Label(RichText.InColour(suggestions, Color.white * 0.8f));            

            GUILayout.BeginHorizontal();
            if (tabbed)
                CurrentCommand = items[0];
            CurrentCommand = GUILayout.TextField(CurrentCommand, GUILayout.MaxWidth(Width - 50));
            if(tabbed)
            {
                var txtF = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                txtF.cursorIndex = CurrentCommand.Length;
                txtF.selectIndex = CurrentCommand.Length;
            }
            bool pin = GUILayout.Button(Pin, GUILayout.ExpandWidth(false));
            if (pin)
                PinCmd(CurrentCommand);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            // Log view area.
            scroll = GUILayout.BeginScrollView(scroll, false, true, GUILayout.Width((Width - SafeZone) * LOG_AREA));
            for (int i = Log.Count - 1; i >= 0; i--)
            {
                var item = Log[i];
                item.Draw();
            }            
            
            GUILayout.EndScrollView();

            // Pinned commands area.
            scroll2 = GUILayout.BeginScrollView(scroll2, false, true, GUILayout.Width((Width - SafeZone) * (1f - LOG_AREA)));

            // Single pinned command.
            for (int i = 0; i < PinnedCommands.Count; i++)
            {
                var cmd = PinnedCommands[i];
                GUILayout.BeginHorizontal();
                bool run = GUILayout.Button(cmd, GUILayout.MaxWidth(100));
                if (run)
                    ExecuteCommand(cmd);
                bool delete = GUILayout.Button(Bin, GUILayout.ExpandWidth(false));
                if(delete)
                {
                    PinnedCommands.RemoveAt(i);
                    i--;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUI.skin = previous;
        });
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus))
            open = !open;
    }

    public static void LoadCommands()
    {
        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        s.Start();

        Assembly a = typeof(CommandUI).Assembly;
        Debug.Log($"Searching for custom commands and variables in assembly '{a.FullName}'");

        // Partition on the type list initially.
        var def = typeof(CommandAttribute);
        var found = from t in a.GetTypes().AsParallel()
                    let methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    from m in methods
                    where m.IsDefined(def, true)
                    select new { Type = t, Method = m, Attribute = m.GetCustomAttribute<CommandAttribute>() };

        var gameVarDef = typeof(GameVarAttribute);
        var gameF = from t in a.GetTypes().AsParallel()
                    let fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    from f in fields
                    where f.IsDefined(gameVarDef, true)
                    select new { Type = t, Field = f, Attribute = f.GetCustomAttribute<GameVarAttribute>() };
        var gameP = from t in a.GetTypes().AsParallel()
                    let properties = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    from p in properties
                    where p.IsDefined(gameVarDef, true)
                    select new { Type = t, Property = p, Attribute = p.GetCustomAttribute<GameVarAttribute>() };

        foreach (var gv in gameF)
        {
            var thing = new GameVar(gv.Attribute, gv.Field, null);
            GameVars.Add(thing.Name, thing);
        }
        foreach (var gv in gameP)
        {
            var thing = new GameVar(gv.Attribute, null, gv.Property);
            GameVars.Add(thing.Name, thing);
        }

        for (int i = 0; i < GameVars.Count; i++)
        {
            var gv = GameVars.ElementAt(i).Value;
            if (!gv.IsValid)
            {
                Debug.LogWarning($"{gv.Name} is not a valid game variable, ignoring.");
                GameVars.Remove(gv.Name);
                i--;
            }
        }

        int count = 0;
        StringBuilder str = new StringBuilder();
        foreach (var cmd in found)
        {
            var type = cmd.Type;
            var method = cmd.Method;
            var attr = cmd.Attribute;

            if (!method.IsStatic)
            {
                Debug.LogError($"Currently non-static commands are not supported: {type.FullName}.{method.Name}");
                continue;
            }

            Command c = new Command(attr, method);
            Commands.Add(c.Name, c);

            str.Append($"Class: {type.FullName}, Method: {method.Name}\n");
            count++;
        }

        s.Stop();

        Debug.Log($"Took {s.Elapsed.TotalMilliseconds:F1}ms. Found {count} commands:\n{str.ToString()}\n");
    }

    private void PinCmd(string cmd)
    {
        cmd = cmd.Trim();
        if (!PinnedCommands.Contains(cmd))
        {
            PinnedCommands.Insert(0, cmd);
        }
    }

    private void BrowseHistory(int direction)
    {
        if (PreviousCommands.Count == 0)
            return;

        historyIndex = Mathf.Clamp(historyIndex, 0, PreviousCommands.Count - 1);
        if (CurrentCommand.Trim() != PreviousCommands[historyIndex])
        {
            historyIndex = 0;
            CurrentCommand = PreviousCommands[0];
        }
        else
        {
            historyIndex += direction;
            historyIndex = Mathf.Clamp(historyIndex, 0, PreviousCommands.Count - 1);
            CurrentCommand = PreviousCommands[historyIndex];
        }     
    }

    private void SubmitCommand(string cmd)
    {
        cmd = cmd.Trim();

        LogText(RichText.InItalics(cmd));

        ExecuteCommand(cmd);
        LogHistory(cmd);

        historyIndex = -1;
    }

    private void LogText(string text)
    {
        Log.Add(new CommandDrawable(() =>
        {
            GUILayout.Label(text);
        }));
    }

    private void LogBoxText(string text)
    {
        Log.Add(new CommandDrawable(() =>
        {
            GUILayout.Box(text);
        }));
    }

    private List<string> items = new List<string>();
    public List<string> GetSuggestions(string input)
    {
        items.Clear();
        if (string.IsNullOrWhiteSpace(input))
            return items;

        input = input.Trim().ToLowerInvariant();
        if (input[0] == '-')
            input = input.Substring(1);

        foreach (var gv in GameVars)
        {
            items.Add('-' + gv.Key);
        }
        foreach (var cmd in Commands)
        {
            items.Add(cmd.Key);
        }

        for (int i = 0; i < items.Count; i++)
        {
            bool remove = false;
            string val = items[i];

            if (!val.Contains(input))
                remove = true;

            if (remove)
            {
                items.RemoveAt(i);
                i--;
            }
        }

        return items;
    }

    public string GetSuggestionString(List<string> items)
    {
        str.Clear();
        int l = Mathf.Min(4, items.Count);
        for (int i = 0; i < l; i++)
        {
            string s = items[i];
            str.Append(s);
            str.Append("   ");
        }

        return str.ToString();
    }

    private (string, string[]) SplitCommand(string input)
    {
        input = input.Trim();
        int firstSpace = input.IndexOf(' ');
        if (firstSpace < 0)
            firstSpace = input.Length;
        string firstPart = input.Substring(0, firstSpace);
        string[] args = input.Substring(firstSpace).TrimStart().Split(',');
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = args[i].Trim();
        }
        if(args.Length == 1 && string.IsNullOrWhiteSpace(args[0]))
        {
            args = new string[0];
        }

        return (firstPart, args);
    }

    private void ExecuteCommand(string cmd)
    {
        (string command, string[] args) = SplitCommand(cmd);
        Debug.Log($"CMD: '{command}' with {args.Length} args");

        if (command.StartsWith("-"))
        {
            // It's a variable get or set.
            string name = command.Remove(0, 1);
            if (GameVars.ContainsKey(name))
            {
                var gv = GameVars[name];

                if (!gv.FInfo.IsStatic)
                {
                    LogText(RichText.InColour($"Game var '{name}' is not static, which is currently not fully supported: cannot read or write.", Color.yellow));
                }
                else
                {
                    try
                    {
                        // TODO add support for non-static variables and commands.
                        if (args.Length == 0)
                        {
                            if (gv.FInfo.CanRead)
                            {
                                // Read varaible...
                                LogBoxText(gv.Converter.MakeString(null, gv.FInfo));
                            }
                            else
                            {
                                LogText(RichText.InColour($"That variable is a C# property, which is set to be write only.", Color.yellow));
                            }
                        }
                        else
                        {
                            if (gv.FInfo.CanWrite)
                            {
                                // Write variable.
                                string old = null;
                                if (GVWriteCompare)
                                    old = gv.Converter.MakeString(null, gv.FInfo);
                                string error = gv.Converter.Write(null, gv.FInfo, args);
                                bool worked = error == null;
                                if (!worked)
                                {
                                    LogText(RichText.InColour("Failed to write to variable:\n{error}", Color.yellow));
                                }
                                else if (GVWriteCompare)
                                {
                                    string updated = gv.Converter.MakeString(null, gv.FInfo);
                                    LogBoxText($"{old} -> {updated}");
                                }
                            }
                            else
                            {
                                LogText(RichText.InColour($"That variable is a C# property, which is set to be read only.", Color.yellow));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogText(RichText.InColour($"Exception when reading or writing value '{name}': {e}!", Color.yellow));

                    }
                }                
            }
            else
            {
                LogText(RichText.InColour("Could not find that variable!", Color.yellow));
            }
        }
        else
        {
            // Normal command.
            if (Commands.ContainsKey(command))
            {
                var c = Commands[command];
                if (c.HasStringReturn)
                {
                    int expected = c.Method.GetParameters().Length;
                    if (args.Length != expected)
                    {
                        LogText(RichText.InColour($"Incorrect number of arguments. Expected {expected}, got {args.Length}", Color.yellow));
                    }
                    else
                    {
                        try
                        {
                            object[] realArgs = MakeArgs(c.ArgTypes, args, out string error);
                            if(error != null)
                            {
                                LogText(RichText.InColour(error, Color.yellow));
                            }
                            else
                            {
                                var r = c.Method.Invoke(null, realArgs);
                                if (c.HasStringReturn && !string.IsNullOrWhiteSpace(r as string))
                                {
                                    LogBoxText(r as string);
                                }
                                else
                                {
                                    LogText(RichText.InColour("Run successfully", Color.green));
                                }
                            }                            
                        }
                        catch(Exception e)
                        {
                            LogText(RichText.InColour($"Exception when running command '{command}':\n{e}", Color.yellow));
                        }
                    }                    
                }
            }
            else
            {
                LogText(RichText.InColour("Could not find that command!", Color.yellow));
            }
        }        
    }

    private object[] MakeArgs(Type[] types, string[] text, out string error)
    {
        int expected = 0;
        VarConverter[] converters = new VarConverter[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            var c = GameVar.GetConverter(types[i]);
            if(c == null)
            {
                error = $"Could not find parser for type '{types[i].FullName}'.";
                return null;
            }

            converters[i] = c;
            expected += c.ExpectedArgCount;
        }

        if(expected != text.Length)
        {
            error = $"Incorrect number of input parts.\nExpected: {expected} parts from {types.Length} parameters, got {text.Length} parts.";
            return null;
        }

        object[] output = new object[types.Length];
        int readIndex = 0;
        for (int i = 0; i < types.Length; i++)
        {
            var c = converters[i];
            string[] args = new string[c.ExpectedArgCount];
            System.Array.Copy(text, readIndex, args, 0, c.ExpectedArgCount);
            readIndex += c.ExpectedArgCount;

            object obj = c.Convert(args, out string oops);
            if(oops != null)
            {
                error = $"Failed to parse argument #{i} ({types[i].Name}):\n{oops}";;
                return null;
            }

            output[i] = obj;
        }

        error = null;
        return output;
    }

    private void LogHistory(string cmd)
    {
        if(PreviousCommands.Count > 0)
        {
            if (cmd != PreviousCommands[0])
            {
                PreviousCommands.Insert(0, cmd);
                if (PreviousCommands.Count > MAX_HISTORY)
                    PreviousCommands.RemoveAt(PreviousCommands.Count - 1);
            }
        }
        else
        {
            PreviousCommands.Insert(0, cmd);
        }        
    }

    public void OnGUI()
    {
        if(open)
            Input.OnGUI();
    }

    [Command("Clears the command console.")]
    public static string Clear()
    {
        Log.Clear();
        return "Cleared";
    }

    [Command("Prints all variable names and values.")]
    public static string Vars()
    {
        str.Clear();
        foreach (var pair in GameVars)
        {
            str.Append(pair.Key);
            str.Append(": ");
            if (pair.Value.FInfo.IsStatic)
            {
                str.Append(pair.Value.Converter.MakeString(null, pair.Value.FInfo));
            }
            else
            {
                str.Append("(Not static)");
            }
            str.AppendLine();
        }

        return str.ToString().TrimEnd();
    }

    [Command("Prints all command names, parameters and tooltip.")]
    public static string Cmds()
    {
        str.Clear();
        foreach (var item in Commands)
        {
            str.Append(item.Key);
            str.Append('(');
            str.Append(item.Value.ParamsString);
            str.Append(") : ");
            str.Append(item.Value.Attribute.Tooltip);
            str.AppendLine();
        }
        return str.ToString();
    }
}
