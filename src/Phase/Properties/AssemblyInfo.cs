using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Phase")]
[assembly: AssemblyDescription("Phase is an in-process, platform agnostic, tenant isolating, CQRS and Event Sourcing for stateful services. Phase operates against in-memomry state as its primary source of data for both writes and reads. It then provides an abstraction for persisting the events to a durable store. This allows the entire framework to be executed end-to-end, in-proc from any platform. It also allows it to serve both commands and queries in sub-millisecond times")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Charles Zipp")]
[assembly: AssemblyProduct("Phase")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("232c5fe9-37b3-45f0-9881-5c1540b02889")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("0.1.0")]
