#!/usr/bin/env bash

dotnet nuget push nupkgs/"*.nupkg" \
  --api-key ${NUGET_API_KEY} \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate

dotnet nuget push nupkgs/"*.snupkg" \
  --api-key ${NUGET_API_KEY} \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate