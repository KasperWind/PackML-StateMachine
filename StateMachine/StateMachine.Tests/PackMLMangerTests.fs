module PackMLMangerTests

open System
open Xunit
open FsUnit.Xunit
open StateMachine.FSM
open StateMachine.PackML
open StateMachine.PackMLManager

let ``Expected context data after abort`` = "Context has been altered"

let aborting (context: PackMLContext<obj>) =
    {context with ContextData = ``Expected context data after abort`` :> obj}

let node = "Context" :> obj |> PackMLModel.defaultModel "Single Parent" |> registerCommand Aborting aborting |> NoChilds
let nodes = (
    ("Context" :> obj |> PackMLModel.defaultModel "1" |> registerCommand Aborting aborting),
    [(("Context" :> obj |> PackMLModel.defaultModel "1.1" |> registerCommand Aborting aborting), ( 
        [("Context" :> obj |> PackMLModel.defaultModel "1.1.1" |> registerCommand Aborting aborting) |> NoChilds;
        ("Context" :> obj |> PackMLModel.defaultModel "1.1.2" |> registerCommand Aborting aborting) |> NoChilds;
        ("Context" :> obj |> PackMLModel.defaultModel "1.1.3" |> registerCommand Aborting aborting) |> NoChilds;
        ("Context" :> obj |> PackMLModel.defaultModel "1.1.4" |> registerCommand Aborting aborting) |> NoChilds])) |> Childs;
    ("Context" :> obj |> PackMLModel.defaultModel "1.2" |> registerCommand Aborting aborting) |> NoChilds;
    ("Context" :> obj |> PackMLModel.defaultModel "1.3" |> registerCommand Aborting aborting) |> NoChilds;
    ("Context" :> obj |> PackMLModel.defaultModel "1.4" |> registerCommand Aborting aborting) |> NoChilds;
    ("Context" :> obj |> PackMLModel.defaultModel "1.5" |> registerCommand Aborting aborting) |> NoChilds;
    (("Context" :> obj |> PackMLModel.defaultModel "1.6" |> registerCommand Aborting aborting), (
        [("Context" :> obj |> PackMLModel.defaultModel "1.6.1" |> registerCommand Aborting aborting) |> NoChilds;
        (("Context" :> obj |> PackMLModel.defaultModel "1.6.2" |> registerCommand Aborting aborting), (
            [("Context" :> obj |> PackMLModel.defaultModel "1.6.2.1" |> registerCommand Aborting aborting) |> NoChilds;
            ("Context" :> obj |> PackMLModel.defaultModel "1.6.2.2" |> registerCommand Aborting aborting) |> NoChilds;
            ("Context" :> obj |> PackMLModel.defaultModel "1.6.2.3" |> registerCommand Aborting aborting) |> NoChilds;
            ("Context" :> obj |> PackMLModel.defaultModel "1.6.2.4" |> registerCommand Aborting aborting) |> NoChilds])) |> Childs;
        ("Context" :> obj |> PackMLModel.defaultModel "1.6.3" |> registerCommand Aborting aborting) |> NoChilds;
        ("Context" :> obj |> PackMLModel.defaultModel "1.6.4" |> registerCommand Aborting aborting) |> NoChilds])) |> Childs;
    ("Context" :> obj |> PackMLModel.defaultModel "1.7" |> registerCommand Aborting aborting) |> NoChilds]) |> Childs

[<Fact>]
let ``State command should change all to aborted`` () =
    let expectedState = Aborted
    let assertNodes x =
        x.CurrentState |> should equal expectedState

    nodes |> map stateChange |> iter assertNodes
        
[<Fact>]
let ``Run command should change all contexts`` () =
    let assertNodes ( x: PackMLModel<obj> ) =
        x.Context.ContextData :?> string |> should equal ``Expected context data after abort``

    nodes |> map run |> iter assertNodes
