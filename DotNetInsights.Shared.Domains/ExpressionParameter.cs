namespace DotNetInsights.Shared.Domains.Enumerations
{
    public class ExpressionParameter
    {
        public ExpressionComparer ExpressionComparer {get;set;}
        public string Name {get;set;}
        public object Value { get; set; }
        public ExpressionCondition Condition {get;set;}
    }
}
