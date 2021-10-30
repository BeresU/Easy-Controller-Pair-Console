namespace BluetoothService

open InTheHand.Net.Sockets

module BluetoothService =
    let getAllAvailableControllers () = failwith "todo"

    // TODO: get only controllers
    let getAllPairedDevices () =
        let client = new BluetoothClient()

        let devices =
            client.DiscoverDevices(50, true, true, false)

        devices
        |> List.ofArray
        |> List.map (fun device -> device.DeviceName)

    let pairAllControllers () = failwith "todo"
    let pairController (controllerName) = failwith "todo"
    let reconnectAllControllers () = failwith "todo"
    let reconnectController (controllerName) = failwith "todo"
    let removeAllControllers () = failwith "todo"
    let removePairedController (controllerName) = failwith "todo"
