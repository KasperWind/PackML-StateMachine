module StateMachine.PackMLManager

open PackML

type Container<'a> = Map< string, PackMLModel<'a> >