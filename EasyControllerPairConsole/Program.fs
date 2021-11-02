open System
open BluetoothService
open BluetoothService
open Cache

[<EntryPoint>]
let main _ =

    // TODO: consider doing this in parallel and let the user interact with the app
    // TODO: merge similar methods to minimize boiler plate.
    let showAvailableControllers () =
        printfn "\nsearching for devices..."
        let devices = getAllAvailableDevices ()

        match devices.Length with
        | 0 ->
            printfn "no available devices!"
            false
        | _ ->
            printfn "available devices:"

            let devicesToCache =
                devices
                |> List.groupBy (fun device -> device.DeviceName)
                |> List.map snd
                |> List.collect
                    (fun list ->
                        list
                        |> List.mapi
                            (fun index device ->
                                match index with
                                | 0 ->
                                    { DeviceKey = device.DeviceName
                                      DeviceData = device }
                                | _ ->
                                    { DeviceKey = $"%s{device.DeviceName} %d{index}"
                                      DeviceData = device }))

            saveToCache devicesToCache

            devicesToCache
            |> List.iter
                (fun device ->
                    printfn $"\t* %s{device.DeviceKey}, address: %s{device.DeviceData.DeviceAddress.ToString()}")

            true


    let showPairedControllers () =
        let devices = getAllPairedDevices ()

        match devices.Length with
        | 0 ->
            printfn "\nno devices are paired!"
            false
        | _ ->
            printfn "\npaired devices:"

            devices
            |> List.map
                (fun device ->
                    $"name: %s{device.DeviceName}, connected: %b{device.Connected}, address: %s{device.DeviceAddress.ToString()}")
            |> List.iter (fun device -> printfn $"\t%s{device}\n")
            true


    let rec pairControllerByName () =

        match cacheIsEmpty () with
        | true ->
            match showAvailableControllers () with
            | true -> pairControllerByName ()
            | false -> () // return
        | false ->
            printfn "\nplease insert controller name, empty name will bring you back"
            let input = Console.ReadLine()

            match existInCache input with
            | true ->
                printfn "pairing controller"
                let device = getDeviceFromCache input

                let success =
                    pairController device.DeviceAddress "5555"

                match success with
                | true ->
                    printfn $"connected %s{input}"
                    removeFromCache input |> ignore

                | false -> printfn $"something went wrong, couldn't connect %s{input}"
            | false ->
                match String.IsNullOrEmpty input with
                | true -> () // return
                | false ->
                    printfn $"%s{input} is no valid controller"
                    pairControllerByName ()


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
    // TODO show help message each time when return to this method.
    let rec checkForKeys (key: ConsoleKey) =
        
        let onInputProcessFinished() =
            printfn "\nplease select a key. press h for help and press q to exist\n"
            checkForKeys (Console.ReadKey().Key)
            
        match key with
        | ConsoleKey.T ->
            showAvailableControllers () |> ignore
            onInputProcessFinished()
        | ConsoleKey.Y ->
            showPairedControllers () |> ignore
            onInputProcessFinished()
        | ConsoleKey.U ->
            pairControllerByName ()
            onInputProcessFinished()
        | ConsoleKey.I ->
            removeControllerByName ()
            onInputProcessFinished()
        | ConsoleKey.O ->
            reconnectController ()
            onInputProcessFinished()
        | ConsoleKey.P ->
            connectAllControllers ()
            onInputProcessFinished()
        | ConsoleKey.J ->
            removeAllControllers ()
            onInputProcessFinished()
        | ConsoleKey.K ->
            reconnectAllControllers ()
            onInputProcessFinished()
        | ConsoleKey.H ->
            showHelpText ()
            onInputProcessFinished()
        | ConsoleKey.Q -> ()
        | _ ->
            printfn "\nwrong key pressed!"
            onInputProcessFinished()

    printfn "please select a key. press h for help and press q to exist\n"
    checkForKeys (Console.ReadKey().Key)
    0
