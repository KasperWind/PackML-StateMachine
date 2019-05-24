module StateMachine.PackMLManager

open PackML

type Container<'a,'b when 'b : comparison> = Map< 'b, PackMLModel<'a> >

let runSingle selector ( container :  Container<'a, 'b> ) : Container<'a, 'b> =
    container |> Map.tryFind selector
    |> function
        | None -> container
        | Some s -> container |> Map.add selector (s |> run)
    
let runAll (container : Container<'a, 'b>) : Container<'a, 'b> =
    container
    |> Map.map ( fun _ model -> model |> run )