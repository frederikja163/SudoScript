using SudoScript.Data;
using System.Diagnostics;
using System.Reflection;

namespace SudoScript;

public static class Plugins
{
    private static readonly Dictionary<string, List<TypeInfo>> _rules = new Dictionary<string, List<TypeInfo>>();
    private static readonly Dictionary<string, List<TypeInfo>> _units = new Dictionary<string, List<TypeInfo>>();

    static Plugins()
    {
        string[] files = Directory.GetFiles("./", "*.dll");
        foreach (string file in files)
        {
            AssemblyName name = AssemblyName.GetAssemblyName(file);
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(name);
            }
            catch (FileNotFoundException)
            {
                assembly = Assembly.LoadFrom(file);
            }

            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (type.ImplementedInterfaces.Contains(typeof(IRule)))
                {
                    if (_rules.TryGetValue(type.Name, out List<TypeInfo>? functions))
                    {
                        Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
                    }
                    else
                    {
                        functions = new List<TypeInfo>();
                        _rules.Add(type.Name, functions);
                    }
                    functions.Add(type);
                }
                if (type.IsSubclassOf(typeof(Unit)))
                {
                    if (_units.TryGetValue(type.Name, out List<TypeInfo>? functions))
                    {
                        Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
                    }
                    else
                    {
                        functions = new List<TypeInfo>();
                        _units.Add(type.Name, functions);
                    }
                    functions.Add(type);
                }
            }
        }
    }

    public static IRule CreateRule(string name, params object[] args)
    {
        return Create<IRule>(_rules, "Rule", name, args);
    }
    public static Unit CreateUnit(string name, params object[] args)
    {
        return Create<Unit>(_units, "Unit", name, args);
    }

    private static T Create<T>(IReadOnlyDictionary<string, List<TypeInfo>> typesByName, string typeName, string name, params object[] args)
    {
        if (!typesByName.TryGetValue(name, out List<TypeInfo>? types))
        {
            // TODO: Add fuzzy search here, to find the closest matching rule and output the name of that.
            throw new Exception($"{typeName} '{name}' doesn't exist.");
        }

        foreach (TypeInfo type in types)
        {
            try
            {
                T? obj = (T?)Activator.CreateInstance(type, args);
                if (obj is not null)
                {
                    return obj;
                }
            }
            catch
            {
                // Ignore.
            }
        }

        IEnumerable<string> argStrings = args.Select(x =>
            x is int ? "Digit" : x is Cell ? "Cell" : "None");
        string argList = string.Join(" ", argStrings);
        string functionCall = name + " " + argList;
        throw new Exception($"Found no matching function interface for function call '{functionCall}'");
    }
}
