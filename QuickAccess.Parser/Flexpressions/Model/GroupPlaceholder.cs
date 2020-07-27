namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class GroupPlaceholder<TConstraint> : Flexpression<TConstraint>, IRepresentGroup
        where TConstraint : IFlexpressionConstraint
    {
        public override string Name => GroupName ?? base.Name;
        public string GroupName { get; }
        public bool IsDefined => false;

  
        public GroupPlaceholder(string groupName)
        {
            GroupName = groupName;
        }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitGroupPlaceholder(GroupName);
            return visitationResult;
        }

        public override string ToString() { return $"<{Name}>"; }
    }
}