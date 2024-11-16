using Interpreter.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public interface IEnvironmentToken { }
    public record OperatorToken(Operator Value) : IEnvironmentToken;
    public record IntegerToken(int Value) : IEnvironmentToken;

    public class InterpretingEnvironment
    {
        public Stack<IEnvironmentToken> Stack { get; } = new Stack<IEnvironmentToken>();

        public int Execute()
        {
            Stack<IEnvironmentToken> stack = new(Stack);
            int popInt() => (stack.Pop() as IntegerToken)?.Value ?? 0;

            while (stack.Count > 1)
            {
                var x = popInt();
                var y = popInt();
                var op = stack.Pop() as OperatorToken;

                if (op == null)
                    throw new InvalidOperationException("Should not happen.");

                var result = op.Value switch
                {
                    Operator.Plus => x + y,
                    Operator.Minus => x - y,
                    Operator.Multiply => x * y,
                    Operator.Divide => x / y,
                    _ => throw new InvalidOperationException("Should not happen.")
                };

                stack.Push(new IntegerToken(result));
            }

            return popInt();
        }
    }
}
