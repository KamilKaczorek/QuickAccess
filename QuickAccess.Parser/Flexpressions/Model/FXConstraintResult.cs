namespace QuickAccess.Parser.Flexpressions.Model
{
    /// <summary>
    /// Defines result of domain constraint operation (or operator) allowance.
    /// </summary>
    public enum FXConstraintResult
    {
        /// <summary>
        /// Operation or specific operator is allowed in the current FX domain.
        /// </summary>
        Allowed = 0,

        /// <summary>
        /// Operation or specific operator is not allowed in the current FX domain.
        /// </summary>
        OperationNotAllowed,

        /// <summary>
        /// Operation or specific operator is not allowed for the given argument in the current FX domain.
        /// </summary>
        InvalidOperationArguments,
    }
}