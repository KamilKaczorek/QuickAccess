namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class StringFlexpression : Flexpression 
    {
        public static Flexpression Create(string content)

        {
            return new StringFlexpression(content);
        }

        public override string Name => $"\"{Content}\"";
        public StringFlexpression(string content) { Content = content; }
        public string Content { get; }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            return visitor.VisitString(Content);
        }

        public override int GetHashCode() { return string.IsNullOrEmpty(Content) ? 0 : Content.GetHashCode(); }
    }

    
}