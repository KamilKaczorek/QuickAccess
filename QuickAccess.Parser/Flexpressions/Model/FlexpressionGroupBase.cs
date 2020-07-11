using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public abstract class FlexpressionGroupBase<TConstraint> : Flexpression<TConstraint>, IRepresentGroup
        where TConstraint : IFlexpressionConstraint
    {
        /// <inheritdoc />
        public sealed override string Name => GroupName;
        public string GroupName { get; }
        public bool IsDefined => Content != null;
        public abstract bool IsSealed { get; }

        public abstract IFlexpression<TConstraint> Content { get; }
        public abstract void SetContent(IFlexpression<TConstraint> content);

        protected FlexpressionGroupBase(string groupName)
        {
            Guard.ArgNotNullNorEmpty(groupName, nameof(groupName));
            GroupName = groupName;
        }

        public sealed override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = IsDefined
                ? visitor.VisitGroup(GroupName, Content.AcceptVisitor(visitor))
                : visitor.VisitPlaceholder(GroupName);

            return visitationResult;
        }

        public override string ToString() { return IsDefined ? $"<{Name}=({Content})>" : $"<{GroupName}>"; }
    }
}