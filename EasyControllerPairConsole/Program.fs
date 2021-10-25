open System

//let temp =
//    while true do
//        printfn "please select a key"
//        let key = Console.ReadKey()
//        printfn $"chosen key: %s{key.Key.ToString()}"


[<EntryPoint>]
let main _ =
    let doStuff key =
        printfn $"\nDoing stuff with key: %c{key}"

    let rec checkForKeys (key: char) =
        match key with
        | 'w' ->
            doStuff key
            checkForKeys (Console.ReadKey().KeyChar)
        | 'e' ->
            doStuff key
            checkForKeys (Console.ReadKey().KeyChar)
        | 'r' ->
            doStuff key
            checkForKeys (Console.ReadKey().KeyChar)
        | 'q' -> ()
        | _ ->
            printfn "\nWrong key pressed!"
            checkForKeys (Console.ReadKey().KeyChar)

    checkForKeys (Console.ReadKey().KeyChar)
    0


