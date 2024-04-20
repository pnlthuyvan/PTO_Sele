using System.Reflection;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PTO")]
[assembly: AssemblyDescription("Automation for PTO")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Simpson Strong-tie")]
[assembly: AssemblyProduct("PTO")]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("f518a335-dd41-41b7-9a79-f712c251d9b4")]

// Define attribute
[assembly: Parallelizable(ParallelScope.Children)]


// Number browser
[assembly: LevelOfParallelism(2)]