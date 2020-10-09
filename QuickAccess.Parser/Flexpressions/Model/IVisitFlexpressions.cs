using System;
using System.Collections.Generic;
using QuickAccess.Infrastructure.Algebra;
using QuickAccess.Infrastructure.CharMatching;
using QuickAccess.Infrastructure.Patterns.Visitor;

namespace QuickAccess.Parser.Flexpressions.Model
{
    
    public interface IVisitFlexpressions<TVisitationResult> : IVisit<TVisitationResult>
    {
        TVisitationResult VisitString(string str);

        TVisitationResult VisitCharactersRange(ICharactersRangeDefinition range);

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