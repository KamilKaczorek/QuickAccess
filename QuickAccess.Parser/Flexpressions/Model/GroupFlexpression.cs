using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class GroupFlexpression<TConstraint> : FlexpressionGroupBase<TConstraint>
        where TConstraint : IFlexpressionConstraint
    {
        private IFlexpression<TConstraint> _content;
        public override bool IsSealed => false;
        public override IFlexpression<TConstraint> Content => _content;

        public override void SetContent(IFlexpression<TConstraint> content)
        {
            Guard.ArgNotNull(content, nameof(content));
            _content = content;
        }

        public GroupFlexpression(string targetGroupName) : base(targetGroupName)
        {
            Constraint.ValidatePlaceholderAllowed(targetGroupName);
        }
    }
}