open System
open BluetoothService
open BluetoothService

[<EntryPoint>]
let main _ =

    // TODO: consider doing this in parallel and let the user interact with the app
    let showAvailableControllers () =
        printfn "\nshowing available controllers"

    let showPairedControllers () =
        let devices = getAllPairedDevices ()

        match devices.Length with
        | 0 -> printfn "No devices are paired!"
        | _ ->
            printfn "\nPaired devices:"
            devices
            |> List.iter (fun device -> printfn $"\t%s{device}\n")

    // TODO: handle wrong input case.
    // TODO: back if no input
    let pairControllerByName () =
        printfn "\nplease insert controller name, empty name will bring you back"
        let input = Console.ReadLine()
        printfn $"pairing controller: %s{input}"

    // TODO: handle wrong input case.
    // TODO: back if no input
    let removeControllerByName () =
        printfn "\nplease insert controller name, empty name will bring you back" // TODO: cache string?
        let input = Console.ReadLine() // TODO: maybe can reuse this
        printfn $"removing controller: %s{input}"

    let reconnectController () =
        printfn "\nplease insert controller name, empty name will bring you back" // TODO: cache string?
        let input = Console.ReadLine() // TODO: maybe can reuse this
        printfn $"reconnecting controller: %s{input}"

    let connectAllControllers () =
        printfn "connecting all available controllers" // TODO: log all controllers

    let removeAllControllers () = printfn "removing all controllers" // TODO: log all controllers

    let reconnectAllControllers () =
        printfn "\nreconnecting all paired controllers" // TODO: log all controllers

    let showHelpText () =
        printfn "
        press t/T to show available controllers to pair
        press y/Y to show paired controllers
        press u/U to pair a controller
        press i/I to remove a controller
        press o/O to reconnect a controller
        press p/P to connect all controllers that are available
        press j/J to remove all the controllers that are paired
        press k/K to reconnect all the controllers that are paired"

    let rec checkForKeys (key: ConsoleKey) =
        match key with
        | ConsoleKey.T ->
            showAvailableControllers ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.Y ->
            showPairedControllers ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.U ->
            pairControllerByName ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.I ->
            removeControllerByName ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.O ->
            reconnectController ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.P ->
            connectAllControllers ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.J ->
            removeAllControllers ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.K ->
            reconnectAllControllers ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.H ->
            showHelpText ()
            checkForKeys (Console.ReadKey().Key)
        | ConsoleKey.Q -> ()
        | _ ->
            printfn "\nWrong key pressed!"
            checkForKeys (Console.ReadKey().Key)

    printfn "please select a key. press h for help, press q to exist\n"
    checkForKeys (Console.ReadKey().Key)
    0
