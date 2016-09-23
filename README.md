## Synopsis

This library aims to extend [MEF (Managed Extensibility Framework)](https://msdn.microsoft.com/en-us/library/dd460648%28v=vs.110%29.aspx) with a new catalog that supports lazy loading of plugin assemblies while still making their metadata available.
This is a proof of concept and was not tested in production so far.

## Example usage

First we need to serialize the plugin assembly's part information. This should be done in advance - a good place to put this is probably the plugin's post-build event.
```
// Serialize assembly imports, exports, and metadata to file.
SerializationUtils.SerializeAssemblyPartInformation(typeof(MyPlugin).Assembly, @"C:\PartInformation.xml");
```

Now that we have the serialized part information, the application can create a `LazyAssemblyCatalog` from this data.
```
// Create the catalog and pass the serialized data
var catalog = new LazyAssemblyCatalog(@"C:\PartInformation.xml");
```

To avoid unnecessary assembly loading, imports should be wrapped in `Lazy` and use interfaces which are already loaded and available to the application, for example:
```
[ImportMany]
public Lazy<IPlugin, IPluginMetadata>[] Plugins { get; set; }

[Import]
public Lazy<IDependency> Dependency { get; set; }
```

A sample program can be found in the "Sample" folder.

## Motivation

The motivation behind writing this library is to improve the performance of applications which rely heavily on MEF plugins.
There are plenty of cases where an application loads all of its plugins, where in fact the user only actively uses a few of them. This results in wasteful memory usage and a performance hit to the application's startup, as all plugin assemblies have to be loaded.
Using the `LazyAssemblyCatalog` class allows the application to:

1. Start up quickly, as only necessary assemblies will be loaded on startup.
2. Keep the memory footprint to a minimum. Plugin assemblies will only be loaded when they actually need to be used.
3. Be able to access export metadata, even for plugins whose assemblies haven't been loaded yet.

## Installation

At the moment there is no NuGet package, so you'll have to compile the assembly and add it to your project yourself.

## Contribution

Feel free to make any contributions via pull requests.
