#!/usr/bin/env bash

dotnet clean -c Release
dotnet build -c Release

dotnet pack src/Dumpy/Dumpy.csproj -c Release -o ./nupkgs --no-build \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true

dotnet pack src/Dumpy.Console/Dumpy.Console.csproj -c Release -o ./nupkgs --no-build \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true

dotnet pack src/Dumpy.Html/Dumpy.Html.csproj -c Release -o ./nupkgs --no-build \
  -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg \
  -p:ContinuousIntegrationBuild=true