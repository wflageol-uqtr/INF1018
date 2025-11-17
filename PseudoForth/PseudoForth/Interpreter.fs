module Interpreter

open Lexer
open Semantic
open System
open System.Collections.Generic

type VirtualMachine = { Words: Dictionary<string, Instruction list>
                        Variables: Dictionary<string, ScalarType>
                        OperationStack: Stack<ScalarType> }

let newVirtualMachine () = { Words = Dictionary<string, Instruction list>()
                             Variables = Dictionary<string, ScalarType>()
                             OperationStack = Stack<ScalarType>() }
                             
let scalarToInt scalar =
    match scalar with
    | ScalarInt i -> i
    | _ -> failwith "Impossible de convertir en int."

let scalarToText scalar =
    match scalar with
    | ScalarText t -> t
    | ScalarInt i -> sprintf "%i" i

let rec interpret ast vm =
    for instruction in ast do
        match instruction with
        | Scalar s -> interpretScalar s vm 
        | Operation o -> interpretOperation o vm
        | Assignation name -> interpretAssign name vm
        | Declaration decl -> interpretDeclaration decl vm
        | Call name -> interpretCall name vm

and interpretCall name vm =
    if vm.Variables.ContainsKey name
    then interpretScalar vm.Variables.[name] vm
    else let instructions = vm.Words.[name]
         interpret instructions vm

and interpretScalar scalar vm =
    vm.OperationStack.Push scalar

and interpretAssign name vm =
    vm.Variables.[name] <- vm.OperationStack.Pop ()

and interpretDeclaration decl vm =
    vm.Words.[decl.WordName] <- decl.Definition

and interpretOperation op vm =
    match op with
    | "+" -> interpretBinaryOperation (+) vm
    | "-" -> interpretBinaryOperation (-) vm
    | "*" -> interpretBinaryOperation (*) vm
    | "/" -> interpretBinaryOperation (/) vm
    | "dup" -> interpretDup vm
    | "swap" -> interpretSwap vm
    | "print" -> interpretPrint vm
    | _ -> failwith "Opérateur invalide."

and interpretBinaryOperation fn vm =
    let left = vm.OperationStack.Pop () |> scalarToInt
    let right = vm.OperationStack.Pop () |> scalarToInt

    let result = fn left right

    vm.OperationStack.Push (ScalarInt result)

and interpretDup vm =
    let top = vm.OperationStack.Peek ()
    vm.OperationStack.Push top

and interpretSwap vm =
    let left = vm.OperationStack.Pop ()
    let right = vm.OperationStack.Pop ()
    vm.OperationStack.Push left
    vm.OperationStack.Push right

and interpretPrint vm =
    let text = vm.OperationStack.Pop () |> scalarToText
    Console.WriteLine text