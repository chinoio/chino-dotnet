using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RestSharp.Extensions;

namespace Chino
{
    public abstract class SearchLeaf<TValueType> : ISearchTreeNode
    {
        private string _field;
        private TValueType _value;

        protected string field
        {
            get => _field;
            set => _field = value;
        }

        protected FilterOperator type { get; private set; }

        protected TValueType value
        {
            get => _value;
            set => _value = value;
        }

        protected SearchLeaf (string field, FilterOperator type, TValueType value) {
            this.field = field;
            this.type = type;
            this.value = value;
        }

        
        
        public void setType(FilterOperatorEnum enumType) {
            type = new FilterOperator(enumType);
        }

        public virtual StringBuilder getString() {
            var sb = new StringBuilder("{");

            if (field == null || type == null) {
                sb.Append("ERROR! ")
                    .Append(
                        (field == null) ? "'field'"
                            : "'type'"
                    )
                    .Append(" can't be 'null'");
            } else {
                sb.Append(field).Append(" ")
                    .Append(type).Append(" ")
                    .Append(value);
            }

            return sb.Append("}");
        }

        public virtual string parseJson(int indentLevel)
        {
            var indent = "";
            for (var i = 0; i < indentLevel; i++)
                indent += "/t";
            return indent + "{}";
        }

        protected string parseJSONWithValue(string valueString, int indentLevel) {
            var sb = new StringBuilder().Append("{\n");

            indent(sb, indentLevel).Append("\"field\": ").Append("\"").Append(field).Append("\",\n");
            indent(sb, indentLevel).Append("\"type\": ").Append("\"").Append(type.toJSON()).Append("\",\n");

            // write custom value
            indent(sb, indentLevel).Append("\"value\": ")
                .Append(valueString)
                .Append("\n");

            return indent(sb, indentLevel - 1).Append("}\n").ToString();
        }

        private static StringBuilder indent(StringBuilder sb, int level) {
            for(var i=0; i<level; i++) {
                sb.Append("\t");
            }
            return sb;
        }
    }
    
    public class StringSearchLeaf : SearchLeaf<string> {
        public StringSearchLeaf (string field, FilterOperator type, string value)
            : base(field, type, value) {}
        
        public override string parseJson(int indentLevel)
        {
            return value == null ? parseJSONWithValue("null", indentLevel) 
                : parseJSONWithValue("\"" + value + "\"", indentLevel);
        }
    }
    
    public class IntegerSearchLeaf : SearchLeaf<int> {
        public IntegerSearchLeaf(string field, FilterOperator type, int value)
            : base(field, type, value) {}

        public override string parseJson(int indentLevel) {
            return parseJSONWithValue(value + "", indentLevel);
        }
    }
    
    public class FloatSearchLeaf : SearchLeaf<float> {
        public FloatSearchLeaf(string field, FilterOperator type, float value)
            : base(field, type, value) {}

        public override string parseJson(int indentLevel) {
            return parseJSONWithValue(value + "", indentLevel);
        }
    }

    public class BoolSearchLeaf : SearchLeaf<bool> {
        public BoolSearchLeaf(string field, FilterOperator type, bool value) 
            : base(field, type, value) { }

        public override string parseJson(int indentLevel) {
            return parseJSONWithValue(value ? "true" : "false", indentLevel);
        }
    }
    
    public class ArraySearchLeaf<T> : SearchLeaf<List<T>> {
        public ArraySearchLeaf(string field, FilterOperator type, List<T> value) 
            : base(field, type, value) {}
        
//        private static readonly SimpleDateFormat DATE_FORMATTER = new SimpleDateFormat("yyyy-MM-dd");

        public override string parseJson(int indentLevel) 
        {
            var valuesString = new StringBuilder("[");
            
            var it = value.OfType<T>().GetEnumerator();
            var hasNext = it.MoveNext();
            while (hasNext) {
                valuesString.Append(
                    getValueStringEncoding(
                        it.Current
                    )
                );
                hasNext = it.MoveNext();
                if (hasNext) {
                    valuesString.Append(",");
                }
            }
            it.Dispose();

            return parseJSONWithValue(valuesString.Append("]").ToString(), indentLevel);
        }

        public override StringBuilder getString() 
        {
            if (field == null || type == null) 
            {
                return base.getString();
            }

            var sb = new StringBuilder("{");
            sb.Append(field).Append(" ")
                .Append(type).Append(" ")
                .Append("[");

            // parse list elements
            if (value.Count > 0)
                sb.Append(
                    getValueStringEncoding(value[0])
                );
            foreach (var item in value.GetRange(1, value.Count-1)) 
            {
                sb.Append(", ").Append(getValueStringEncoding(item));
            }
            sb.Append("]");

            return sb.Append("}");
        }

        private string getValueStringEncoding(T element) {
            if (element == null)
                return "null";
            
            if (AbstractSearchClient<object>.IsNumber(element))
                return "" + element;
            
            if (element is string)
                return "\"" + element + "\"";

            if (bool.TryParse(element.ToString(), out var result)) {
                Console.WriteLine($"{element} -> {result}");
                return result ? "true" : "false";
            }
            
            return element.ToString();
        }
    }
}