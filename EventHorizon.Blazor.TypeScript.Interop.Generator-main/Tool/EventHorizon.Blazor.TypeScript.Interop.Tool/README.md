[![EventHorizon.Blazor.TypeScript.Interop.Tool](https://img.shields.io/nuget/vpre/EventHorizon.Blazor.TypeScript.Interop.Tool?style=for-the-badge&label=Tool)](https://www.nuget.org/packages/EventHorizon.Blazor.TypeScript.Interop.Tool)

# About EventHorizon Blazor TypeScript Interop Tool

This dotnet tool encapsulates project generation for TypeScript Blazor Interop proxy abstraction. 

## Install

~~~ bash
dotnet tool install -g EventHorizon.Blazor.TypeScript.Interop.Tool
~~~

## Command Line Options

Identifier | Details | Required/Default
--- | --- | ---
-s, --source &lt;source&gt; | TypeScript Definition to generate from, can be a File or URL, accepts multiple or as Array |  REQUIRED
-c, --class-to-generate &lt;class-to-generate&gt; | A Class to generate, accepts multiple or as Array |  REQUIRED
-a, --project-assembly &lt;project-assembly&gt; | The project name of the Assembly that will be generated | Default: "Generated.WASM"
-l, --project-generation-location &lt;project-generation-location&gt; | The directory where the Generated Project assembly will be saved | Default: "_generated"
-f, --force | This will force generation, by deleting --project-generation-location | Default: (False)
-p, --parser | The type of TypeScript parser to use, Supported values: ("dotnet","nodejs")  | Default: ("dotnet")
-h, --host-type | The host type the source should be generator for, Supported values: ("wasm","server"). | Default: ("wasm") 

## Parsers 

The tool supports two types of parsers, one using the embedded .NET parser and one using NodeJS the TypeScript Complier. 

These both have trade offs:

Type | Details
--- | ---
dotnet | Has no external dependencies, and on average 3x faster than the nodejs parser. A con is that it does not support modern TypeScript syntax, but should coverage 90% of use-cases.
nodejs | Requires NodeJS to function, supports modern TypeScript syntax. A con is that it is very slow relative to the dotnet parser.

## Host Types

This tool supports two types of Hosting models, the standard Blazor Server and Wasm hosting models. The main reason for this is the Wasm generated source will create an interop layer that has a speed benefit but does not work outside of the Wasm hosting model.

Below is a table listing the pro/con of each hosting type.

Type | Details
--- | ---
wasm | Uses the unmarshaller to move data between the .NET and JavaScript layers. The con is that it only works in a Blazor WebAssembly hosted model, and will not function when used from Blazor Server.
server | Uses the IJSRuntime for moving data between the .NET and JavaScript layers. A pro is that it allows it to work in both Blazor Server and WebAssembly, but with a con that it has to go through the async layer adding a performance overhead to each request.

## Usage

~~~ bash
# Generated BabylonJS project with interop to Vector3 class.
# Created with a project assembly of Blazor.BabylonJS.
ehz-generate --project-assembly Blazor.BabylonJS --class-to-generate Vector3 --source https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/babylon.d.ts
~~~

~~~ bash
# (using shorthand alias) Generated BabylonJS project with interop to Vector3 class.
# Created with a project assembly of Blazor.BabylonJS.
ehz-generate -a Blazor.BabylonJS -c Vector3 -s https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/babylon.d.ts
~~~

~~~ bash
# Generated BabylonJS project interop with multiple classes.
ehz-generate -c Vector3 -c Mesh -c Engine -c Scene -s https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/babylon.d.ts
~~~

~~~ bash
# Generated BabylonJS project interop with multiple sources.
ehz-generate -c Button -s https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/babylon.d.ts -s https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/gui/babylon.gui.d.ts
~~~

~~~ bash
# Generate a BabylonJS project that work in both Blazor Server and Wasm projects
ehz-generate --host-type server -a Blazor.BabylonJS -c Vector3 -s https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/dist/babylon.d.ts

~~~
