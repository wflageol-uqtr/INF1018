// For more information see https://aka.ms/fsharp-console-apps
open System

let greet (i: int) = $"Hello - {i}"

let integers = [1; 2; 3; 4; 5]

List.reduce (*) integers |> Console.WriteLine

type Student = { FirstName: string
                 LastName: string
                 Age: int }

let student firstName lastName age = { Student.LastName = lastName
                                       FirstName = firstName
                                       Age = age }

type Teacher = { FirstName: string
                 LastName: string
                 Code: string }

let teacher firstName lastName code = { Teacher.FirstName = firstName
                                        LastName = lastName
                                        Code = code }

type Person = StudentConstructor of Student
            | TeacherConstructor of Teacher

let john = student "John" "Smith" 45
let jane = teacher "Jane" "Smith" "AAAA0000000"
let persons = [| StudentConstructor john
                 TeacherConstructor jane |]

printf "%A" persons

match persons[0] with
| StudentConstructor student -> sprintf "%i" student.Age
| TeacherConstructor teacher -> teacher.Code
|> Console.WriteLine
