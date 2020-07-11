using System;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class SealedGroupFlexpression<TConstraint> : FlexpressionGroupBase<TConstraint>
        where TConstraint : IFlexpressionConstraint
    {
        public override bool IsSealed => true;
        public override IFlexpression<TConstraint> Content { get; }

        public SealedGroupFlexpression(string groupName, IFlexpression<TConstraint> content) : base(groupName)
        {
            Guard.ArgNotNull(content, nameof(content));
            Constraint.ValidateGroupAllowed(groupName);
            Content = content;
        }

        public override void SetContent(IFlexpression<TConstraint> content)
        {
            throw new NotSupportedException("SetContent operation is not supported by the sealed group.");
        }
    }
}