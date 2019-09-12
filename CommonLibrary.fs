module CommonLibrary 

open System

let LockingColorPrint =
    let lockObj = obj()
    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printf "%s" s
            Console.ForegroundColor <- ConsoleColor.White
            )

let LockingColorPrintLn =
    let lockObj = obj()
    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printfn "%s" s
            Console.ForegroundColor <- ConsoleColor.White)


let charArrayToString (achar : char[]) = 
    System.String.Concat(achar)



        
