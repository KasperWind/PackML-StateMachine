module StateMachine.PackMLManager

open PackML

type PackMLManager = 
    | NoChilds of node: PackMLModel<obj>
    | Childs of node: PackMLModel<obj> * nodes: PackMLManager list

let rec map ( f: PackMLModel<obj> -> PackMLModel<obj> ) ( x: PackMLManager ) : PackMLManager =
    match x with
    | NoChilds me -> f me |> NoChilds
    | Childs (me, childs) ->
        let n = f me
        let nt = childs |> List.map (map f)
        (n, nt) |> Childs
