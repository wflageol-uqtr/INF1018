// For more inf ormation see https://aka.ms/fsharp-console-apps
open System

type Return = { ReturnValue: int }
type ComparativeOperator = GreaterThan
                         | LesserThan
                         | Equal
                         | GreaterEq
                         | LesserEq
type Condition = { LeftValue: int
                   RightValue: int
                   Operator: ComparativeOperator }
type Instruction = Return of Return
                 | If of If
and  If = { Condition: Condition
            IfTrue: Instruction
            IfFalse: Instruction }

let return5 = Return { ReturnValue = 5 }
let return6 = Return { ReturnValue = 6 }
let cond = { LeftValue = 4
             RightValue = 4
             Operator = Equal }
let instruction = If { Condition = cond
                       IfTrue = return5
                       IfFalse = return6 }

let executeCondition condition =
    let left = condition.LeftValue
    let right = condition.RightValue
    match condition.Operator with
    | GreaterThan -> left > right
    | GreaterEq -> left >= right
    | LesserThan -> left < right
    | LesserEq -> left <= right
    | Equal -> left = right

let rec interpret instruction =
    match instruction with
    | Return r -> Console.WriteLine r.ReturnValue
    | If i -> if executeCondition i.Condition
              then interpret i.IfTrue
              else interpret i.IfFalse

Console.WriteLine (interpret instruction)