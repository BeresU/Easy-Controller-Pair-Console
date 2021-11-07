open System
open BluetoothService
open BluetoothService
open Cache
open InTheHand.Net.Bluetooth
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

    let insertDeviceTypeMessage =
        "\nplease insert device type, empty type will take you back, insert \"all\" to connect all available devices"

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
                    printfn
                        $"\t* name: %s{device.DeviceKey}, address: %s{device.DeviceData.DeviceAddress.ToString()}, device type: %s{device.DeviceData.ClassOfDevice.Device.ToString()}")

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
                        $"\t* name: %s{device.DeviceKey}, connected: %b{device.DeviceData.Connected}, address: %s{device.DeviceData.DeviceAddress.ToString()}, device type: %s{device.DeviceData.ClassOfDevice.Device.ToString()}\n")

            true

    let pairDevice device =
        let success =
            pairDevice device.DeviceData.DeviceAddress

        match success with
        | false ->
            printfn $"something went wrong, couldn't connect %s{device.DeviceKey}"
            false
        | true ->
            printfn $"%s{device.DeviceKey} paired successfully"
            true

    let removePairedDevice device =
        printfn $"removing device: %s{device.DeviceKey}"

        match device.DeviceData.DeviceAddress
              |> removePairedDevice with
        | true ->
            printfn $"device: %s{device.DeviceKey} was removed"
            true
        | false ->
            printfn $"something went wrong, couldn't remove device: %s{device.DeviceKey}"
            false


    let rec pairDeviceByInput () =
        match cacheIsEmpty () with
        | true ->
            match showAvailableDevices () with
            | true -> pairDeviceByInput ()
            | false -> () // return
        | false ->
            printfn $"%s{insertDeviceMessage}"
            let input = Console.ReadLine()

            match existInCache input with
            | true ->
                printfn "pairing device"
                let device = getDeviceFromCache input
                pairDevice device |> ignore
            | false ->
                match String.IsNullOrEmpty input with
                | true -> () // return
                | false ->
                    printfn $"%s{input} is no valid controller"
                    pairDeviceByInput ()

    let rec removeDeviceByInput () =
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
                | true -> removePairedDevice selectedDevice.Value |> ignore
                | false ->
                    printfn $"The device: %s{input} is not valid please select device that is paired"
                    removeDeviceByInput ()

    let rec connectAllDevicesByInput () =
        printfn $"%s{insertDeviceTypeMessage}"

        let input = Console.ReadLine()

        let connectDevices (devices: Device list) =
            printfn "available devices:"

            devices
            |> List.iter
                (fun device ->
                    printfn $"device name: {device.DeviceKey}, address: %s{device.DeviceData.DeviceAddress.ToString()}")

            devices
            |> List.iter (fun device -> pairDevice device |> ignore)

        match input with
        | "" -> () // return
        | "all" ->
            // TODO: get from cache
            printfn "connecting all available devices!"

            let devices =
                match cache.Count with
                | 0 -> getAllAvailableDevices () |> mapToDeviceData
                | _ -> cacheList ()


            match devices.Length with
            | 0 -> printfn "there are no available devices!"
            | _ -> connectDevices devices

        | _ ->
            match Enum.TryParse<DeviceClass>(input) with
            | false, _ ->
                printfn $"the type: %s{input} is invalid"
                connectAllDevicesByInput ()
            | true, deviceType ->

                let devices =
                    match cache.Count with
                    | 0 ->
                        getAllAvailableDevices ()
                        |> List.filter (fun device -> device.ClassOfDevice.Device = deviceType)
                        |> mapToDeviceData
                    | _ ->
                        cacheList ()
                        |> List.filter (fun device -> device.DeviceData.ClassOfDevice.Device = deviceType)

                match devices.Length with
                | 0 ->
                    printfn $"there are no devices of type: %s{input}"
                    connectAllDevicesByInput ()
                | _ -> connectDevices devices



    let rec removeAllControllersByInput () =
        printfn $"%s{insertDeviceTypeMessage}"

        let input = Console.ReadLine()

        let removeDevices (devices: Device list) =
            printfn "removing devices:"

            devices
            |> List.iter (fun device -> printfn $"device name: {device.DeviceKey}")

            devices
            |> List.iter (fun device -> removePairedDevice device |> ignore)

        match input with
        | "" -> () // return
        | "all" ->
            // TODO: get from cache
            printfn "removing all paired devices!"

            let devices =
                getAllPairedDevices () |> mapToDeviceData

            match devices.Length with
            | 0 -> printfn "there are no paired devices!"
            | _ -> removeDevices devices

        | _ ->
            match Enum.TryParse<DeviceClass>(input) with
            | false, _ ->
                printfn $"the type: %s{input} is invalid"
                removeAllControllersByInput ()
            | true, deviceType ->
                let devices =
                    getAllPairedDevices ()
                    |> List.filter (fun device -> device.ClassOfDevice.Device = deviceType)
                    |> mapToDeviceData

                match devices.Length with
                | 0 ->
                    printfn $"there are no devices of type: %s{input} that are paired!"
                    removeAllControllersByInput ()
                | _ -> removeDevices devices



    // TODO: load from file (for practice)
    let showHelpText () =
        printfn
            "
        press t/T to show available controllers to pair
        press y/Y to show paired controllers
        press u/U to pair a controller
        press i/I to remove a controller
        press p/P to connect all controllers that are available
        press j/J to remove all the controllers that are paired"

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
            pairDeviceByInput ()
            onInputProcessFinished ()
        | ConsoleKey.I ->
            removeDeviceByInput ()
            onInputProcessFinished ()
        | ConsoleKey.P ->
            connectAllDevicesByInput ()
            onInputProcessFinished ()
        | ConsoleKey.J ->
            removeAllControllersByInput ()
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
