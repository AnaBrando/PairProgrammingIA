#!/bin/bash
set -e

export PATH="$PATH:/root/.dotnet/tools"

echo "Installing dotnet-ef..."
dotnet tool install --global dotnet-ef

echo "Running EF Core migrations..."
dotnet ef database update

echo "Starting app..."
exec dotnet IAPairProgrammer.dll
