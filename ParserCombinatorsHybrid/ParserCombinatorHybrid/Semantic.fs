﻿module Semantic

open Streams
open Combinators
open Lexer
open Interpreter.AST

// Convertis un token en entier.
let convertInteger (token: Token option) : int =
    match token with
    | Some (Integer i) -> i
    | _ -> failwith "Invalid conversion."

// Convertis un token en Operator.
let convertOp (token: Token option) : Operator =
    match token with
    | Some (Op Plus) -> Operator.Plus
    | Some (Op Minus) -> Operator.Minus
    | Some (Op Multiply) -> Operator.Multiply
    | Some (Op Divide) -> Operator.Divide
    | _ -> failwith "Invalid conversion."

let rec parseExpression : Parser<Token, Expression> =
    fun stream ->
        let scalarToken, stream = next stream
        let scalar = convertInteger scalarToken

        match parsePartialExpression stream with
        | Ok (pExpr, stream) ->
            Ok([Expression(scalar, Seq.head pExpr)], stream)
        | Error -> Error

and parsePartialExpression : Parser<Token, IPartialExpression> =
    fun stream ->
        match peek stream with
        | Some _ ->
            let opToken, stream = next stream
            let op = convertOp opToken

            let scalarToken, stream = next stream
            let scalar = convertInteger scalarToken

            match parsePartialExpression stream with
            | Ok (pExpr, stream) ->
                Ok([TailExpression (op, scalar, Seq.head pExpr)], stream)
            | Error -> Error
        | None -> Ok ([EmptyExpression()], stream)