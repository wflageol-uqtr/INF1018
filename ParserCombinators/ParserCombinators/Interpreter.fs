module Interpreter

open Lexer
open Semantic
open System.Collections.Generic

type InterpretingEnvironment = { Stack: Stack<Token> }

let envPush env lexUnit =
    env.Stack.Push lexUnit

// Interprète l'Expression suivante en la poussant dans le stack de l'environnement.
let rec interpretExpression (env: InterpretingEnvironment) (expression: Expression) =
    let (scalar, pExpr) = expression
    envPush env (Integer scalar)
    interpretPartialExpression env pExpr

// Interprète la PartialExpression suivante en la poussant dans le stack de l'environnement.
and interpretPartialExpression (env: InterpretingEnvironment) (pExpr: PartialExpression) =
    match pExpr with
    | EmptyExpression -> ()
    | TailExpression (op, scalar, pExpr) ->
        envPush env (Integer scalar)
        envPush env (Op op)
        interpretPartialExpression env pExpr

// Évalue le résultat des opérations dans le stack de l'environnement.
let execEnv (env: InterpretingEnvironment) =
    let unwrap token =
        match token with
        | Integer i -> i
        | _ -> failwith "Not an integer."

    // Inverser le stack pour mettre la priorité à gauche.
    let stack = env.Stack.ToArray () |> Stack<_>

    while stack.Count > 1 do
        let x = stack.Pop ()
        let y = stack.Pop ()
        let op = stack.Pop ()

        let x = unwrap x
        let y = unwrap y
            
        let result = 
            match op with
            | Op Plus -> x + y
            | Op Minus -> x - y
            | Op Multiply -> x * y
            | Op Divide -> x / y
            | _ -> failwith "Should not happen."

        stack.Push (Integer result)
    stack.Pop () |> unwrap

// Interprète le résultat de l'Expression spécifiée.
let interpret (expression: Expression) =
    let env = { Stack = Stack<_>() }
    interpretExpression env expression
    execEnv env