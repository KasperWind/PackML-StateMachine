#load "../StateMachine/FSM.fs"
#load "../StateMachine/PackML.fs"
#load "../StateMachine/PackMLManager.fs"

open StateMachine.FSM
open StateMachine.PackML
open StateMachine.PackMLManager

let aborting (context: PackMLContext<obj>) =
    let x = PackMLContext.getContextData context :?> string
    printfn "Aborting statemodel: %s with context: %s" (PackMLContext.getName context) x
    context

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

nodes |> map run |> ignore
nodes |> map stateChange