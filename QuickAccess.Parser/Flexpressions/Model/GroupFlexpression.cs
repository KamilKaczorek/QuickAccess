using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class GroupFlexpression<TConstraint> : Flexpression<TConstraint>, IRepresentGroup
        where TConstraint : IFlexpressionConstraint
    {
        public override string Name => GroupName ?? base.Name;
        public string GroupName { get; }
        public bool IsDefined => ContentContainer.IsDefined;

        public IEditableValue<IFlexpression<TConstraint>> ContentContainer { get; }
        public IFlexpression<TConstraint> Content => ContentContainer.Value;
  
        public GroupFlexpression(string groupName, IEditableValue<IFlexpression<TConstraint>> contentContainer)
        {
            Guard.ArgNotNull(contentContainer, nameof(contentContainer));

            GroupName = groupName;
            ContentContainer = contentContainer;
        }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = ContentContainer.TryGetValue(out var contentFlexpression)
                ? visitor.VisitGroup(GroupName, contentFlexpression.AcceptVisitor(visitor))
                : visitor.VisitGroupPlaceholder(GroupName);

            return visitationResult;
        }

        public override string ToString() { return IsDefined ? $"<{Name}=({Content})>" : $"<{Name}>"; }
    }
}