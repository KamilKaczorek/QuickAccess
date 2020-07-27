using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class GroupFlexpressionDefinition<TConstraint> : Flexpression<TConstraint>, IDefineGroupFlexpression<TConstraint>
        where TConstraint : IFlexpressionConstraint
    {
        public override string Name => GroupName ?? base.Name;
        public string GroupName { get; }
        public bool IsDefined => ContentContainer.IsDefined;

        public IEditableValue<IFlexpression<TConstraint>> ContentContainer { get; }
        public IFlexpression<TConstraint> Content => ContentContainer.Value;

        public bool IsSealed { get; set; }

        public GroupFlexpressionDefinition(string groupName,
            IEditableValue<IFlexpression<TConstraint>> contentContainer, bool isSealed = false)
        {
            Guard.ArgNotNull(contentContainer, nameof(contentContainer));

            IsSealed = isSealed;
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