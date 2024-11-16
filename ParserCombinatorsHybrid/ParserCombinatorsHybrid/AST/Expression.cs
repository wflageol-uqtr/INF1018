using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.AST
{
    public record Expression(int Scalar, IPartialExpression PartialExpression) : IInterpretable
    {
        public void Interpret(InterpretingEnvironment env)
        {
            env.Stack.Push(new IntegerToken(Scalar));
            PartialExpression.Interpret(env);
        }
    }
}
