using System;

namespace Chino
{
    public enum FilterOperatorEnum
    {
        Equals,
        GreaterEqual,
        GreaterThan,
        In,
        Is,
        Like,
        LowerEqual,
        LowerThan
    }
        
    public class FilterOperator
    {
        private readonly FilterOperatorEnum _enumValue;

        public FilterOperator(FilterOperatorEnum enumValue)
        {
            _enumValue = enumValue;
        }
        
        public string toJson() {
            switch (_enumValue) {
                case FilterOperatorEnum.Equals:
                    return "eq";
                case FilterOperatorEnum.LowerThan:
                    return "lt";
                case FilterOperatorEnum.LowerEqual:
                    return "lte";
                case FilterOperatorEnum.GreaterThan:
                    return "gt";
                case FilterOperatorEnum.GreaterEqual:
                    return "gte";
                case FilterOperatorEnum.Is:
                case FilterOperatorEnum.In:
                case FilterOperatorEnum.Like:
                    return _enumValue.ToString().ToLower();
                default:
                    throw new NotSupportedException($"{ _enumValue } is not a valid FilterOperator");
            }
        }
        
        public override string ToString() {
            switch (_enumValue) {
                case FilterOperatorEnum.Equals:
                    return "=";
                case FilterOperatorEnum.LowerThan:
                    return "<";
                case FilterOperatorEnum.LowerEqual:
                    return "<=";
                case FilterOperatorEnum.GreaterThan:
                    return ">";
                case FilterOperatorEnum.GreaterEqual:
                    return ">=";
                case FilterOperatorEnum.Like:
                    return "matches";
                case FilterOperatorEnum.Is:
                case FilterOperatorEnum.In:
                    return _enumValue.ToString().ToLower();
                default:
                    throw new NotSupportedException($"{ _enumValue } is not a valid FilterOperator");
            }
        }
    }
    
    public enum ResultTypeEnum 
    {
        FullContent,
        NoContent,
        OnlyId,
        Count,
        Exists,
        UsernameExists
    }

    public class ResultType
    {
        private readonly ResultTypeEnum _enumValue;

        public ResultType(ResultTypeEnum enumValue)
        {
            _enumValue = enumValue;
        }

        public override string ToString()
        {
            return _enumValue.ToString().ToLower();
        }
    }
}