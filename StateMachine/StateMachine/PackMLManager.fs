module StateMachine.PackMLManager

open PackML

type PackMLManager<'a> = 
    | NoChilds of node: PackMLModel<'a>
    | Childs of node: PackMLModel<'a> * nodes: PackMLManager<'a> list

module PackMLManager =
    let rec map ( f: PackMLModel<'a> -> PackMLModel<'b> ) ( x: PackMLManager<'a> ) : PackMLManager<'b> =
        match x with
        | NoChilds me -> f me |> NoChilds
        | Childs (me, childs) ->
            let n = f me
            let nt = childs |> List.map (map f)
            (n, nt) |> Childs
(*        
type Container<'a,'b when 'a : comparison> = Map< 'a, PackMLModel<'b> >

module Container =
    let ofList list : Container<'a, 'b>  = list |> Map.ofList

let runSingle selector ( container :  Container<'a, 'b> ) : Container<'a, 'b> =
    container |> Map.tryFind selector
    |> function
        | None -> container
        | Some s -> container |> Map.add selector (s |> run)
    
let runAll (container : Container<'a, 'b>) : Container<'a, 'b> =
    container
    |> Map.map ( fun _ model -> model |> run )

// f: b -> c x: Container<a, b>
let map f x =
    ()
    *)