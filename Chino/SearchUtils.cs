using System;
using System.Text;

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
        
        public string toJSON() {
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

    public interface ISearchTreeNode
    {
        StringBuilder getString();
        string parseJson(int indentLevel);
    }



    public enum OrderEnum
    {
        /* ascending order */
        ASC,

        /* descending order */
        DESC
    }

    public class Order
    {
        private readonly OrderEnum _enumValue;

        public Order(OrderEnum enumValue)
        {
            _enumValue = enumValue;
        }

        public string toString() {
            return _enumValue.ToString().ToLower();
        }
    }

    public class SortRule {

        private string fieldName;
        private Order order;

        SortRule(string field, Order order) {
            fieldName = field;
            this.order = order;
        }

        SortRule(string field, OrderEnum order) 
            : this(field, new Order(order)) {}

        public StringBuilder toJSONString(int indentLevel) {
            var sb = indent(new StringBuilder(), indentLevel).Append("{\n");
            indent(sb, indentLevel)
                .Append("\t")
                .Append("\"field\": ").Append("\"").Append(fieldName).Append("\",\n");
            indent(sb, indentLevel)
                .Append("\t")
                .Append("\"order\": ").Append("\"").Append(order.ToString()).Append("\"\n");
            indent(sb, indentLevel)
                .Append("}");

            return sb;
        }

        private static StringBuilder indent(StringBuilder sb, int level) {
            for(var i=0; i<level; i++) {
                sb.Append("\t");
            }
            return sb;
        }
    }
    
}