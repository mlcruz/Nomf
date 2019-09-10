module App

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props

// MODEL
type Model = {isAlertOpen : bool ; alertText : string}

type Msg =
| HappyAlert
| SadAlert

let init() : Model = {isAlertOpen = false ; alertText = ""}

// UPDATE
let update (msg:Msg) (model:Model) =
    match msg with
    | HappyAlert ->
        Fable.Core.JS.eval("alert(':)')") |> ignore
        {isAlertOpen = true; alertText = ":)"}
    | SadAlert -> 
        Fable.Core.JS.eval("alert(':(')") |> ignore
        {isAlertOpen = true; alertText = ":("}

// VIEW (rendered with React)
let view (model:Model) dispatch =
  div []
    [
        textarea [Style [Border "1px solid red"] ] []
        br []
        button [Style [Height 20; Width 50] ; OnClick (fun _ -> dispatch HappyAlert) ] [str model.alertText]
    ]

// App
Program.mkSimple init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
