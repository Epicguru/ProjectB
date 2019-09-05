
using System.Reflection;
using System.Text;

public class Command
{
    private static StringBuilder str = new StringBuilder();

    public string Name { get; private set; }
    public CommandAttribute Attribute { get; private set; }
    public MethodInfo Method { get; private set; }
    public string ParamsString;

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
        for(int i = 0; i < paramz.Length; i++)
        {
            var item = paramz[i];
            str.Append(item.ParameterType.Name);
            str.Append(' ');
            str.Append(item.Name);
            if(i != paramz.Length - 1)
                str.Append(", ");
        }
        ParamsString = str.ToString();
    }
}