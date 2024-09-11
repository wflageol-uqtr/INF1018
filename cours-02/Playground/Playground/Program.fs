// For more information see https://aka.ms/fsharp-console-apps

let x = 4

let mutable add = fun x y z -> x + y + z

let c = (2, 4)
let a = fst c
let b = snd c

// round 2.6 |> int |> add x 4 |> printfn "%A"

// printfn "%A" (add x 4 (int (round 2.6)))




let list = [2; 3; 4]

let rec addList list =
    match list with
    | [] -> 0
    | head::tail -> head + addList tail

printfn "%A" (addList list)

List.reduce (+) list |>
printfn "%A"

let double x = x + x
List.map double list |> printfn "%A"