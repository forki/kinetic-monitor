namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("KineticMonitor")>]
[<assembly: AssemblyProductAttribute("KineticMonitor")>]
[<assembly: AssemblyDescriptionAttribute("A monitor service for Kinetic devices")>]
[<assembly: AssemblyVersionAttribute("0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.1"
