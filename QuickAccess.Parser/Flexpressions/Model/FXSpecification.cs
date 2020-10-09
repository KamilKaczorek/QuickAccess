using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickAccess.Infrastructure.CharMatching;
using QuickAccess.Infrastructure.Collections;
using QuickAccess.Infrastructure.Freezable;
using QuickAccess.Infrastructure.ValueContract;
using QuickAccess.Parser.Flexpressions.Model.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IDefineFlexpressionOperators
    {
        IEnumerable<BinaryOperatorDefinition> BinaryOperators { get; }
        IEnumerable<UnaryOperatorDefinition> UnaryOperators { get; }

        BinaryOperatorDefinition GetOperatorDefinition(
            DefinableFxBinaryOperator binaryOperator);

        UnaryOperatorDefinition GetOperatorDefinition(
            DefinableFxUnaryOperator unaryOperator);

        bool IsOperatorDefined(
            DefinableFxBinaryOperator binaryOperator);

        bool IsOperatorDefined(
            DefinableFxUnaryOperator unaryOperator);
    }

    public readonly struct BinaryOperatorDefinition : IEquatable<BinaryOperatorDefinition>, IComparable<BinaryOperatorDefinition>, ICanBeUndefined
    {
        public static BinaryOperatorDefinition Create(
            DefinableFxBinaryOperator @operator,
            Func<Flexpression, Flexpression, Flexpression> expression)
        {
            return new BinaryOperatorDefinition(@operator, expression);
        }

        public BinaryOperatorDefinition(DefinableFxBinaryOperator @operator, Func<Flexpression, Flexpression, Flexpression> expression)
        {
            Operator = @operator;
            Expression = expression;
        }


        public DefinableFxBinaryOperator Operator { get; }
        public Func<Flexpression, Flexpression, Flexpression> Expression { get; }
        public bool IsDefined => Expression != null;
        public bool Equals(BinaryOperatorDefinition other)
        {
            return Operator == other.Operator && Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryOperatorDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Operator;
        }

        public static bool operator ==(BinaryOperatorDefinition left, BinaryOperatorDefinition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BinaryOperatorDefinition left, BinaryOperatorDefinition right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(BinaryOperatorDefinition other)
        {
            return Operator.CompareTo(other.Operator);
        }

        
    }

    public readonly struct UnaryOperatorDefinition : IEquatable<UnaryOperatorDefinition>, IComparable<UnaryOperatorDefinition>, ICanBeUndefined
    {
        public static UnaryOperatorDefinition Create(
            DefinableFxUnaryOperator @operator,
            Func<Flexpression, Flexpression> expression)
        {
            return new UnaryOperatorDefinition(@operator, expression);
        }

        public UnaryOperatorDefinition(DefinableFxUnaryOperator @operator, Func<Flexpression, Flexpression> expression)
        {
            Operator = @operator;
            Expression = expression;
        }

        public DefinableFxUnaryOperator Operator { get; }
        public Func<Flexpression, Flexpression> Expression { get; }

        public bool IsDefined => Expression != null;

        public bool Equals(UnaryOperatorDefinition other)
        {
            return Operator == other.Operator && Equals(Expression, other.Expression);
        }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperatorDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)Operator;
        }

        public static bool operator ==(UnaryOperatorDefinition left, UnaryOperatorDefinition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UnaryOperatorDefinition left, UnaryOperatorDefinition right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(UnaryOperatorDefinition other)
        {
            return Operator.CompareTo(other.Operator);
        }

      
    }


    public interface IFlexpressionOperatorsDefinition : IDefineFlexpressionOperators
    {
        Func<Flexpression, Flexpression, Flexpression> this[OperatorSelection.BinarySelector operatorSelector] { get; set; }
        Func<Flexpression, Flexpression, Flexpression> this[DefinableFxBinaryOperator binaryOperator] { get; set; }

        Func<Flexpression, Flexpression> this[OperatorSelection.UnarySelector operatorSelector] { get; set; }
        Func<Flexpression, Flexpression> this[DefinableFxUnaryOperator unaryOperator] { get; set; }

        void DefineOperator(BinaryOperatorDefinition definition);
        void DefineOperator(UnaryOperatorDefinition definition);
    }

    public class FXSpecification
    {
        [Pure]
        public static FXSpecification Create()

        {
            return new FXSpecification(new Dictionary<string, GroupFlexpressionDefinition>());
        }

        [Pure]
        public static FXSpecification Create(IEqualityComparer<string> groupNameComparer)

        {
            return new FXSpecification(
                new Dictionary<string, GroupFlexpressionDefinition>(groupNameComparer));
        }

        [Pure]
        public static FXSpecification Create(
            IEnumerable<IDefineGroupFlexpression> predefinedOverwritableGroups,
            IEnumerable<IDefineGroupFlexpression> predefinedSealedGroups,
            IEqualityComparer<string> groupNameComparer = null)

        {
            predefinedOverwritableGroups ??= Array.Empty<IDefineGroupFlexpression>();
            predefinedSealedGroups ??= Array.Empty<IDefineGroupFlexpression>();

            groupNameComparer ??= StringComparer.Ordinal;

            var groups = predefinedOverwritableGroups
                         .Where(p => p.IsDefined)
                         .Select(p =>
                             new GroupFlexpressionDefinition(p.GroupName,
                                 AutoFreezingValue.CreateDefinedNotFrozen(p.Content)))
                         .Concat(predefinedSealedGroups
                                 .Where(p => p.IsDefined)
                                 .Select(p =>
                                     new GroupFlexpressionDefinition(p.GroupName,
                                         AutoFreezingValue.CreateDefinedFrozen(p.Content), isSealed: true)))
                         .ToDictionary(
                             pK => pK.GroupName,
                             pV => pV,
                             groupNameComparer);

            var res = new FXSpecification(groups);
            return res;
        }

        public static FXSpecification Create(
            IEnumerable<IDefineGroupFlexpression> predefinedGroups,
            IEqualityComparer<string> groupNameComparer = null)

        {
            var groupsBySealed = predefinedGroups.ToLookup(p => p.IsSealed);

            var res = Create(groupsBySealed[false], groupsBySealed[true], groupNameComparer);
            return res;
        }

        private readonly Dictionary<string, GroupFlexpressionDefinition> _groupsDefinitionsByName;
        private readonly Dictionary<string, GroupPlaceholder> _groupsPlaceholdersByName;

        internal FXSpecification(Dictionary<string, GroupFlexpressionDefinition> groupsByName)
        {
            _groupsDefinitionsByName = groupsByName ?? new Dictionary<string, GroupFlexpressionDefinition>();
            _groupsPlaceholdersByName = _groupsDefinitionsByName.Values.ToDictionary(pK => pK.GroupName,
                pV => new GroupPlaceholder(pV.GroupName));
        }

        public Flexpression Text(string str)
        {
            return StringFlexpression.Create(str);
        }

        public Flexpression DefineGroup(string groupName, IFlexpression content)
        {
            var group = _groupsDefinitionsByName.GetExistingValueOrNew(
                groupName,
                pName => new GroupFlexpressionDefinition(pName, AutoFreezingValue.CreateUndefined<IFlexpression>()));

            if (content != null)
            {
                group.ContentContainer.Set(content);
            }

            return group;
        }

        public Flexpression this[string groupName]
        {
            get => _groupsPlaceholdersByName.GetExistingValueOrNew(groupName, pName => new GroupPlaceholder(pName));
            set => DefineGroup(groupName, value);
        }

        public Flexpression Char(char c)
        {
            return CharactersRangeFlexpression.Create(CharactersRangeDefinition.CreateMatching(c));
        }

        private readonly Dictionary<DefinableFxBinaryOperator, Func<Flexpression, Flexpression, Flexpression>> _binOperatorDefinitions = new Dictionary<DefinableFxBinaryOperator, Func<Flexpression, Flexpression, Flexpression>>();
        private readonly Dictionary<DefinableFxUnaryOperator, Func<Flexpression, Flexpression>> _unaryOperatorDefinitions = new Dictionary<DefinableFxUnaryOperator, Func<Flexpression, Flexpression>>();

        public void DefineOperator(DefinableFxBinaryOperator binaryOperator, Func<Flexpression, Flexpression, Flexpression> definition)
        {
            _binOperatorDefinitions[binaryOperator] = definition;
        }

        public void DefineOperator(DefinableFxUnaryOperator unaryOperator, Func<Flexpression, Flexpression> definition)
        {
            _unaryOperatorDefinitions[unaryOperator] = definition;
        }

        public Func<Flexpression, Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            DefinableFxBinaryOperator binaryOperator)
        {
            return _binOperatorDefinitions.GetOperatorDefinitionOrNull(binaryOperator);
        }

        public Func<Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            DefinableFxUnaryOperator unaryOperator)
        {
            return _unaryOperatorDefinitions.GetOperatorDefinitionOrNull(unaryOperator);
        }

        public Func<Flexpression, Flexpression, Flexpression> this[OperatorSelection.BinarySelector operatorSelector]
        {
            set => _binOperatorDefinitions[operatorSelector.EvaluateOperator()] = value;
            get => _binOperatorDefinitions.GetOperatorDefinitionOrNull(operatorSelector);
        }

        public Func<Flexpression, Flexpression> this[OperatorSelection.UnarySelector operatorSelector]
        {
            set => _unaryOperatorDefinitions[operatorSelector.EvaluateOperator()] = value;
            get => _unaryOperatorDefinitions.GetOperatorDefinitionOrNull(operatorSelector);
        }

        public Func<Flexpression, Flexpression, Flexpression> this[DefinableFxBinaryOperator binaryOperator]
        {
            set => _binOperatorDefinitions[binaryOperator] = value;
            get => _binOperatorDefinitions.GetOperatorDefinitionOrNull(binaryOperator);
        }

        public Func<Flexpression, Flexpression> this[DefinableFxUnaryOperator unaryOperator]
        {
            set => _unaryOperatorDefinitions[unaryOperator] = value;
            get => _unaryOperatorDefinitions.GetOperatorDefinitionOrNull(unaryOperator);
        }
    }
}