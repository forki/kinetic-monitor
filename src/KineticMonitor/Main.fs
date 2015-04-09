// Copyright (c) 2015 Seagate Technology

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// author: Ignacio Corderi

module Kinetic.Monitor.Main

open System.Net
open System.Net.Sockets
open Kinetic.Monitor

/// Time in seconds to mark a drive as offline 
let ACTIVE_TIMEOUT = 30 // seconds

// Network - Broadcast Listener
let localIPAddress = IPAddress.Parse("172.16.0.1")
let localEp = new IPEndPoint(IPAddress.Parse("239.1.2.3"), 8123) 
let socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)

socket.Bind(localEp)
 
let mcastOption = new MulticastOption(IPAddress.Parse("239.1.2.3"), localIPAddress)
socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mcastOption)
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true)
 
// State
let active = new System.Collections.Generic.Dictionary<string, Device>()

// Main
printfn "Listening this will never quit so you will need to ctrl-c it"

let remoteEP = new IPEndPoint(IPAddress.Any,0) :> EndPoint
let data: byte [] = Array.zeroCreate (64 * 1024)

while true do
    let bytesRead = socket.ReceiveFrom(data, ref remoteEP)
    let strData = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead)
    let info = KineticBroadcast.Parse(strData)

    let now = System.DateTime.Now

    // Check if device is new
    if not <| active.ContainsKey(info.WorldWideName) then
        // Add device to known devices
        printfn "Discovered new device { WWN= %s }" info.WorldWideName
        active.Add(info.WorldWideName, { Info = info; LastSeen = now }) |> ignore
    else
        let x = active.[info.WorldWideName]
        // Refresh last time seen
        // Use new info instead of old in case IP Address changed.
        active.[info.WorldWideName] <- { Info = info; LastSeen = now }
            