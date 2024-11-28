using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace DIALOGUE.LogicalLines
{
    public static class LogicalLineUtils
    {
        
        public static class Encapsulation
        {
            public struct EncapsulatedData
            {
                public bool isNull => lines == null;
                public List<string> lines;
                public int endingIndex;
                public int startingIndex;
            }

            private const char ENCAPSULATION_START = '{';
            private const char ENCAPSULATION_END = '}';

            public static EncapsulatedData RipEncapsulatedData(Conversation conversation, int startingIndex, bool ripHeaderAndEncapsulators = false)
            {
                int encapsulationDepth = 0;
                EncapsulatedData data = new EncapsulatedData { lines = new List<string>(), startingIndex = startingIndex, endingIndex = 0 };

                for (int i = startingIndex; i < conversation.Count; i++)
                {
                    string line = conversation.GetLines()[i];

                    if (ripHeaderAndEncapsulators || encapsulationDepth > 0 && !isEncapsulationEnd(line))
                        data.lines.Add(line);

                    if (isEncapsulationStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if (isEncapsulationEnd(line))
                    {
                        encapsulationDepth--;
                        if (encapsulationDepth == 0)
                        {
                            data.endingIndex = i;
                            break;
                        }
                    }
                }

                return data;
            }

            public static bool isEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
            public static bool isEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
        }

        public static class Expressions
        {
            public static HashSet<string> OPERATORS = new HashSet<string>() { "=", "-", "+", "*", "/", "%", "-=", "+=", "*=", "/=" };
            public static readonly string REGEX_ARITHMETIC = @"([-+*/=]=?)";
            public static readonly string REGEX_OPERATOR_LINE = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";

            public static object CalculateValue(string[] expressionParts)
            {
                List<string> operandStrings = new List<string>();
                List<string> operatorStrings = new List<string>();
                List<object> operands = new List<object>();

                for (int i = 0; i < expressionParts.Length; i++)
                {
                    string part = expressionParts[i].Trim();

                    if (part == string.Empty)
                        continue;

                    if (OPERATORS.Contains(part))
                        operatorStrings.Add(part);
                    else
                        operandStrings.Add(part);
                }

                foreach (string operandString in operandStrings)
                {
                    operands.Add(ExtractValue(operandString));
                }

                CalculateValue_MultiplicationAndDivision(operatorStrings, operands);
                
                CalculateValue_AdditionAndSubtraction(operatorStrings, operands);

                return operands[0];
            }

            private static void CalculateValue_MultiplicationAndDivision(List<string> operatorStrings, List<object> operands)
            {
                for (int i = 0; i < operatorStrings.Count; i++)
                {
                    string operatorString = operatorStrings[i];

                    if (operatorString == "*" || operatorString == "/")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if (operatorString == "*")
                            operands[i] = leftOperand * rightOperand;
                        else
                        {
                            if (rightOperand == 0)
                            {
                                Debug.LogError("Can't divide by zero!");
                                return;
                            }

                            operands[i] = leftOperand / rightOperand;
                        }

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);
                        i--;
                    }
                }
            }
            
            private static void CalculateValue_AdditionAndSubtraction(List<string> operatorStrings, List<object> operands)
            {
                for (int i = 0; i < operatorStrings.Count;i++)
                {
                    string operatorString = operatorStrings[i];

                    if (operatorString == "+" || operatorString == "-")
                    {
                        double leftOperand = Convert.ToDouble(operands[i]);
                        double rightOperand = Convert.ToDouble(operands[i + 1]);

                        if (operatorString == "+")
                            operands[i] = leftOperand + rightOperand;
                        else
                            operands[i] = leftOperand - rightOperand;

                        operands.RemoveAt(i + 1);
                        operatorStrings.RemoveAt(i);
                        i--;
                    }
                }
            }

            private static object ExtractValue(string value)
            {
                bool negate = false;

                if (value.StartsWith('!'))
                {
                    negate = true;
                    value = value.Substring(1);
                }

                if (value.StartsWith(VariableStore.VARIABLE_ID))
                {
                    string variableName = value.TrimStart(VariableStore.VARIABLE_ID);
                    if (!VariableStore.HasVariable(variableName))
                    {
                        Debug.LogError($"Variable '{variableName}' does not exist!");
                        return null;
                    }

                    VariableStore.TryGetValue(variableName, out object val);

                    if (val is bool boolVal && negate)
                        return !boolVal;

                    return val;
                }

                else if (value.StartsWith('\"') && value.EndsWith('\"'))
                {
                    value = TagManager.Inject(value, injectTags: true, injectVariables: true);
                    return value.Trim('"');
                }
                else
                {
                    if (int.TryParse(value, out int intValue))
                        return intValue;
                    else if (float.TryParse(value, out float floatValue))
                        return floatValue;
                    else if (bool.TryParse(value, out bool boolValue))
                        return negate ? !boolValue : boolValue;
                    else 
                    {
                        value = TagManager.Inject(value, injectTags: true, injectVariables: true);
                        return value; 
                    }
                }
            }
        }

        public static class Conditions
        {
            public static readonly string REGEX_CONDITIONAL_OPERATORS = @"(==|!=|<=|>=|<|>|&&|\|\|)";

            public static bool EvaluateCondition(string condition)
            {
                condition = TagManager.Inject(condition, injectTags: true, injectVariables: true);

                string[] parts = Regex.Split(condition, REGEX_CONDITIONAL_OPERATORS)
                    .Select(p => p.Trim()).ToArray();

                for (int i = 0 ; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\"") && parts[i].EndsWith("\""))
                        parts[i] = parts[i].Substring(1, parts[i].Length - 2);
                }

                // if there is only one part within the condition try to parse as a boolean
                if (parts.Length == 1)
                {
                    if (bool.TryParse(parts[0], out bool result))
                        return result;
                    else
                    {
                        Debug.LogError($"Could not parse condition: {condition}");
                        return false;
                    }
                }
                // if there are 3 parts, then it is an expression
                else if (parts.Length == 3)
                {
                    return EvaluateExpression(parts[0], parts[1], parts[2]);
                }
                // otherwise, it is an unsupported format and return an error
                else
                {
                    Debug.LogError($"Unsupported condition format: {condition}");
                    return false;
                }
            }

            private delegate bool OperatorFunc<T>(T lhs, T rhs);

            private static Dictionary<string, OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>()
            {
                { "&&", (lhs, rhs) => lhs && rhs },
                { "||", (lhs, rhs) => lhs || rhs },
                { "==", (lhs, rhs) => lhs == rhs },
                { "!=", (lhs, rhs) => lhs != rhs }
            };

            private static Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
            {
                { "==", (lhs, rhs) => lhs == rhs },
                { "!=", (lhs, rhs) => lhs != rhs },
                { ">", (lhs, rhs) => lhs > rhs },
                { ">=", (lhs, rhs) => lhs >= rhs },
                { "<", (lhs, rhs) => lhs < rhs },
                { "<=", (lhs, rhs) => lhs <= rhs },
            };

            private static Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>()
            {
                { "==", (lhs, rhs) => lhs == rhs },
                { "!=", (lhs, rhs) => lhs != rhs },
                { ">", (lhs, rhs) => lhs > rhs },
                { ">=", (lhs, rhs) => lhs >= rhs },
                { "<", (lhs, rhs) => lhs < rhs },
                { "<=", (lhs, rhs) => lhs <= rhs },
            };

            private static bool EvaluateExpression(string lhs, string op, string rhs)
            {
                // try to evaluate it as a boolean expression
                if (bool.TryParse(lhs, out bool leftBool) && bool.TryParse(rhs, out bool rightBool))
                {
                    if (boolOperators.ContainsKey(op))
                        return boolOperators[op](leftBool, rightBool);
                    else
                        throw new InvalidOperationException($"Unsupported operation for boolean expression: {op}");
                }

                // or try to evaluate it as a float expression
                if (float.TryParse(lhs, out float leftFloat) &&  float.TryParse(rhs, out float rightFloat))
                    return floatOperators[op](leftFloat, rightFloat);
                
                // or try to evaluate it as an integer expressions
                if (int.TryParse(lhs, out int leftInt) &&  int.TryParse(rhs, out int rightInt))
                    return intOperators[op](leftInt, rightInt);

                // otherwise, assume it's a string and switch the operator between == or !=
                switch (op)
                {
                    case "==": return lhs == rhs;
                    case "!=": return lhs != rhs;
                    default: throw new InvalidOperationException($"Unsupported operation: {op}");
                }
            }
        }
    }
}