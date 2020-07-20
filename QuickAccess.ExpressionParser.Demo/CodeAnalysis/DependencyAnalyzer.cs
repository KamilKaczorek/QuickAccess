using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuickAccess.ExpressionParser.Demo.CodeAnalysis
{
    public sealed class DependencyAnalyzer
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.Instance |
                                                            BindingFlags.NonPublic |
                                                            BindingFlags.Public;


        private readonly Dictionary<Type, IDependencyAnalysisResult> _analyzed = new Dictionary<Type, IDependencyAnalysisResult>();


        public IReadOnlyDictionary<Type, IDependencyAnalysisResult> ResultByAnalyzedType => _analyzed;

        public IEnumerable<IDependencyAnalysisResult> Analyze(Assembly assembly)
        {
            return assembly.GetTypes().Select(Analyze);
        }

        public IEnumerable<IDependencyAnalysisResult> Analyze(IEnumerable<Type> types)
        {
            return types.Select(Analyze);
        }

        public IDependencyAnalysisResult Analyze(Type type)
        {
            if (_analyzed.TryGetValue(type, out var alreadyAnalyzedResult))
            {
                return alreadyAnalyzedResult;
            }

            var result = new Context();

            AnalyzeType(type, result);

            _analyzed[type] = result;

            return result;
        }



        private void AnalyzeType(Type type, Context context)
        {
            if (type.FullName == null)
            {
                return;
            }

            if (!context.AddReferencedType(type))
            {
                return;
            }

            if (_analyzed.TryGetValue(type, out var analyzed))
            {
                context.AddReferencesOf(analyzed);
                return;
            }

            var types = GetFieldsTypes(type)
                .Concat(GetPropertiesTypes(type))
                .Concat(GetMethodArgsTypes(type))
                .Where(t => t != null && t != typeof(void) && !t.IsSpecialName);

            AnalyzeTypes(types, context);
        }

        private void AnalyzeTypes(IEnumerable<Type> types, Context context)
        {
            foreach (var type in types)
            {
                AnalyzeType(type, context);
            }
        }

        private IEnumerable<Type> GetFieldsTypes(Type type)
        {
            return type.GetFields(BindingFlags).Select(p => p.FieldType);
        }

        private IEnumerable<Type> GetPropertiesTypes(Type type)
        {
            return type.GetProperties().Select(p => p.PropertyType);
        }

        private IEnumerable<Type> GetMethodArgsTypes(Type type)
        {
            var methods = type.GetMethods();

            return methods.SelectMany(pM => pM.GetParameters().Select(p => p.ParameterType)).Concat(methods.Select(pM => pM.ReturnType));
        }


        private sealed class Context : IDependencyAnalysisResult
        {
            private readonly HashSet<Type> _referencedTypes = new HashSet<Type>();

            public Type AnalyzedType { get; private set; }

            public bool AddReferencedType(Type type)
            {
                if (string.IsNullOrWhiteSpace(type?.FullName))
                {
                    return false;
                }

                if (type.FullName.StartsWith("System."))
                {
                    return false;
                }

                if (AnalyzedType == null)
                {
                    AnalyzedType = type;
                    return true;
                }

                if (type == AnalyzedType)
                {
                    return false;
                }

                return _referencedTypes.Add(type);
            }

            int IDependencyAnalysisResult.TotalReferencesCount => _referencedTypes.Count;

            IReadOnlyCollection<Type> IDependencyAnalysisResult.ReferencedTypes => _referencedTypes;

            public void AddReferencesOf(IDependencyAnalysisResult analyzed)
            {
                _referencedTypes.UnionWith(analyzed.ReferencedTypes);
                _referencedTypes.Remove(AnalyzedType);
            }

            public override string ToString()
            {
                return
                    $"{nameof(AnalyzedType)}='{AnalyzedType}', {nameof(IDependencyAnalysisResult.TotalReferencesCount)}={_referencedTypes.Count}";
            }
        }
    }
}