module StateMachine.FSM
type T<'TState, 'TEvent, 'TContext when 'TEvent : comparison and 'TState : comparison> = {
    Transitions : Map<'TState, Map<'TEvent, 'TState>>
    CurrentState : 'TState
    InitialState : 'TState
    ResetEvents : Map<'TEvent, ('TState * 'TState list)>
    Commands : Map<'TState, ('TContext -> 'TContext)>
    Context : 'TContext}

let initialize context state =
    { Transitions = Map.empty; CurrentState = state; InitialState = state; ResetEvents = Map.empty; Commands = Map.empty; Context = context }

let registerResetEvent fromStates event toState  fsm = {fsm with ResetEvents = fsm.ResetEvents |> Map.add event (toState,fromStates)}

let registerTransition currentState event nextState fsm =
    match fsm.Transitions |> Map.tryFind currentState with
    | None -> {fsm with Transitions = fsm.Transitions |> Map.add currentState (Map.empty |> Map.add event nextState)}
    | Some m -> {fsm with Transitions = fsm.Transitions |> Map.add currentState (m |> Map.add event nextState)}

let registerCommand state command fsm =
    {fsm with Commands = fsm.Commands |> Map.add state command}

let event e fsm =
    let transitionTo state fsm = 
        {fsm with CurrentState = state}

    let findStateInResetEvents resetEvent =
        let (nextState, states) = resetEvent
        if states |> List.isEmpty then fsm.InitialState |> Some
        elif List.contains fsm.CurrentState states then nextState |> Some
        else None

    let findResetEvent resetEvents =
        resetEvents |> Map.tryFind e |> Option.bind findStateInResetEvents

    match fsm.ResetEvents |> findResetEvent with
    | Some ns ->
        transitionTo ns fsm
    | None ->
        fsm.Transitions
        |> Map.tryFind fsm.CurrentState
        |> Option.bind (Map.tryFind e)
        |> Option.bind (fun nextState -> Some(transitionTo nextState fsm))
        |> function | None -> fsm | Some fsm' -> fsm'

let command fsm =
    match fsm.Commands |> Map.tryFind fsm.CurrentState with
    | None -> fsm
    | Some x -> {fsm with Context = x (fsm.Context)}
