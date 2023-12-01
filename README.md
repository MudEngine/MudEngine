# Mud Engine

Mud Engine is a C# [MUD Game Engine](https://en.wikipedia.org/wiki/Multi-user_dungeon "MUD Game Engine") using Asp.Net Core 8 (C#12, Kestrel, Roslyn, gRPC) and SQL Server 2022. 

![Mud Engine Architecture](./Notes/architecture.png?raw=true "Mud Engine Architecture")

[Kestrel](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel "Kestrel") is a cross-platform web server for ASP.Net Core. 

Since version 2.1 of ASP.Net Core, it has been possible to configure Kestrel as a [generic TCP server](https://github.com/davidfowl/MultiProtocolAspNetCore/tree/master "generic TCP server") (due to [Project Bedrock](https://github.com/dotnet/aspnetcore/issues/4772 "Project Bedrock")) and make use of [high performance I/O pipelines](https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/ "high performance I/O pipelines"). Producer/consumer relationships are added (as necessary) with [Threading Channels](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/ "Threading Channels") to handle back pressure. 

Scalable inter-process communication (seperating the MUD itself from the Telnet/Command servers) is handled via [gRPC duplex streaming](https://learn.microsoft.com/en-us/aspnet/core/grpc/interprocess "gRPC duplex streaming").

The Command Server accesses data stored in SQL Server 2022+, thus making use of [Parameter Sensitive Plan Optimization](https://www.sqlservercentral.com/articles/exploring-parameter-sensitive-plan-optimization-in-sql-server-2022 "Parameter Sensitive Plan Optimization").

Commands can be compiled using the [Roslyn compiler](https://github.com/dotnet/roslyn "Roslyn compiler"), and executed as [collectible assemblies](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/collectible-assemblies "collectible assemblies") in a [secured AssemblyLoadContext](https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext "Secured AssemblyLoadContext") during runtime.

Supported protocols (so far) include [GMCP and MSSP](https://wiki.mudlet.org/w/Manual:Supported_Protocols#Encoding_in_Mudlet "GMCP and MSSP"), and have been tested against [Mudlet](https://www.mudlet.org/ "Mudlet") and [zMud](https://www.zuggsoft.com/ "zMUD").

The Telnet Server accepts UTF-8 I/O, but the input stream is Folded into Ascii-7 at the Hub Server using FormD -> FormC Normalization via [Runes](https://learn.microsoft.com/en-us/dotnet/api/system.memoryextensions.enumeraterunes "Runes") - a more nuanced version of grapheme clustering similar to Lucene's [AsciiFoldingFilter](https://gist.githubusercontent.com/andyraddatz/e6a396fb91856174d4e3f1bf2e10951c/raw/c241ed4eabb53b3f4cd2c4f594dc90f1518db1e7/ASCIIStringExtensions.cs "AsciiFoldingFilter").