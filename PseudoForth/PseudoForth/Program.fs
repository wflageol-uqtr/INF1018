open System

open Streams
open Lexer
open Semantic
open Interpreter

let source = "
: double dup +;
: quadruple double double;

5 3 * quadruple to x
x x + print"
let sourceStream = stream source

let tokens = lex sourceStream

let ast = parse (stream tokens)

let vm = newVirtualMachine ast
interpret vm |> ignore