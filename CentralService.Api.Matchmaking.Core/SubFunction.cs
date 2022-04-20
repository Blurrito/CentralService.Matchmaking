using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core
{
    public class SubFunction
    {
        public string PropertyName { get; set; }
        public string Operator { get; set; }
        public int Value { get; set; }

        public SubFunction(string FunctionString)
        {
            if (FunctionString.Contains('('))
                throw new NotImplementedException("Complex subfunctions are not supported.");
            SetOperator(FunctionString);
            SetPropertyNameValue(FunctionString);
        }

        public string Run(Server Server)
        {
            KeyValuePair<string, string> Parameter = Server.Properties.FirstOrDefault(x => x.Key == PropertyName);
            if (Parameter.Value != null)
            {
                int PropertyValue = int.Parse(Parameter.Value);
                return Calculate(PropertyValue).ToString();
            }
            return null;
        }

        private int Calculate(int PropertyValue)
        {
            switch (Operator)
            {
                case "&":
                    return PropertyValue & Value;
                case "+":
                    return PropertyValue + Value;
                case "-":
                    return PropertyValue - Value;
                case "*":
                    return PropertyValue * Value;
                case "/":
                    return PropertyValue / Value;
                case "%":
                    return PropertyValue % Value;
                case "|":
                    return PropertyValue | Value;
                case "^":
                    return PropertyValue ^ Value;
                case ">>":
                    return PropertyValue >> Value;
                case "<<":
                    return PropertyValue << Value;
                default:
                    throw new NotImplementedException("Unrecognized operator.");
            }
        }

        private void SetPropertyNameValue(string FunctionString)
        {
            string[] SplitFunction = FunctionString.Split(Operator);
            int NumericValue = 0;
            if (int.TryParse(SplitFunction[0], out NumericValue))
                PropertyName = SplitFunction[1];
            else
            {
                PropertyName = SplitFunction[0];
                NumericValue = int.Parse(SplitFunction[1]);
            }
            Value = NumericValue;
        }

        private void SetOperator(string FunctionString)
        {
            if (FunctionString.Contains("&"))
                Operator = "&";
            else if (FunctionString.Contains("+"))
                Operator = "+";
            else if (FunctionString.Contains("-"))
                Operator = "-";
            else if (FunctionString.Contains("*"))
                Operator = "*";
            else if (FunctionString.Contains("/"))
                Operator = "/";
            else if (FunctionString.Contains("%"))
                Operator = "%";
            else if (FunctionString.Contains("|"))
                Operator = "|";
            else if (FunctionString.Contains("^"))
                Operator = "^";
            else if (FunctionString.Contains(">>"))
                Operator = ">>";
            else if (FunctionString.Contains("<<"))
                Operator = "<<";
            throw new ArgumentException("Could not find a valid operator.", nameof(FunctionString));
        }
    }
}
