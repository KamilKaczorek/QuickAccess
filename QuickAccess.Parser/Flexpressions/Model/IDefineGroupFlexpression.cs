namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IDefineGroupFlexpression : IRepresentGroup
    {
        IFlexpression Content { get; }
        public bool IsSealed { get; }
    }
}