namespace BluetoothService

open InTheHand.Net
open InTheHand.Net.Bluetooth
open InTheHand.Net.Sockets

module BluetoothService =

    //TODO: convert this to async.
    let getAllAvailableDevices () =
        let client = new BluetoothClient()

        let devices =
            client.DiscoverDevices(50, false, false, true)

        devices |> List.ofArray


    let getAllPairedDevices () =
        let client = new BluetoothClient()

        let devices =
            client.DiscoverDevices(50, true, true, false)

        devices |> List.ofArray

    let pairAllControllers () = failwith "todo"

    let pairController (deviceAddress: BluetoothAddress, pin: string) =
        BluetoothSecurity.PairRequest(deviceAddress, pin)

    let reconnectAllControllers () = failwith "todo"
    let reconnectController (controllerName) = failwith "todo"
    let removeAllControllers () = failwith "todo"
    let removePairedController (controllerName) = failwith "todo"
