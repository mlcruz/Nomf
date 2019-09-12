module AsyncFileWatcher

open System.IO

type AsyncFileWatcherConfig () =
    
    let onNewLine  = printf "%s"
    member val path = @"D:\git\test.txt"  with get, set
    member val action  = onNewLine with get, set

type AsyncFileWatcher (config : AsyncFileWatcherConfig option) =
    let config = 
        match config with 
            | Some x -> x
            | None -> new AsyncFileWatcherConfig()

    let pathDirectories =  config.path.Split(Path.DirectorySeparatorChar)
    let dir = (pathDirectories |> Array.splitAt(pathDirectories.Length - 1) |> fst  |> String.concat("/")) + "/"
    let file = Array.last pathDirectories
    let timer = new System.Threading.AutoResetEvent(false)
    let fswatcher = new System.IO.FileSystemWatcher(dir)
    
    let filestream = new System.IO.FileStream(config.path,System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite)
    let fs = new System.IO.StreamReader(filestream)

    let (charArr : char array) = Array.zeroCreate(4096)
    let mutable curLine = ""
    
    let rec cullLoop bytesRead = 
        match bytesRead with 
            | 0 -> ()
            | _ -> 
                fs.DiscardBufferedData ()
                (charArr,0,4095) |> fs.Read |> cullLoop

    do fs.Read(charArr,0,4095) |> cullLoop 

    do fswatcher.Filter <- file
    do fswatcher.EnableRaisingEvents <- true
    do fswatcher.Changed.Add( fun _ -> timer.Set() |> ignore)

    let rec AsyncInputLoop (config : AsyncFileWatcherConfig) : AsyncFileWatcherConfig = 
        curLine <- fs.ReadLine() 
        if (curLine <> null) then config.action curLine else timer.WaitOne(1) |> ignore
        AsyncInputLoop config

    member this.Start() = AsyncInputLoop config |> ignore
