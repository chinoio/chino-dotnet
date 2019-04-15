using System;

namespace Chino
{
    public enum FilterOperatorEnum
    {
        EQUALS,
        GREATER_EQUAL,
        GREATER_THAN,
        IN,
        IS,
        LIKE,
        LOWER_EQUAL,
        LOWER_THAN
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
                case FilterOperatorEnum.EQUALS:
                    return "eq";
                case FilterOperatorEnum.LOWER_THAN:
                    return "lt";
                case FilterOperatorEnum.LOWER_EQUAL:
                    return "lte";
                case FilterOperatorEnum.GREATER_THAN:
                    return "gt";
                case FilterOperatorEnum.GREATER_EQUAL:
                    return "gte";
                case FilterOperatorEnum.IS:
                case FilterOperatorEnum.IN:
                case FilterOperatorEnum.LIKE:
                    return _enumValue.ToString().ToLower();
                default:
                    throw new NotSupportedException($"{ _enumValue } is not a valid FilterOperator");
            }
        }
        
        public override string ToString() {
            switch (_enumValue) {
                case FilterOperatorEnum.EQUALS:
                    return "=";
                case FilterOperatorEnum.LOWER_THAN:
                    return "<";
                case FilterOperatorEnum.LOWER_EQUAL:
                    return "<=";
                case FilterOperatorEnum.GREATER_THAN:
                    return ">";
                case FilterOperatorEnum.GREATER_EQUAL:
                    return ">=";
                case FilterOperatorEnum.LIKE:
                    return "matches";
                case FilterOperatorEnum.IS:
                case FilterOperatorEnum.IN:
                    return _enumValue.ToString().ToLower();
                default:
                    throw new NotSupportedException($"{ _enumValue } is not a valid FilterOperator");
            }
        }
    }
    
    public enum ResultTypeEnum 
    {
        FULL_CONTENT,
        NO_CONTENT,
        ONLY_ID,
        COUNT,
        EXISTS,
        USERNAME_EXISTS
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