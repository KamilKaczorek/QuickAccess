using System.Collections.Generic;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IVisitFlexpressions<TVisitationResult>
    {
        TVisitationResult VisitChar(char ch);

        TVisitationResult VisitString(string str);

        TVisitationResult VisitQuantifier(TVisitationResult contentVisitationResult, long min, long max);

        TVisitationResult VisitUnaryOperator(
            OverloadableCodeUnarySymmetricOperator unaryOperator,
            TVisitationResult argVisitationResult
        );

        TVisitationResult VisitBinaryOperator(
            OverloadableCodeBinarySymmetricOperator binaryOperator,
            IEnumerable<TVisitationResult> argsVisitationResults, 
            int argsCount);

        TVisitationResult VisitOperation(
            string operationName,
            IEnumerable<TVisitationResult> argsVisitationResults, 
            int argsCount);

        TVisitationResult VisitGroup(
            string groupName,
            TVisitationResult contentVisitationResult);

        TVisitationResult VisitGroupPlaceholder(string targetGroupName);

        TVisitationResult VisitCustom(IFlexpression customFlexpression);

    }
}