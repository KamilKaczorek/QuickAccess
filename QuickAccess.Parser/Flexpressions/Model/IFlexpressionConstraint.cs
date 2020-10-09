using JetBrains.Annotations;
using QuickAccess.Infrastructure.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    /// <summary>
    /// Allows to determine operations and operator constrains for building flexpressions (FX).
    /// </summary>
    public interface IFlexpressionConstraint
    {
        /// <summary>
        /// Determines whether the char operator is allowed for the specified character.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsCharAllowed(char ch);

        /// <summary>
        /// Determines whether the string operator is allowed for the specified text.
        /// </summary>
        /// <param name="str">The text.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsStringAllowed(string str);

        /// <summary>
        /// Determines whether the quantifier ([count], [min,max]) is allowed for the specified ranges. 
        /// </summary>
        /// <param name="min">The min quantification.</param>
        /// <param name="max">The max quantification.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsQuantifierAllowed(long min, long max);

        /// <summary>
        /// Determines whether the specified unary operator is allowed.
        /// </summary>
        /// <param name="unaryOperator">Unary operator.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator);

        /// <summary>
        /// Determines whether the specified binary operator is allowed for the specified number of arguments.
        /// </summary>
        /// <param name="binaryOperator">Binary operator.</param>
        /// <param name="argsCount">Number of arguments (concatenated).</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount);

        /// <summary>
        /// Determines whether specified operation is allowed for the specified number of arguments.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="argsCount">The number of arguments.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsOperationAllowed(string operationName, int argsCount);

        /// <summary>
        /// Determines whether specified group is allowed.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsGroupAllowed(string groupName);

        /// <summary>
        /// Determines whether specified group placeholder is allowed.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <returns>Validation result.</returns>
        [Pure] FXConstraintResult IsPlaceholderAllowed(string groupName);
    }
}