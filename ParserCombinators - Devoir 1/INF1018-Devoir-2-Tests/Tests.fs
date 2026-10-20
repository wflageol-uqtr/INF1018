namespace INF1018_Devoir_2_Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open INF1018_Devoir_2.Lex

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestVar () =
        let result = parse "B"
        Assert.AreEqual(result, [Var B])
        
    [<TestMethod>]
    member this.TestOp () =
        let result = parse "/"
        Assert.AreEqual(result, [Op Divide])

    [<TestMethod>]
    member this.TestInt () =
        let result = parse "42"
        Assert.AreEqual(result, [Int 42])

    [<TestMethod>]
    member this.TestComplex1 () =
        let result = parse "A + 18"
        Assert.AreEqual(result, [Var A; Op Plus; Int 18])

    [<TestMethod>]
    member this.TestComplex2 () =
        let result = parse "B - 2 / C + 67"
        Assert.AreEqual(result, [Var B; Op Minus; Int 2; Op Divide; Var C; Op Plus; Int 67])