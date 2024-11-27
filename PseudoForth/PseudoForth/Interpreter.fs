module Interpreter

open Lexer
open Semantic
open System
open System.Collections.Generic

type DataType = DataInt of int
              | DataText of string
              | DataList of DataType list

type VirtualMachine = { Instructions: Stack<AstToken>
                        Stack: Stack<DataType>  
                        Dictionary: Dictionary<string, AstToken list>
                        Variables: Dictionary<string, DataType>}

let newVirtualMachine ast = { Instructions = Seq.toArray ast |> Seq.rev |> Stack<AstToken>
                              Stack = Stack<DataType>()
                              Dictionary = Dictionary<string, AstToken list>()
                              Variables = Dictionary<string, DataType>() }

let defineWord vm =
    match vm.Instructions.Pop() with
    | Scalar (Ident name) -> let rec accBody acc =
                                match vm.Instructions.Pop () with
                                | Scalar (Sep ';') -> List.rev acc
                                | token -> accBody (token :: acc)
                             let body = accBody []
                             vm.Dictionary.Add (name, body)
    | _ -> failwith "Not a valid word name."

let defineVariable vm =
    match vm.Instructions.Pop() with
    | Scalar (Ident name) -> let value = vm.Stack.Pop()
                             vm.Variables.Add (name, value)
    | _ -> failwith "Not a valid variable name."

let applyIntegerFunction2 vm fn =
    let convertToInt token =
        match token with
        | DataInt i -> i
        | _ -> failwith "Not an integer."
    let p2 = vm.Stack.Pop () |> convertToInt
    let p1 = vm.Stack.Pop () |> convertToInt
    fn p1 p2 |> DataInt |> vm.Stack.Push

let applyDup vm = vm.Stack.Peek() |> vm.Stack.Push

let applyPrint vm =
    let rec printToken token =
        match token with
        | DataText text -> Console.Write text
        | DataInt i -> Console.Write i
        | DataList list -> Console.Write "[ "
                           for token in list do
                             printToken token
                             Console.Write " "
                           Console.Write "]"
    vm.Stack.Pop() |> printToken
    Console.WriteLine()
                        

let interpretFunctionCall vm name = 
    match name with
    | "+" -> applyIntegerFunction2 vm (+)
    | "-" -> applyIntegerFunction2 vm (-)
    | "*" -> applyIntegerFunction2 vm (*)
    | "/" -> applyIntegerFunction2 vm (/)
    | ":" -> defineWord vm
    | "to" -> defineVariable vm
    | "dup" -> applyDup vm
    | "print" -> applyPrint vm
    | ident -> let body = vm.Dictionary.GetValueOrDefault (ident, [])
               if List.isEmpty body 
               then vm.Variables.[ident] |> vm.Stack.Push
               else for inst in List.rev body do
                        vm.Instructions.Push inst


let interpret vm =
    while not (Seq.isEmpty vm.Instructions) do
        let inst = vm.Instructions.Pop ()

        match inst with
        | Scalar (Ident name) -> interpretFunctionCall vm name
        | Scalar (Integer i) -> vm.Stack.Push (DataInt i)
        | Scalar (Text text) -> vm.Stack.Push (DataText text)
        | _ -> failwith "Unsupported."

    vm.Stack