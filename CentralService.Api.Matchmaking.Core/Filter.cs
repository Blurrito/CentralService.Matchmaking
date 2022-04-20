using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core
{
    public class Filter
    {
        public string PropertyName { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        public Filter(string FilterString)
        {
            if (FilterString.Contains('('))
                Value = GetSubFunction(ref FilterString);
            SetOperator(FilterString);
            SetPropertyNameValue(FilterString);
        }

        public bool Run(Server Server)
        {
            string StringValue = GetStringValue(Server);
            if (StringValue == null)
                return false;

            KeyValuePair<string, string> ServerValue = Server.Properties.FirstOrDefault(x => x.Key == PropertyName);
            if (ServerValue.Value == null)
                return false;

            if (ServerValue.Value != StringValue)
                return false;
            return true;
        }

        private string GetStringValue(Server Server)
        {
            if (Value.GetType() == typeof(SubFunction))
            {
                SubFunction Function = (SubFunction)Value;
                return Function.Run(Server);
            }
            else
                return (string)Value;
        }

        private SubFunction GetSubFunction(ref string FilterString)
        {
            int StartIndex = FilterString.IndexOf('(');
            int EndIndex = FilterString.LastIndexOf(')');
            string Substring = FilterString.Substring(StartIndex + 1, EndIndex - StartIndex + 1);
            FilterString.Remove(StartIndex, EndIndex - StartIndex + 1);
            return new SubFunction(Substring);
        }

        private void SetPropertyNameValue(string FilterString)
        {
            string[] SplitFilter = FilterString.Split(Operator);
            if (Value != null)
            {
                if (SplitFilter[0] != string.Empty)
                    PropertyName = SplitFilter[0];
                else
                    PropertyName = SplitFilter[1];
            }
            else
            {
                if (SplitFilter[0].Contains('\'') || int.TryParse(SplitFilter[0], out _))
                {
                    Value = SplitFilter[0].Replace("\'", "");
                    PropertyName = SplitFilter[1];
                }
                else
                {
                    Value = SplitFilter[1].Replace("\'", "");
                    PropertyName = SplitFilter[0];
                }
            }
        }

        private void SetOperator(string Input)
        {
            if (Input.Contains(">="))
                Operator = ">=";
            else if (Input.Contains("<="))
                Operator = "<=";
            else if (Input.Contains("!="))
                Operator = "!=";
            else if (Input.Contains("="))
                Operator = "=";
            else if (Input.Contains("LIKE"))
                Operator = "LIKE";
            else if (Input.Contains(">"))
                Operator = ">";
            else if (Input.Contains("<"))
                Operator = "<";
            else
                throw new ArgumentException("Could not find a known operator in the provided string.", nameof(Input));
        }
    }
}
