using System.Diagnostics;
using System.Reflection;
using SudoScript.Core.Data;

namespace SudoScript.Core;

public sealed class SymbolTable
{
    public enum ArgType
    {
        None,
        Cell,
        Digit,
    }

    public record Function(ArgType[] Args);

    public record UnitFunction(Func<object[], IEnumerable<Unit>> Delegate, ArgType[] Args) : Function(Args);
    public record RuleFunction(Func<object[], IRule> Delegate, ArgType[] Args) : Function(Args);
    
    private readonly Dictionary<string, int> _digitTable;
    private readonly Dictionary<CellReference, Cell> _cellTable;
    private readonly Dictionary<string, List<RuleFunction>> _rules;
    private readonly Dictionary<string, List<UnitFunction>> _units;

    public SymbolTable()
    {
        _digitTable = new Dictionary<string, int>();
        _cellTable = new Dictionary<CellReference, Cell>();
        _rules = new Dictionary<string, List<RuleFunction>>();
        _units = new Dictionary<string, List<UnitFunction>>();
        
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
                    foreach ((Func<object[], object> func, ArgType[] args) in GetConstructors(type))
                    {
                        AddRulesFunction(type.Name, new Func<object[], IRule>(func), args);
                    }
                }
                if (type.IsSubclassOf(typeof(Unit)))
                {
                    foreach ((Func<object[], object> func, ArgType[] args) in GetConstructors(type))
                    {
                        AddUnitFunction(type.Name, new Func<object[], Unit>(func), args);
                    }
                }

                if (type.ImplementedInterfaces.Contains(typeof(IEnumerable<Unit>)))
                {
                    foreach ((Func<object[], object> func, ArgType[] args) in GetConstructors(type))
                    {
                        AddUnitFunction(type.Name, new Func<object[], IEnumerable<Unit>>(func), args);
                    }
                }
            }
        }
    }

    public SymbolTable(SymbolTable table)
    {
        _digitTable = table._digitTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        _units = table._units.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        _cellTable = table._cellTable;
        _rules = table._rules;
    }

    public void AddDigit(string identifier, int value)
    {
        if (_digitTable.ContainsKey(identifier))
        {
            throw new Exception("Duplicate definition of identifier " +  identifier);
        }
        _digitTable.Add(identifier, value);
    }

    public void AddCell(CellReference reference, Cell cell)
    {
        if (!_cellTable.TryGetValue(reference, out Cell? oldCell))
        {
            _cellTable.Add(reference, cell);
            return;
        }

        if (oldCell.IsGiven && cell.IsGiven)
        {
            throw new Exception("Multiple definitions of the given for cell " + cell.X + " " + cell.Y);
        }
        else if (cell.IsGiven)
        {
            _cellTable[reference] = cell;
        }
    }
    
    public void AddUnitFunction(string name, Func<object[], Unit> func, ArgType[] args)
    {
        if (!_units.TryGetValue(name, out List<UnitFunction>? functions))
        {
            functions = new List<UnitFunction>();
            _units.Add(name, functions);
        }

        if (functions.Any(f => f.Args.SequenceEqual(args)))
        {
            throw new Exception(name + ' ' + string.Join(' ', args) + " is defined two times.");
        }
        
        IEnumerable<Unit> EnumUnitFunc(object[] obj)
        {
            yield return func(obj);
        }
        functions.Add(new UnitFunction(EnumUnitFunc, args));
    }
    
    public void AddUnitFunction(string name, Func<object[], IEnumerable<Unit>> func, ArgType[] args)
    {
        if (!_units.TryGetValue(name, out List<UnitFunction>? functions))
        {
            functions = new List<UnitFunction>();
            _units.Add(name, functions);
        }

        if (functions.Any(f => f.Args.SequenceEqual(args)))
        {
            throw new Exception(name + ' ' + string.Join(' ', args) + " is defined two times.");
        }
        
        functions.Add(new UnitFunction(func, args));
    }
    
    private void AddRulesFunction(string name, Func<object[], IRule> func, ArgType[] args)
    {
        if (!_rules.TryGetValue(name, out List<RuleFunction>? functions))
        {
            functions = new List<RuleFunction>();
            _rules.Add(name, functions);
        }

        if (functions.Any(f => f.Args.SequenceEqual(args)))
        {
            throw new Exception(name + ' ' + string.Join(' ', args) + " is defined two times.");
        }
        
        functions.Add(new RuleFunction(func, args));
    }

    public Cell[] GetCells()
    {
        return _cellTable.Values.ToArray();
    }

    public IEnumerable<Unit> GetUnits(string name, object[] args)
    {
        if (!_units.TryGetValue(name, out List<UnitFunction>? functions))
        {
            throw new Exception($"No unit functions found with name {name}");
        }

        IEnumerable<ArgType> argTypes = GetArgTypes(args.Select(o => o.GetType()));
        UnitFunction? function = functions.Find(f => f.Args.SequenceEqual(argTypes));
        if (function is null)
        {
            throw new Exception("Function not found");
        }

        return function.Delegate.Invoke(args);
    }
    
    public IRule GetRules(string name, object[] args)
    {
        if (!_rules.TryGetValue(name, out List<RuleFunction>? functions))
        {
            throw new Exception($"No unit functions found with name {name}");
        }

        IEnumerable<ArgType> argTypes = GetArgTypes(args.Select(o => o.GetType()));
        RuleFunction? function = functions.Find(f => f.Args.SequenceEqual(argTypes));
        if (function is null)
        {
            throw new Exception("Function not found");
        }

        return function.Delegate.Invoke(args);
    }

    private static IEnumerable<(Func<object[], object>, ArgType[])> GetConstructors(TypeInfo type)
    {
        foreach (ConstructorInfo constructor in type.GetConstructors())
        {
            IEnumerable<Type> paramTypes = constructor.GetParameters().Select(p => p.ParameterType);
            Func<object[], object> func = constructor.Invoke;
            ArgType[] args = GetArgTypes(paramTypes).ToArray();
            
            // Skip constructors with non-valid args.
            if (args.Any(a => a == ArgType.None))
            {
                continue;
            }
            yield return (func, args);
        }
    }

    private static IEnumerable<ArgType> GetArgTypes(IEnumerable<Type> types)
    {
        foreach (Type type in types)
        {
            if (type == typeof(CellReference))
            {
                yield return ArgType.Cell;
            }
            else if (type == typeof(int))
            {
                yield return ArgType.Digit;
            }
            else
            {
                yield return ArgType.None;
            }
        }
    }
}