open System

open Streams
open Combinators
open Lexer
open Semantic
open Interpreter

let source = "
: double dup +; 
: quadruple double double; 
: greeting \"Hello World!\"; 

5 3 * double quadruple print greeting print [1 2 [4 5 6] 3] print"
let sourceStream = stream source

let tokens = lex sourceStream

let ast = parse (stream tokens)

let vm = newVirtualMachine ast
interpret vm |> ignore