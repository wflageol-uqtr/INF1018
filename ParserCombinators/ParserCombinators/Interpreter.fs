module Interpreter

open Lexer
open Semantic

// Interprète l'expression spécifiée.
let rec interpretExpression (expression: Expression) : int =
    match expression.Op with
    | Plus -> expression.Left + interpretOperation expression.Operation
    | Minus -> expression.Left - interpretOperation expression.Operation
    | Multiply -> expression.Left * interpretOperation expression.Operation
    | Divide -> expression.Left / interpretOperation expression.Operation

// Interprète l'opération spécifiée.
and interpretOperation (operation: Operation) : int =
    match operation with
    | Expression ex -> interpretExpression ex
    | Scalar i -> i