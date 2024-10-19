open System

open Streams
open Combinators
open Lexer
open Semantic
open Interpreter

// On crée un stream de caractères à passer au lexer.
let charStream = stream "1234 + 123 - 56 * 21"
let tokens = lex charStream

// Affichage de tous les tokens retournés par le lexer.
for token in tokens do
    Console.WriteLine token

// On crée un stream de token à passer au parser sémantique.
let tokenStream = stream tokens
let ast = parseOperation tokenStream |> unwrap |> Seq.head

// On passe l'arbre de syntaxe abstrait obtenu à l'interpréteur.
interpretOperation ast
|> Console.WriteLine