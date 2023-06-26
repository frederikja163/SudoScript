// using System.Diagnostics;
// using System.Reflection;
// using SudoScript.Core.Data;
//
// namespace SudoScript.Core;
//
// public static class Plugins
// {
//     public delegate IEnumerable<T> UnitFunction<T>(SymbolTable symbolTable, object[] arguments);
//
//     private static readonly Dictionary<string, List<TypeInfo>> _rules = new Dictionary<string, List<TypeInfo>>();
//     // This exists to not create unnessary objects in Plugins.CreateRule.
//     private static readonly Dictionary<string, UnitFunction<IRule>> _ruleFunctions = new Dictionary<string, UnitFunction<IRule>>();
//     private static readonly SymbolTable _emptyTable = new SymbolTable();
//
//     private static readonly Dictionary<string, List<TypeInfo>> _units = new Dictionary<string, List<TypeInfo>>();
//     // TODO: Take a list of UnitFunction here and loop over them to find the correct overload.
//     private static readonly Dictionary<string, UnitFunction<Unit>> _unitFunctions = new Dictionary<string, UnitFunction<Unit>>();
//
//     static Plugins()
//     {
//         string[] files = Directory.GetFiles("./", "*.dll");
//         foreach (string file in files)
//         {
//             AssemblyName name = AssemblyName.GetAssemblyName(file);
//             Assembly assembly;
//             try
//             {
//                 assembly = Assembly.Load(name);
//             }
//             catch (FileNotFoundException)
//             {
//                 assembly = Assembly.LoadFrom(file);
//             }
//
//             foreach (TypeInfo type in assembly.DefinedTypes)
//             {
//                 if (type.ImplementedInterfaces.Contains(typeof(IRule)))
//                 {
//                     if (_rules.TryGetValue(type.Name, out List<TypeInfo>? functions))
//                     {
//                         Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
//                     }
//                     else
//                     {
//                         functions = new List<TypeInfo>();
//                         _rules.Add(type.Name, functions);
//                     }
//                     functions.Add(type);
//                 }
//                 if (type.IsSubclassOf(typeof(Unit)))
//                 {
//                     if (_units.TryGetValue(type.Name, out List<TypeInfo>? functions))
//                     {
//                         Debug.WriteLine($"Warning, multiple definitions of {type.Name}");
//                     }
//                     else
//                     {
//                         functions = new List<TypeInfo>();
//                         _units.Add(type.Name, functions);
//                     }
//                     functions.Add(type);
//                 }
//             }
//         }
//     }
//
//     public static IRule CreateRule(string name, params object[] args)
//     {
//         return Create<IRule>(_rules, _ruleFunctions, _emptyTable, name, args).First();
//     }
//     public static IEnumerable<Unit> CreateUnit(string name, SymbolTable symbolTable, params object[] args)
//     {
//         return Create<Unit>(_units, _unitFunctions, symbolTable, name, args);
//     }
//
//     public static void AddUnitFunction(string name, UnitFunction<Unit> unitFunction)
//     {
//         if (!_unitFunctions.ContainsKey(name) && !_units.ContainsKey(name))
//         {
//             _unitFunctions.Add(name, unitFunction);
//         }
//         else
//         {
//             throw new Exception($"Unit function {name} already defined");
//         }
//     }
//
//     private static IEnumerable<T> Create<T>(
//         IReadOnlyDictionary<string, List<TypeInfo>> typesByName,
//         IReadOnlyDictionary<string, UnitFunction<T>> functions,
//         SymbolTable symbolTable, string name, params object[] args)
//     {
//         bool foundUnits = false;
//
//         if (typesByName.TryGetValue(name, out List<TypeInfo>? types))
//         {
//             foreach (TypeInfo type in types)
//             {
//                 T? obj = default(T);
//                 try
//                 {
//                     obj = (T?)Activator.CreateInstance(type, args);
//                 }
//                 catch
//                 {
//                     // Ignore.
//                 }
//                 if (obj is not null)
//                 {
//                     yield return obj;
//                     foundUnits = true;
//                 }
//             }
//         }
//
//         if (foundUnits)
//         {
//             yield break;
//         }
//
//         if (functions.TryGetValue(name, out UnitFunction<T>? unitFunction))
//         {
//             foreach (T obj in unitFunction.Invoke(symbolTable, args))
//             {
//                 yield return obj;
//                 foundUnits = true;
//             }
//         }
//
//         if (foundUnits)
//         {
//             yield break;
//         }
//
//         IEnumerable<string> argStrings = args.Select(x =>
//             x is int ? "Digit" : x is CellReference ? "Cell" : "None");
//         string argList = string.Join(" ", argStrings);
//         string functionCall = name + " " + argList;
//         // TODO: Add fuzzy search to search fo closest matching function name.
//         throw new Exception($"Found no matching function interface for function call '{functionCall}'");
//     }
// }
