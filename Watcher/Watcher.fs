module Watcher
open CommonLibrary
open System.IO
open System.Threading.Tasks

let (|SystemCommand|UserCommand|) (cmd : string) =
    match cmd with
        | command when 
            cmd.ToCharArray()  |> Array.splitAt(3) |> fst |> charArrayToString = "sys" ->  SystemCommand cmd
        | _ -> UserCommand cmd
    
type Command = 
    | SystemCommand of string
    | UserCommand of string

type Msg =  Command * string


type WatcherFunctions = 
    { 
    transform: string -> string
    action: string -> unit;
    stateMatcher : Command -> (string -> unit) * WatcherFunctions -> (string -> unit) * WatcherFunctions
    }

type WatcherConfig () =

    let stateMatcher (cmd : Command ) watcher =
        let (oldAction, oldState) = watcher
        let newState =
            match cmd with
            | SystemCommand _ -> (oldAction, oldState)
            | UserCommand _ -> (oldAction, oldState)
        newState

    let watcherFunctions : WatcherFunctions = {
        action = printf "%s"
        stateMatcher = stateMatcher;
        transform = fun x -> x;
        }

    member val WatcherFunctions = watcherFunctions  with get, set


type Watcher (config : WatcherConfig option) =

    let initializedConfig = 
               match config with
                   | Some conf -> conf
                   | None  -> new WatcherConfig ()

    let watcherMailbox = MailboxProcessor.Start(fun inbox-> 
          let rec messageLoop (watcher : (string -> unit) * WatcherFunctions) = async{
              let! (message : Msg) = inbox.Receive()
              let (oldAction, oldState) = watcher
              
              let newState = 
                match message with
                    | SystemCommand cmd, msg when string cmd = "sysdie" -> None
                    | UserCommand _ , msg -> 
                        match msg with 
                            | _ -> 
                                oldAction msg
                                Some(watcher)
                    | _ -> Some(watcher)

            match newState with
                | Some st -> return! messageLoop st
                | None _ -> return! (Async.AwaitTask Task.CompletedTask)
              }
  
          messageLoop (initializedConfig.WatcherFunctions.action, initializedConfig.WatcherFunctions)
          )

    member this.Mailbox = watcherMailbox
