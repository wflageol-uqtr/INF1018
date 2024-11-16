using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.AST
{
    public enum Operator
    {
        Plus, Minus, Multiply, Divide
    }

    public interface IPartialExpression : IInterpretable { }

    public class EmptyExpression : IPartialExpression
    {
        public void Interpret(InterpretingEnvironment env) { }
    }

    public record TailExpression(Operator Operator, int Scalar, IPartialExpression PartialExpression) : IPartialExpression
    {
        public void Interpret(InterpretingEnvironment env)
        {
            env.Stack.Push(new IntegerToken(Scalar));
            env.Stack.Push(new OperatorToken(Operator));
            PartialExpression.Interpret(env);
        }
    }
}
