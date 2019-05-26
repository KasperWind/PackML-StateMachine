module StateMachine.PackML
open FSM

//PackML model

type Events =
    | StateChange
    | Abort | Clear
    | Stop | Reset
    | Start | Hold | UnHold | Suspend | UnSuspend

type States =
    | Aborting | Aborted
    | Clearing | Stopping | Stopped
    | Resetting | Idle | Starting | Execute
    | Suspending | Suspended | Unsuspending
    | Holding | Held | Unholding
    | Completing | Complete

let abortableStates = [
    Clearing; Stopping; Stopped;
    Resetting; Idle; Starting; Execute; 
    Suspending; Suspended; Unsuspending; 
    Holding; Held; Unholding; 
    Completing; Complete ]

let stopableStates = [
    Resetting; Idle; Starting; Execute; 
    Suspending; Suspended; Unsuspending; 
    Holding; Held; Unholding; 
    Completing; Complete ]
    
type TransitionState = | FirstRun | Running | IsDone
type PackMLContext<'a> = {Name: string; TransitionState : TransitionState; ContextData : 'a}
type PackMLModel<'a> = FSM<States,Events, PackMLContext<'a>>
type CommandModel<'a> = PackMLModel<'a> -> PackMLModel<'a>

let stateModel<'a> (context:PackMLContext<'a>) : PackMLModel<'a> =
    initialize context States.Aborting
    |> registerResetEvent abortableStates Abort Aborting 
    |> registerResetEvent stopableStates Stop Stopping
    |> registerTransition Aborting StateChange Aborted
    |> registerTransition Aborted Clear Clearing
    |> registerTransition Clearing StateChange Stopped
    |> registerTransition Stopping StateChange Stopped
    |> registerTransition Stopped Reset Resetting
    |> registerTransition Resetting StateChange Idle
    |> registerTransition Idle Start Starting
    |> registerTransition Starting StateChange Execute
    |> registerTransition Execute StateChange Completing
    |> registerTransition Execute Suspend Holding
    |> registerTransition Execute Hold Suspending
    |> registerTransition Completing StateChange Complete
    |> registerTransition Complete Reset Resetting
    |> registerTransition Suspending StateChange Suspended
    |> registerTransition Suspended UnSuspend Unsuspending
    |> registerTransition Unsuspending StateChange Execute
    |> registerTransition Holding StateChange Held
    |> registerTransition Held UnSuspend Unholding
    |> registerTransition Unholding StateChange Execute

//PackML helper functions
module PackMLContext =
    let setTransitionState<'a> state ( context : PackMLContext<'a> ) = { context with TransitionState = state }
    let setFirstRun<'a> : PackMLContext<'a> -> PackMLContext<'a> = setTransitionState FirstRun
    let setRunning<'a> : PackMLContext<'a> -> PackMLContext<'a> = setTransitionState Running
    let setIsDone<'a> : PackMLContext<'a> -> PackMLContext<'a> = setTransitionState IsDone
    let defaultContext name data = {Name = name; TransitionState = FirstRun; ContextData = data}
    let getContextData context = context.ContextData
    let getName context = context.Name

module PackMLModel =
    let setTransitionState<'a> state (fsm : PackMLModel<'a>) = {fsm with Context = fsm.Context |> PackMLContext.setTransitionState state}
    let setFirstRun<'a> : PackMLModel<'a> -> PackMLModel<'a> = setTransitionState FirstRun
    let setRunning<'a> : PackMLModel<'a> -> PackMLModel<'a> = setTransitionState Running
    let setIsDone<'a> : PackMLModel<'a> -> PackMLModel<'a> = setTransitionState IsDone
    let getName model = PackMLContext.getName model.Context
    let defaultModel<'a> name data = (PackMLContext.defaultContext name data) |> stateModel<'a>
    
let eventHandler e fsm =
    let checkEqual newFsm =
        if fsm.CurrentState = newFsm.CurrentState then fsm
        else newFsm |> PackMLModel.setFirstRun
    fsm |> event e |> checkEqual
    
let checkFirstRun fsm =
    match fsm.Context.TransitionState with
    | FirstRun -> fsm |> PackMLModel.setRunning
    | _ -> fsm

let stateChange<'a> : CommandModel<'a> = eventHandler StateChange
let abort<'a>  : CommandModel<'a> = eventHandler Abort
let clear<'a>  : CommandModel<'a> = eventHandler Clear
let stop<'a>  : CommandModel<'a> = eventHandler Stop
let reset<'a>  : CommandModel<'a> = eventHandler Reset
let start<'a>  : CommandModel<'a> = eventHandler Start
let hold<'a>  : CommandModel<'a> = eventHandler Hold
let unHold<'a>  : CommandModel<'a> = eventHandler UnHold
let suspend<'a>   : CommandModel<'a> = eventHandler Suspend
let unSuspend<'a>  : CommandModel<'a> = eventHandler UnSuspend
let run<'a> (stateModel: PackMLModel<'a>) : PackMLModel<'a> = stateModel |> FSM.command |> checkFirstRun
let isAbortableState state = abortableStates |> List.contains state
let isStopableState state = stopableStates |> List.contains state
