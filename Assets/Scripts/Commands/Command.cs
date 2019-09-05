
using System;
using System.Reflection;
using System.Text;

public class Command
{
    private static StringBuilder str = new StringBuilder();

    public string Name { get; }
    public CommandAttribute Attribute { get; }
    public MethodInfo Method { get; }
    public string ParamsString { get; }
    public Type[] ArgTypes { get; }

    public bool HasStringReturn
    {
        get
        {
            return Method.ReturnParameter != null && Method.ReturnParameter.ParameterType == typeof(string);
        }
    }

    public Command(CommandAttribute atr, MethodInfo method)
    {
        if (string.IsNullOrWhiteSpace(atr.Name))        
            Name = method.Name.ToLowerInvariant();        
        else        
            Name = atr.Name.Trim().ToLowerInvariant();
        
        Attribute = atr;
        Method = method;

        str.Clear();

        var paramz = method.GetParameters();
        ArgTypes = new Type[paramz.Length];

        for(int i = 0; i < paramz.Length; i++)
        {
            var item = paramz[i];

            ArgTypes[i] = item.ParameterType;

            str.Append(item.ParameterType.Name);
            str.Append(' ');
            str.Append(item.Name);
            if(i != paramz.Length - 1)
                str.Append(", ");
        }
        ParamsString = str.ToString();
    }
}