using System;

namespace MergeGen.Core
{
    public partial class MergeCriterion
    {
        public string FieldName {get;set;}
        public ConditionOperator Comparison   {get;set;}
    }
}