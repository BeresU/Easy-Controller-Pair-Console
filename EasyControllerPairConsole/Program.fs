open System
open BluetoothService
open BluetoothService
open Cache
open InTheHand.Net.Sockets

[<EntryPoint>]
let main _ =

    let mapToDeviceData (devices: BluetoothDeviceInfo list) =
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

    let insertDeviceMessage =
        "\nplease insert device name, empty name will bring you back"

    // TODO: consider doing this in parallel and let the user interact with the app
    // TODO: merge similar methods to minimize boiler plate.
    let showAvailableDevices () =
        printfn "\nsearching for devices..."
        let devices = getAllAvailableDevices ()

        match devices.Length with
        | 0 ->
            printfn "no available devices!"
            false
        | _ ->
            let devicesToCache = mapToDeviceData devices
            saveToCache devicesToCache
            printfn "available devices:"

            devicesToCache
            |> List.iter
                (fun device ->
                    printfn $"\t* name: %s{device.DeviceKey}, address: %s{device.DeviceData.DeviceAddress.ToString()}")

            true

    let showPairedDevices () =
        let devices = getAllPairedDevices ()

        match devices.Length with
        | 0 ->
            printfn "\nno devices are paired!"
            false
        | _ ->
            printfn "\npaired devices:"

            devices
            |> mapToDeviceData
            |> List.iter
                (fun device ->
                    printfn
                        $"\t* name: %s{device.DeviceKey}, connected: %b{device.DeviceData.Connected}, address: %s{device.DeviceData.DeviceAddress.ToString()}\n")

            true

    let rec pairDeviceByName () =
        match cacheIsEmpty () with
        | true ->
            match showAvailableDevices () with
            | true -> pairDeviceByName ()
            | false -> () // return
        | false ->
            printfn $"%s{insertDeviceMessage}"
            let input = Console.ReadLine()

            match existInCache input with
            | true ->
                printfn "pairing device"
                let device = getDeviceFromCache input

                let success =
                    pairController device.DeviceAddress "5555"

                match success with
                | false -> printfn $"something went wrong, couldn't connect %s{input}"
                | true ->
                    printfn $"%s{input} paired successfully"
                    removeFromCache input |> ignore
            | false ->
                match String.IsNullOrEmpty input with
                | true -> () // return
                | false ->
                    printfn $"%s{input} is no valid controller"
                    pairDeviceByName ()

    let rec removeDeviceByName () =
        let pairedDevices = getAllPairedDevices ()

        match pairedDevices.Length with
        | 0 -> printfn "no device are paired!"
        | _ ->
            printfn $"%s{insertDeviceMessage}"
            let input = Console.ReadLine() //

            match String.IsNullOrEmpty input with
            | true -> () // return
            | false ->
                let selectedDevice =
                    pairedDevices
                    |> mapToDeviceData
                    |> List.tryFind (fun device -> device.DeviceKey = input)

                match selectedDevice.IsSome with
                | true ->
                    printfn $"removing device: %s{input}"

                    match selectedDevice.Value.DeviceData.DeviceAddress
                          |> removePairedController with
                    | true -> printfn $"device: %s{input} was removed"
                    | false -> printfn $"something went wrong, couldn't remove device: %s{input}"
                | false ->
                    printfn $"The device: %s{input} is not valid please select device that is paired"
                    removeDeviceByName ()

    let rec reconnectDevice () =
        let pairedDevices = getAllPairedDevices ()

        match pairedDevices.Length with
        | 0 -> printfn "no device are paired!"
        | _ ->
            printfn $"%s{insertDeviceMessage}"
            let input = Console.ReadLine()

            match String.IsNullOrEmpty input with
            | true -> () // return
            | false ->
                let selectedDevice =
                    pairedDevices
                    |> mapToDeviceData
                    |> List.tryFind (fun device -> device.DeviceKey = input)

                match selectedDevice.IsSome with
                | false ->
                    printfn $"The device: %s{input} is not valid please select device that is paired"
                    reconnectDevice ()
                | true ->
                    printfn $"reconnecting device: %s{input}"

                    match selectedDevice.Value.DeviceData.DeviceAddress
                          |> removePairedController with
                    | false -> printfn $"something went wrong, couldn't remove device: %s{input}"
                    | true ->
                        printfn $"device: %s{input} was removed"

                        let availableDevice =
                            getAllAvailableDevices ()
                            |> mapToDeviceData
                            |> List.tryFind (fun device -> device.DeviceKey = input)

                        match availableDevice.IsSome with
                        | false ->
                            printfn
                                $"couldn't find bluetooth signal of device: %s{input}, please make sure the device is active"
                        | true ->
                            printfn $"pairing device: %s{input}"

                            let success =
                                pairController availableDevice.Value.DeviceData.DeviceAddress "5555"

                            match success with
                            | false -> printfn $"couldn't connect device: %s{input}"
                            | true -> printfn $"%s{input} paired successfully"

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

        let onInputProcessFinished () =
            printfn "\nplease select a key. press h for help and press q to exist\n"
            checkForKeys (Console.ReadKey().Key)

        match key with
        | ConsoleKey.T ->
            showAvailableDevices () |> ignore
            onInputProcessFinished ()
        | ConsoleKey.Y ->
            showPairedDevices () |> ignore
            onInputProcessFinished ()
        | ConsoleKey.U ->
            pairDeviceByName ()
            onInputProcessFinished ()
        | ConsoleKey.I ->
            removeDeviceByName ()
            onInputProcessFinished ()
        | ConsoleKey.O ->
            reconnectDevice ()
            onInputProcessFinished ()
        | ConsoleKey.P ->
            connectAllControllers ()
            onInputProcessFinished ()
        | ConsoleKey.J ->
            removeAllControllers ()
            onInputProcessFinished ()
        | ConsoleKey.K ->
            reconnectAllControllers ()
            onInputProcessFinished ()
        | ConsoleKey.H ->
            showHelpText ()
            onInputProcessFinished ()
        | ConsoleKey.Q -> ()
        | _ ->
            printfn "\nwrong key pressed!"
            onInputProcessFinished ()

    printfn "please select a key. press h for help and press q to exist\n"
    checkForKeys (Console.ReadKey().Key)
    0
