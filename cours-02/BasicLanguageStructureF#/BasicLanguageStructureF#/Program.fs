type Value = int
and Addition = Instruction * Instruction
and Subtraction = Instruction * Instruction
and Equality = Instruction * Instruction
and If = { Condition: Instruction
           Then: Instruction
           Else: Instruction option }
and Instruction = Value of Value
                  | Addition of Addition
                  | Subtraction of Subtraction
                  | Equality of Equality
                  | If of If

let rec execute instruction =
    match instruction with
    | Value v -> v
    | Addition (x, y) -> (execute x) + (execute y)
    | Subtraction (x, y) -> (execute x) - (execute y)
    | Equality (x, y) -> if (execute x) = (execute y) 
                         then 1
                         else 0
    | If i -> if (execute i.Condition) = 1
              then (execute i.Then)
              else match i.Else with
                   | Some e -> execute e
                   | None -> 0

let program = If { Condition = Equality (Value 0, Value 1)
                   Then = Addition (Value 2, Value 3)
                   Else = Subtraction (Value 5, Value 4) |> Some}

printfn "%A" (execute program)
