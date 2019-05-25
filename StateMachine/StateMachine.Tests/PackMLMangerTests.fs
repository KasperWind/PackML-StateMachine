module PackMLMangerTests

open System
open Xunit
open FsUnit.Xunit
open StateMachine.FSM
open StateMachine.PackML
open StateMachine.PackMLManager

let expectedAfterRun = PackMLContext.defaultContext "has been run..."
let expectedRun _ = expectedAfterRun
let node = "single parent" |> PackMLModel.defaultModel |> registerCommand Aborting expectedRun |> NoChilds
    


[<Fact>]
let ``Should run all PackMLModels in container`` () =
    let expectetedContext = "has been run"
    let expectedList = List.init modules.Count (fun _ -> expectetedContext)
    let command ctx = { ctx with ContextData = expectetedContext }
    
    modules 
    |> Map.map (fun _ state -> state |> registerCommand Aborting command)
    |> runAll
    |> Map.toList
    |> List.map (snd >> getContext >> getContextData)
    |> should matchList expectedList



    (*
    type Modules = | Module1 | Module2 | Module3
    
    let modules = 
        [(Module1, {TransitionState = FirstRun; ContextData = ""}); 
        (Module2, {TransitionState = FirstRun; ContextData = ""}); 
        (Module3, {TransitionState = FirstRun; ContextData = ""});]
        |> List.map (fun (fst,snd) -> fst, snd |> stateModel)
        |> Container.ofList
        *)