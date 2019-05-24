module FSM_Tests

open System
open Xunit
open StateMachine.FSM

[<Fact>]
let ``Should create empty state with context`` () =
    let expectedState = "Start State"
    let expectedContext = "Empty context"
    
    let actual = initialize expectedContext expectedState

    Assert.Equal(expectedState, actual.CurrentState)
    Assert.Equal(expectedContext, actual.Context)
    Assert.Equal(expectedState, actual.InitialState)

[<Fact>]
let ``Should add reset event`` () =
    let expectedState = "Start State"
    let expectedResetEvent = "Reset Event"
    
    let actual  =
        initialize () expectedState
        |> registerResetEvent [expectedState] expectedResetEvent expectedState

    Assert.Equal(1, actual.ResetEvents.Count)
    Assert.Contains(expectedResetEvent, actual.ResetEvents |> Map.toSeq |> dict)

[<Fact>]
let ``Should transition to state after normal 1 event`` () =
    let startState = "Start State"
    let expectedState = "Expected State"
    let eventToFire = "Event"

    let actual =
        initialize () startState
        |> registerTransition startState eventToFire expectedState
        |> event eventToFire

    Assert.Equal(expectedState, actual.CurrentState)

[<Fact>]
let ``Should transition to state after normal 2 event`` () =
    let startState = "Start State"
    let state2 = "State2"
    let expectedState = "Expected State"
    let eventToFire = "Event"

    let actual =
        initialize () startState
        |> registerTransition startState eventToFire state2
        |> registerTransition state2 eventToFire expectedState
        |> event eventToFire
        |> event eventToFire

    Assert.Equal(expectedState, actual.CurrentState)

[<Fact>]
let ``Should transition when doing reset event`` () =
    let startState = "Start State"
    let state2 = "State2"
    let expectedState = "Reset State"
    let eventToFirst = "EventToFire"
    let resetEvent = "Reset Event"

    let actual =
        initialize () startState
        |> registerTransition startState eventToFirst state2
        |> registerResetEvent [state2] resetEvent expectedState
        |> event eventToFirst
        |> event resetEvent

    Assert.Equal(expectedState, actual.CurrentState)

[<Fact>]
let ``Should not transition when doing reset event from wrong state`` () =
    let startState = "Start State"
    let expectedState = "State2"
    let resetState = "Reset State"
    let eventToFirst = "EventToFire"
    let resetEvent = "Reset Event"

    let actual =
        initialize () startState
        |> registerTransition startState eventToFirst expectedState
        |> registerResetEvent [startState] resetEvent resetState
        |> event eventToFirst
        |> event resetEvent

    Assert.Equal(expectedState, actual.CurrentState)

[<Fact>]
let ``Should fire command from state`` () =
    let state1 = "State1"
    let state2 = "State2"
    let eventToFire = "event"
    let context = "start"
    let expectedContext = "Expected context after command"

    let actual =
        initialize context state1
        |> registerTransition state1 eventToFire state2
        |> registerCommand state2 (fun x -> expectedContext)
        |> event eventToFire
        |> command

    Assert.Equal(expectedContext, actual.Context)

[<Fact>]
let `` Should not fire command from wrong state`` () =
    let state1 = "State1"
    let state2 = "State2"
    let eventToFire = "event"
    let context = "start"
    let expectedContext = "Expected context after command"

    let actual =
        initialize expectedContext state1
        |> registerTransition state1 eventToFire state2
        |> registerCommand state1 (fun x -> context)
        |> event eventToFire
        |> command

    Assert.Equal(expectedContext, actual.Context)

