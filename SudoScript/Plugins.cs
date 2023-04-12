using SudoScript.Data;
using System.Diagnostics;
using System.Reflection;

namespace SudoScript;

public static class Plugins
{
    private enum ParameterType
    {
        None,
        Cell,
        Digit,
    }

    // TODO: Support array types for the last parameter.
    private record FunctionDef(TypeInfo Type, string Name, ParameterType[] Parameters);

    private static readonly Dictionary<string, List<FunctionDef>> _rules = new Dictionary<string, List<FunctionDef>>();
    private static readonly Dictionary<string, List<FunctionDef>> _units = new Dictionary<string, List<FunctionDef>>();

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
                IEnumerable<FunctionDef> function = CreateFunctionDefs(type);
                if (type.ImplementedInterfaces.Contains(typeof(IRule)))
                {
                    if (_rules.TryGetValue(type.Name, out List<FunctionDef>? functions))
                    {
                        Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
                    }
                    else
                    {
                        functions = new List<FunctionDef>();
                        _rules.Add(type.Name, functions);
                    }
                    functions.AddRange(CreateFunctionDefs(type));
                }
                if (type.IsSubclassOf(typeof(Unit)))
                {
                    if (_units.TryGetValue(type.Name, out List<FunctionDef>? functions))
                    {
                        Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
                    }
                    else
                    {
                        functions = new List<FunctionDef>();
                        _units.Add(type.Name, functions);
                    }
                    functions.AddRange(CreateFunctionDefs(type));
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

    private static IEnumerable<FunctionDef> CreateFunctionDefs(TypeInfo type)
    {
        foreach (ConstructorInfo cInfo in type.DeclaredConstructors)
        {
            ParameterType[] parameters = cInfo.GetParameters()
                .Select(p => TypeToParameterType(p.ParameterType))
                .ToArray();
            // If the type is not recognised we dont want to add this constructor.
            if (parameters.Any(p => p == ParameterType.None))
            {
                continue;
            }

            yield return new FunctionDef(type, type.Name, parameters);
        }
    }

    private static ParameterType TypeToParameterType(Type type)
    {
        if (type == typeof(int))
        {
            return ParameterType.Digit;
        }
        if (type == typeof(CellReference))
        {
            return ParameterType.Cell;
        }
        return ParameterType.None;
    }

    private static T Create<T>(IReadOnlyDictionary<string, List<FunctionDef>> defsByName, string typeName, string name, params object[] args)
    {
        if (!defsByName.TryGetValue(name, out List<FunctionDef>? defs))
        {
            // TODO: Add fuzzy search here, to find the closest matching rule and output the name of that.
            throw new Exception($"{typeName} '{name}' doesn't exist.");
        }

        foreach (FunctionDef def in defs)
        {
            for (int i = 0; i < def.Parameters.Length; i++)
            {
                ParameterType paramType = def.Parameters[i];
                bool isCorrectType = paramType switch
                {
                    ParameterType.Cell => args[i] is CellReference,
                    ParameterType.Digit => args[i] is int,

                    // These should not be possible to hit.
                    ParameterType.None => throw new NotImplementedException(),
                    _ => throw new NotImplementedException(),
                };

                if (!isCorrectType)
                {
                    continue;
                }
            }

            try
            {
                T? obj = (T?)Activator.CreateInstance(def.Type, args);
                return obj ?? throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                throw new Exception($"Something unexpected happened creating {typeName.ToLower()} '{name}'. {{{def.Type.Namespace}}}.", ex);
            }
        }

        string functionCall = name + " " + string.Join(" ", args.Select(x => x.GetType() == typeof(int) ? "Digit" : x.GetType() == typeof(Cell) ? "Cell" : "None"));
        throw new Exception($"Found no matching function interface for function call '{functionCall}'");
    }
}
