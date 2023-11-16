using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;

namespace qubees_utility.Runtime.QubeesUtility.Helpers
{
    public static class StringToExpression
    {
        private static readonly Stack<float> OperandStack = new();
        
        private static readonly List<Operator> Operators = new()
        {
            new("+", 1, (f1, f2) => f1 + f2),
            new("-", 1, (f1, f2) => f2 - f1),
            new("*", 2, (f1, f2) => f1 * f2),
            new("/", 2, (f1, f2) => f2 / f1),
            new("^", 3, (f1, f2) => MathF.Pow(f2,f1)),
        };
        
        public static float CalculateValueFromString(string expression, Dictionary<string,int> replaceDictionary)
        {
            foreach (var replacement in replaceDictionary)
            {
                expression = expression.Replace(replacement.Key, replacement.Value.ToString());
            }
            var postfixString = StringExpressionToPostfix(expression);
            char[] separator = { ' ' };
            var postfixArray = postfixString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return PostfixEvaluate(postfixArray);
        }
        
        private static string StringExpressionToPostfix(string infix)
        {
            string postfix = "";
            Stack<char> stack = new Stack<char>();

            foreach (char c in infix)
            {
                // If the character is a digit or a letter, add it to the output string.
                if (char.IsDigit(c) || char.IsLetter(c))
                {
                    postfix += c;
                }
                // If the character is an operator, pop operators from the stack
                // and add them to the output string until the top of the stack
                // has an operator with lower precedence. Then push the current
                // operator onto the stack.
                else if (IsOperator(c.ToString()))
                {
                    while (stack.Count > 0 && IsOperator(stack.Peek().ToString()) 
                                           && GetOperatorPriority(stack.Peek().ToString()) >= GetOperatorPriority(c.ToString()))
                    {
                        postfix += " ";
                        postfix += stack.Pop();
                    }
                    postfix += " ";

                    stack.Push(c);
                }
                // If the character is a left parenthesis, push it onto the stack.
                else if (c == '(')
                {
                    stack.Push(c);
                }
                // If the character is a right parenthesis, pop operators from the stack
                // and add them to the output string until the left parenthesis is found.
                // Then discard the pair of parentheses.
                else if (c == ')')
                {
                    postfix += " ";
                    while (stack.Peek() != '(')
                    {
                        postfix += stack.Pop();
                    }
                    stack.Pop();
                }
            }
            // Pop any remaining operators from the stack and add them to the output string.
            while (stack.Count > 0)
            {
                postfix += " ";
                postfix += stack.Pop();
            }

            return postfix;
        }
        
        private static float PostfixEvaluate(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0) return 0;
                foreach (var token in tokens)
                { 
                    if(float.TryParse(token,out var value))
                        OperandStack.Push(value);
                    else if (IsOperator(token))
                    {
                        var pop1 = OperandStack.Pop();
                        var pop2 = OperandStack.Pop();
                        OperandStack.Push(GetPerformer(token)(pop1, pop2));
                    }
                }
                if(OperandStack.Count == 1)
                {
                    return OperandStack.Pop();
                }
                throw new Exception("The user has provided too many values.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool IsOperator(string token)
        {
            return Operators.FirstOrDefault(x => x.OperatorExpression == token) != null;
        }

        private static int GetOperatorPriority(string token)
        {
            return Operators.First(x => x.OperatorExpression == token).Priority;
        }

        private static Func<float, float, float> GetPerformer(string token)
        {
            return Operators.First(x => x.OperatorExpression == token).Action;
        }
    }
}    

public class Operator
{
    public string OperatorExpression { get; }
    public int Priority { get; }
    public Func<float,float,float> Action { get; }
    
    public Operator(string operatorExpression, int priority, Func<float,float,float> action)
    {
        OperatorExpression = operatorExpression;
        Priority = priority;
        Action = action;
    }

    public float Perform(float a, float b)
    {
        return Action(a, b);
    }
}