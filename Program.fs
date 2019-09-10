open System
open System.Threading
open Suave

type SubprocessResult = {
    exitCode : int
    stdout : string
    stderr : string
}

let runCmd command = 
    let subproc = System.Diagnostics.Process.Start("cmd", sprintf "/c %s" <| command)
    subproc.WaitForExit()
    {exitCode = subproc.ExitCode ; stdout = subproc.StandardOutput.ReadToEnd(); stderr = subproc.StandardError.ReadToEnd()}



[<EntryPoint>]
let main argv = 
  //let cts = new CancellationTokenSource()
  //let conf = { defaultConfig with cancellationToken = cts.Token }
  //let listening, server = startWebServerAsync conf (Successful.OK "Hello World")
    
  //Async.Start(server, cts.Token)
  //printfn "Make requests now"
  //Console.ReadKey true |> ignore
  //cts.Cancel()

  Console.WriteLine(Environment.CurrentDirectory)
 
  0 // return an integer exit code