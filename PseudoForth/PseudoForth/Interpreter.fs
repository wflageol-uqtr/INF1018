module Interpreter

open Lexer
open Semantic
open System
open System.Collections.Generic

type VirtualMachine = { Instructions: Stack<AstToken>
                        Stack: Stack<AstToken>  
                        Dictionary: Dictionary<string, AstToken list> }

let newVirtualMachine ast = { Instructions = Seq.toArray ast |> Seq.rev |> Stack<AstToken>
                              Stack = Stack<AstToken>()
                              Dictionary = Dictionary<string, AstToken list>() }

let defineWord vm =
    match vm.Instructions.Pop() with
    | Scalar (Ident name) -> let rec accBody acc =
                                match vm.Instructions.Pop () with
                                | Scalar (Sep ';') -> List.rev acc
                                | token -> accBody (token :: acc)
                             let body = accBody []
                             vm.Dictionary.Add (name, body)
    | _ -> failwith "Not a valid word name."

let applyIntegerFunction2 vm fn =
    let convertToInt token =
        match token with
        | Scalar (Integer i) -> i
        | _ -> failwith "Not an integer."
    let p2 = vm.Stack.Pop () |> convertToInt
    let p1 = vm.Stack.Pop () |> convertToInt
    fn p1 p2 |> Integer |> Scalar |> vm.Stack.Push

let applyDup vm = vm.Stack.Peek() |> vm.Stack.Push

let applyPrint vm =
    let rec printToken token =
        match token with
        | Scalar (Text text) -> Console.Write text
        | Scalar (Ident ident) -> Console.Write text
        | Scalar (Integer i) -> Console.Write i
        | List list -> Console.Write "[ "
                       for token in list do
                          printToken token
                          Console.Write " "
                       Console.Write "]"
        | _ -> failwith "Should not happen."
    vm.Stack.Pop() |> printToken
    Console.WriteLine()
                        

let interpretFunctionCall vm name = 
    match name with
    | "+" -> applyIntegerFunction2 vm (+)
    | "-" -> applyIntegerFunction2 vm (-)
    | "*" -> applyIntegerFunction2 vm (*)
    | "/" -> applyIntegerFunction2 vm (/)
    | ":" -> defineWord vm
    | "dup" -> applyDup vm
    | "print" -> applyPrint vm
    | ident -> let body = vm.Dictionary.GetValueOrDefault (ident, [])
               if List.isEmpty body then failwith "Undefined word."
               for inst in List.rev body do
                   vm.Instructions.Push inst


let interpret vm =
    while not (Seq.isEmpty vm.Instructions) do
        let inst = vm.Instructions.Pop ()

        match inst with
        | Scalar (Ident name) -> interpretFunctionCall vm name
        | token -> vm.Stack.Push token

    vm.Stack