namespace BluetoothService

open System.Collections.Generic
open InTheHand.Net.Sockets

module Cache =
    type Device =
        { DeviceKey: string
          DeviceData: BluetoothDeviceInfo }

    let cache =
        Dictionary<string, BluetoothDeviceInfo>()

    let existInCache deviceKey = cache.ContainsKey deviceKey

    // TODO: see if can handle errors
    let getAvailableDevice deviceKey =
        match cache.TryGetValue deviceKey with
        | true, v -> v
        | false, _ -> null
        
    let removeFromCache deviceKey = cache.Remove deviceKey
        
    let saveToCache (devices: Device list) =
        cache.Clear()
        devices
        |> List.iter (fun device -> cache.Add(device.DeviceKey, device.DeviceData))
               
