module Semantic

open Streams
open Combinators
open Lexer

type Expression = { Left: int
                    Op: Operator
                    Operation: Operation }
and Operation = Expression of Expression | Scalar of int 

/// Convertis un token en entier.
let convertInteger (token: Token option) : int =
    match token with
    | Some (Integer i) -> i
    | _ -> failwith "Invalid conversion."

// Convertis un token en Operator.
let convertOp (token: Token option) : Operator =
    match token with
    | Some (Op o) -> o
    | _ -> failwith "Invalid conversion."

// Parser qui valide que les prochains token correspondent à une expression.
let rec parseExpression : Parser<Token, Expression> =
    fun stream -> 
        let leftToken, stream = next stream
        let left = convertInteger leftToken

        let opToken, stream = next stream
        let op = convertOp opToken

        match parseOperation stream with
        | Ok (operation, stream) ->
            Ok ([{ Left = left; Op = op; Operation = Seq.head operation }], stream)
        | Error -> Error

// Parser qui valide que les prochains token correspondent à une opération.
and parseOperation : Parser<Token, Operation> =
    fun stream -> 
        let intToken, newStream = next stream
        let int = convertInteger intToken

        match peek newStream with
        | Some _ -> match parseExpression stream with
                    | Ok (r, s) -> Ok(Seq.map Expression r, s)
                    | Error -> Error 
        | None -> Ok([Scalar int], newStream)
