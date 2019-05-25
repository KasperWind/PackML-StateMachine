module StateMachine.PackMLManager

open PackML

//TODO: as node tree?
type PackMLManager<'a> = 
    | NoChilds of node: PackMLModel<'a>
    | Childs of node: PackMLModel<'a> * nodes: PackMLManager<'a> list

module NodeTree =
    let map (f:PackMLModel<'a> -> PackMLModel<'b>) (x:PackMLManager<'a>) : PackMLManager<'b> =
        let rec innerF nodetree =
            match nodetree with
            | NoChilds node -> f node |> NoChilds
            | Childs (node, nodes) ->
                let n = f node
                let nt = nodes |> List.map innerF
                (n, nt) |> Childs
        x |> innerF

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