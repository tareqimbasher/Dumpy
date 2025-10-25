#!/usr/bin/env bash

dotnet clean -c Release

dotnet pack src/Dumpy.Abstractions/Dumpy.Abstractions.csproj -c Release -o ./nupkgs \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true

dotnet pack src/Dumpy.Console/Dumpy.Console.csproj -c Release -o ./nupkgs \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true

dotnet pack src/Dumpy.Html/Dumpy.Html.csproj -c Release -o ./nupkgs \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true