module PackMLTests

open System
open System.Diagnostics
open Xunit
open FsUnit.Xunit
open StateMachine.FSM
open StateMachine.PackML

let rec commandFlow list fsm =
    Debug.Write(sprintf "current state:%A " fsm.CurrentState)
    match list with
    | [] -> 
        Debug.WriteLine("done.")
        fsm
    | [x] ->
        Debug.Write(sprintf "-> %A -> " x)
        fsm |> x |> commandFlow []
    | x::xs -> 
        Debug.Write(sprintf "-> %A -> " x)
        fsm |> x |> commandFlow xs

let ``Goes to starting from aborting`` = [ stateChange; clear; stateChange; reset; stateChange; start ]
let ``Goes to execute from aborting`` = ``Goes to starting from aborting`` @ [ stateChange ]

let emptyPackMLContext = {Name = "Test"; TransitionState = FirstRun; ContextData = ""}

[<Fact>]
let ``Should be in aborting state and with inline data context`` () =
    let expectedContext = {Name = "Test"; TransitionState = FirstRun; ContextData = "Expected Context"}
    stateModel expectedContext |> getContext
    |> should equal expectedContext

[<Fact>]
let ``Should go to starting state`` () =
    stateModel emptyPackMLContext |> commandFlow ``Goes to starting from aborting``
    |> getCurrentState |> should equal Starting

[<Fact>]
let ``Should go to execute state`` () =
    stateModel emptyPackMLContext |> commandFlow ``Goes to execute from aborting``
    |> getCurrentState |> should equal Execute

[<Fact>]
let ``Should go to starting and abort`` () =
    let actual = stateModel emptyPackMLContext |> commandFlow ``Goes to starting from aborting``
    actual |> getCurrentState |> should equal Starting
    actual |> abort |> getCurrentState |> should equal Aborting


[<Fact>]
let ``Should go to execute and abort`` () =
    let actual = stateModel emptyPackMLContext |> commandFlow ``Goes to execute from aborting``
    actual |> getCurrentState |> should equal Execute
    actual |> abort |> getCurrentState |> should equal Aborting

[<Fact>]
let ``Should go to starting and stop`` () =
    let actual = stateModel emptyPackMLContext |> commandFlow ``Goes to starting from aborting``
    actual |> getCurrentState |> should equal Starting
    actual |> stop |> getCurrentState |> should equal Stopping


[<Fact>]
let ``Should go to execute and stop`` () =
    let actual = stateModel emptyPackMLContext |> commandFlow ``Goes to execute from aborting``
    actual |> getCurrentState |> should equal Execute
    actual |> stop |> getCurrentState |> should equal Stopping

[<Fact>]
let ``Should execute command from aborting`` () =
    let expectedContext = {Name = "Test"; TransitionState = Running; ContextData = "Expected Context"}
    stateModel emptyPackMLContext |> registerCommand Aborting (fun x -> expectedContext)
    |> run |> getContext |> should equal expectedContext

[<Fact>]
let ``Should execute command from execute`` () =
    let expectedContext = {Name = "Test"; TransitionState = Running; ContextData = "Expected Context"}
    stateModel emptyPackMLContext |> registerCommand Execute (fun x -> expectedContext)
    |> commandFlow ``Goes to execute from aborting``
    |> run |> getContext |> should equal expectedContext

[<Fact>]
let ``Aborting should not be stoppable state`` () =
    stateModel emptyPackMLContext |> getCurrentState |> isStopableState
    |> should be False

[<Fact>]
let ``Aborting should not be abortable state`` () =
    stateModel emptyPackMLContext |> getCurrentState |> isAbortableState
    |> should be False

[<Fact>]
let ``Stopped should be abortable state`` () =
    stateModel emptyPackMLContext |> stateChange |> clear |> stateChange |> getCurrentState |> isAbortableState
    |> should be True

[<Fact>]
let ``Starting should be stoppable state`` () =
    stateModel emptyPackMLContext |> commandFlow ``Goes to starting from aborting`` |> getCurrentState |> isAbortableState
    |> should be True