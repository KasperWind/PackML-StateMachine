module StateMachine.PackMLManager

open PackML

type PackMLManager = 
    | NoChilds of current: PackMLModel<obj>
    | Childs of current: PackMLModel<obj> * childs: PackMLManager list

let rec map f x =
    match x with
    | NoChilds me -> f me |> NoChilds
    | Childs (me, childs) ->
        let n = f me
        let nt = childs |> List.map (map f)
        (n, nt) |> Childs
        
let rec iter f x =
    match x with
    | NoChilds me -> f me 
    | Childs (me, childs) ->
        f me
        childs |> List.iter (iter f)

let printStates manager =    
    let printState model =
        (printfn "Statemodel: %s; Current state: %A" (PackMLModel.getName model) (model.CurrentState))
    manager |> iter printState
    manager

