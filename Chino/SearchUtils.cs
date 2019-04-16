using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RestSharp;

namespace Chino
{
    public static class Constants
    {
        public static readonly int SearchResultsDefaultLimit = 10;

    }

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

        public string toJSON()
        {
            switch (_enumValue)
            {
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
                    throw new NotSupportedException($"{_enumValue} is not a valid FilterOperator");
            }
        }

        public override string ToString()
        {
            switch (_enumValue)
            {
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
                    throw new NotSupportedException($"{_enumValue} is not a valid FilterOperator");
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

        public string toString()
        {
            return _enumValue.ToString().ToLower();
        }
    }

    public class SortRule
    {

        private string fieldName;
        private Order order;

        public SortRule(string field, Order order)
        {
            fieldName = field;
            this.order = order;
        }

        public SortRule(string field, OrderEnum order)
            : this(field, new Order(order)) { }

        public StringBuilder toJSONString(int indentLevel)
        {
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

        private static StringBuilder indent(StringBuilder sb, int level)
        {
            for (var i = 0; i < level; i++)
            {
                sb.Append("\t");
            }

            return sb;
        }
    }

    public abstract class AbstractSearchClient<TResponseType>
    {

        private ResultType resultType = new ResultType(ResultTypeEnum.FullContent);
        private ISearchTreeNode query;
        private LinkedList<SortRule> sort;

        private readonly RestClient _client; // use new Request(..., POST) to POST
        protected string resourceID;

//    protected static readonly ObjectMapper mapper = new ObjectMapper(); // use ((Newtonsoft.Json.Linq.***)<<OBJECT>>.ToObject<***>()

        protected AbstractSearchClient(RestClient client, string resourceID)
        {
            _client = client;
            this.resourceID = resourceID;
        }

        public AbstractSearchClient<TResponseType> setQuery(ISearchTreeNode query)
        {
            this.query = query;
            return this;
        }

        protected AbstractSearchClient<TResponseType> setResultType(ResultType resultType)
        {
            this.resultType = resultType;
            return null;
        }

        protected AbstractSearchClient<TResponseType> addSortRule(string fieldName, Order order)
        {
            if (sort == null)
            {
                sort = new LinkedList<SortRule>();
            }

            sort.AddLast(
                new SortRule(fieldName, order)
            );
            return null;
        }

        protected AbstractSearchClient<TResponseType> addSortRule(string fieldName, Order order, int index)
        {
            if (sort == null)
            {
                sort = new LinkedList<SortRule>();
            }

            if (index > sort.Count)
            {
                sort.AddLast(new SortRule(fieldName, order));
            }
            else
            {
                var elemAtIndex = sort.ElementAt(index);
                sort.AddBefore(
                    new LinkedListNode<SortRule>(elemAtIndex),
                    new SortRule(fieldName, order)
                );
            }

            return null;
        }

        public SearchQueryBuilder<TResponseType> with(SearchQueryBuilder<TResponseType> searchQuery)
        {
            searchQuery.setClient(this);
            return searchQuery;
        }

        public SearchQueryBuilder<TResponseType> with(string fieldName, FilterOperator type, int value)
        {
            return new SearchQueryBuilder<TResponseType>(new IntegerSearchLeaf(fieldName, type, value), this);
        }

        public SearchQueryBuilder<TResponseType> with(string fieldName, FilterOperator type, float value)
        {
            return new SearchQueryBuilder<TResponseType>(new FloatSearchLeaf(fieldName, type, value), this);
        }

        public SearchQueryBuilder<TResponseType> with(string fieldName, FilterOperator type, bool value)
        {
            return new SearchQueryBuilder<TResponseType>(new BoolSearchLeaf(fieldName, type, value), this);
        }

        public SearchQueryBuilder<TResponseType> with(string fieldName, FilterOperator type, string value)
        {
            return new SearchQueryBuilder<TResponseType>(new StringSearchLeaf(fieldName, type, value), this);
        }

        public SearchQueryBuilder<TResponseType> with<TArrayType>(string fieldName, FilterOperator type, List<TArrayType> value)
        {
            if (value == null)
            {
                return with(fieldName, type, null);
            }

            var arraySearchLeaf = getArraySearchLeaf(fieldName, type, value);

            return new SearchQueryBuilder<TResponseType>(arraySearchLeaf, this);
        }

        private static ArraySearchLeaf<TArrayType> getArraySearchLeaf<TArrayType>(string fieldName, FilterOperator type,
            List<TArrayType> value)
        {
            if (value.Count == 0)
                return new ArraySearchLeaf<TArrayType>(fieldName, type, new List<TArrayType>());

            object element = value[0];
            if (!(IsNumber(element) || element is bool || element is string))
                throw new NotSupportedException(
                    $"Unsupported element in list of type '{element.GetType()}'. " +
                    "Supported types are: decimal, short, ushort, int, uint, long, ulong, float, double"
                );

            return new ArraySearchLeaf<TArrayType>(fieldName, type, value);
        }

        public override string ToString()
        {
            return query.getString().ToString();
        }

        public string toJsonString()
        {
            return parseSearchRequest();
        }

        protected string parseSearchRequest()
        {
            var queryJson = query.parseJson(2);

            /* indented by 0 */
            var sb = new StringBuilder("{\n");

            /* indented by 1 */
            // write resultType field in JSON
            sb.Append("\t").Append("\"result_type\": \"").Append(resultType).Append("\"").Append(",\n");
            // write list of SortRules in JSON
            if (sort != null && sort.Count != 0)
            {
                sb.Append("\t").Append("\"sort\": ").Append("[\n");
                IEnumerator<SortRule> en = sort.GetEnumerator();
                var hasNext = en.MoveNext();
                while (hasNext)
                {
                    if (en.Current != null)
                    {
                        sb.Append(en.Current.toJSONString(2));
                    }

                    hasNext = en.MoveNext(); // check if the array is finished
                    if (hasNext)
                    {
                        sb.Append(",");
                    }

                    sb.Append("\n");
                }
                en.Dispose();

                sb.Append("\t").Append("],\n");
            }

            // write parsed query JSON
            sb.Append("\t").Append("\"query\": ").Append(queryJson);
            return sb.Append("}\n").ToString();
        }

        public abstract TResponseType execute(int offset, int limit);

        public TResponseType execute()
        {
            return execute(0, Constants.SearchResultsDefaultLimit);
        }

        public static bool IsNumber(object value)
        {
            return value is decimal
//                   || value is byte
//                   || value is sbyte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double;
        }

    }

    public abstract class SearchCondition : ISearchTreeNode
    {

        protected readonly List<ISearchTreeNode> childTreeNodes;
        protected readonly string conditionOperator;

        protected SearchCondition(string operatorText)
        {
            childTreeNodes = new List<ISearchTreeNode>();
            conditionOperator = operatorText;
        }

        protected SearchCondition(string operatorText, IReadOnlyCollection<ISearchTreeNode> clauses) : this(
            operatorText)
        {
            if (clauses.Count == 0)
                return;

            foreach (var clause in clauses)
            {
                addChild(clause);
            }
        }

        StringBuilder ISearchTreeNode.getString()
        {
            var sb = new StringBuilder("(");
            var en = childTreeNodes.GetEnumerator();
            var hasNext = en.MoveNext();
            if (hasNext)
            {
                // if list is not empty, read first element
                var elem = en.Current;
                if (elem != null)
                {
                    sb.Append(elem.getString());
                }
                else
                {
                    sb.Append("null");
                }

                while (en.MoveNext())
                {
                    // append string representation of the other list elements, if any
                    elem = en.Current;
                    if (elem != null)
                    {
                        sb.Append(" ")
                            .Append(conditionOperator.ToUpper())
                            .Append(" ")
                            .Append(elem.getString());
                    }
                    else
                    {
                        sb.Append(" ").Append("null");
                    }
                }
            }

            en.Dispose();
            return sb.Append(")");
        }

        protected static StringBuilder indent(StringBuilder sb, int level)
        {
            for (var i = 0; i < level; i++)
            {
                sb.Append("\t");
            }

            return sb;
        }

        string ISearchTreeNode.parseJson(int indentLevel)
        {
            var sb = new StringBuilder("{\n");
            indent(sb, indentLevel + 1).Append("\"").Append(conditionOperator).Append("\" : [\n");
            var en = childTreeNodes.GetEnumerator();
            var hasNext = en.MoveNext();
            if (hasNext)
            {
                // first element
                if (en.Current != null)
                {
                    indent(sb, indentLevel + 2).Append(en.Current.parseJson(indentLevel + 3));
                }
                else
                {
                    sb.Append("null");
                }

                while (en.MoveNext())
                {
                    var childNode = en.Current;
                    indent(sb, indentLevel + 2).Append(",\n");
                    indent(sb, indentLevel + 2).Append(
                        childNode != null
                            ? childNode.parseJson(indentLevel + 3)
                            : "null"
                    );
                }
            }

            en.Dispose();
            indent(sb, indentLevel + 1).Append("]\n");
            return indent(sb, indentLevel).Append("}\n").ToString();
        }

        public ReadOnlyCollection<ISearchTreeNode> getChildren()
        {
            return childTreeNodes.AsReadOnly();
        }

        public virtual void addChild(ISearchTreeNode newChild)
        {
            childTreeNodes.Add(newChild);
        }

        public virtual void removeChild(ISearchTreeNode child)
        {
            childTreeNodes.Remove(child);
        }

        public override bool Equals(object obj)
        {
            if (this == obj || this.Equals(obj))
                return true;
            if (!(obj is SearchCondition))
                return false;

            var other = (SearchCondition) obj;
            if (!other.conditionOperator.Equals(conditionOperator))
                return false;

            foreach (var node in other.childTreeNodes)
            {
                if (!childTreeNodes.Contains(node))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            string childrenCount = childTreeNodes.Count + "";
            string operatorHash = (conditionOperator.GetHashCode() % 1000).ToString();
            string childrenHash = "";

            var tmpHash = 0;
            foreach (var searchTreeNode in childTreeNodes)
            {
                tmpHash += searchTreeNode.GetHashCode() % 100;
            }

            childrenHash += tmpHash;

            return int.Parse(childrenCount + operatorHash + childrenHash);
        }


        public class And : SearchCondition
        {
            public And() : base("and") { }
            public And(IReadOnlyCollection<ISearchTreeNode> clauses) : base("and", clauses) { }

            public override void addChild(ISearchTreeNode newChild)
            {
                if (newChild is And and)
                {
                    childTreeNodes.AddRange(and.childTreeNodes);
                }
                else
                {
                    childTreeNodes.Add(newChild);
                }
            }

            public override void removeChild(ISearchTreeNode child)
            {
                childTreeNodes.Remove(child);
            }
        }

        public class Or : SearchCondition
        {
            public Or() : base("or") { }
            public Or(IReadOnlyCollection<ISearchTreeNode> clauses) : base("or", clauses) { }

            public override void addChild(ISearchTreeNode newChild)
            {
                if (newChild is Or or)
                {
                    childTreeNodes.AddRange(or.childTreeNodes);
                }
                else
                {
                    childTreeNodes.Add(newChild);
                }
            }

            public override void removeChild(ISearchTreeNode child)
            {
                childTreeNodes.Remove(child);
            }
        }

        public class Not : SearchCondition
        {

            public Not() : base("not") { }

            public Not(ISearchTreeNode element) : this()
            {
                setChild(element);
            }

            // Not a duplicate of setChild!!! This method is required because it overrides
            // the default behaviour of the virtual method SearchCondition.addChild()
            public override void addChild(ISearchTreeNode newChild)
            {
                setChild(newChild);
            }

            public override void removeChild(ISearchTreeNode child)
            {
                if (childTreeNodes.Count != 0 && childTreeNodes.Contains(child))
                    childTreeNodes.Clear();
            }

            private void resetChildren()
            {
                childTreeNodes.Clear();
            }

            private void setChild(ISearchTreeNode child)
            {
                if (childTreeNodes.Count != 0)
                    resetChildren();
                if (child != null)
                    setChildInternal(child);
            }

            private void setChildInternal(ISearchTreeNode newChild)
            {
                if (childTreeNodes.Count == 0)
                    childTreeNodes.Add(newChild);
            }

            public ISearchTreeNode getChild()
            {
                return childTreeNodes.Count != 0
                    ? childTreeNodes[0]
                    : null;
            }

            public StringBuilder getString()
            {
                StringBuilder sb = new StringBuilder("(");
                ISearchTreeNode child = getChild();
                if (child != null)
                {
                    sb.Append(conditionOperator.ToUpper())
                        .Append(" ")
                        .Append(child.getString());
                }
                else
                {
                    sb.Append("<ERROR! 'NOT' can't be applied to 'null'>");
                }

                return sb.Append(")");
            }
        }
    }

    public class SearchQueryBuilder<TClientResponseType>
    {
        
        private readonly ISearchTreeNode treeTop;

        private AbstractSearchClient<TClientResponseType> queryExecutor;
        
        public SearchQueryBuilder (ISearchTreeNode rootNode, AbstractSearchClient<TClientResponseType> client)
        {
            treeTop = rootNode;
            queryExecutor = client.setQuery(rootNode);
        }

        protected SearchQueryBuilder(ISearchTreeNode query1, SearchCondition cond, ISearchTreeNode query2, AbstractSearchClient<TClientResponseType> client) {
            treeTop = cond;
            ((SearchCondition) treeTop).addChild(query1);
            ((SearchCondition) treeTop).addChild(query2);
            queryExecutor = client;
        }
        
        public void setClient(AbstractSearchClient<TClientResponseType> searchClient)
        {
            queryExecutor = searchClient;
        }
    }

    public class DocumentsSearch : AbstractSearchClient<GetDocumentsResponse> {
        public DocumentsSearch(RestClient client, string resourceId) : base(client, resourceId) { }
        public override GetDocumentsResponse execute(int offset, int limit)
        {
            throw new NotImplementedException();
        }
    }

    public class UsersSearch : AbstractSearchClient<GetUsersResponse> {
        public UsersSearch(RestClient client, string resourceId) : base(client, resourceId) { }
        public override GetUsersResponse execute(int offset, int limit)
        {
            throw new NotImplementedException();
        }
    }
}