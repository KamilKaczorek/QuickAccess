namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IDefineGroupFlexpression<out TConstraint> : IRepresentGroup
        where TConstraint : IFlexpressionConstraint
    {
        IFlexpression<TConstraint> Content { get; }
        public bool IsSealed { get; }
    }
}