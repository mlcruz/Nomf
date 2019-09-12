module LogWatcher
open AsyncFileWatcher
open Watcher


type LogWatcher (path : string) = 

    let defaultWatcher = new Watcher (None)
    let defaultFileWatcherConfig = new AsyncFileWatcherConfig()
    do defaultFileWatcherConfig.path <- path 
    do defaultFileWatcherConfig.action <- (fun i -> defaultWatcher.Mailbox.Post(UserCommand "print" , i) )
    let defaultFileWatcher = new AsyncFileWatcher (Some(defaultFileWatcherConfig))
    member this.Start () = async{defaultFileWatcher.Start () |> ignore} 
