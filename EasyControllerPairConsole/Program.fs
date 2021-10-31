open System
open BluetoothService
open BluetoothService

[<EntryPoint>]
let main _ =
    // TODO: consider doing this in parallel and let the user interact with the app
    // TODO: merge similar methods to minimize boiler plate.
    let showAvailableControllers () =
        printfn "\nsearching for devices..."
        let devices = getAllAvailableDevices ()

        match devices.Length with
        | 0 -> printfn "\nno available devices!"
        | _ ->
            printfn "\navailable devices:"

            devices
            |> List.iter
                (fun device -> printfn $"\t%s{device.DeviceName}, address: %s{device.DeviceAddress.ToString()}\n")


    let showPairedControllers () =
        let devices = getAllPairedDevices ()

        match devices.Length with
        | 0 -> printfn "\nno devices are paired!"
        | _ ->
            printfn "\nPaired devices:"

            devices
            |> List.map
                (fun device ->
                    $"name: %s{device.DeviceName}, connected: %b{device.Connected}, address: %s{device.DeviceAddress.ToString()}")
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

    // TODO: load from file (for practice)
    let showHelpText () =
        printfn
            "
        press t/T to show available controllers to pair
        press y/Y to show paired controllers
        press u/U to pair a controller
        press i/I to remove a controller
        press o/O to reconnect a controller
        press p/P to connect all controllers that are available
        press j/J to remove all the controllers that are paired
        press k/K to reconnect all the controllers that are paired"

    // TODO: create config file
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

    printfn "please select a key. press h for help and press q to exist\n"
    checkForKeys (Console.ReadKey().Key)
    0
