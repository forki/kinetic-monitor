namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("KineticMonitor")>]
[<assembly: AssemblyProductAttribute("KineticMonitor")>]
[<assembly: AssemblyDescriptionAttribute("A monitor service for Kinetic devices")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
