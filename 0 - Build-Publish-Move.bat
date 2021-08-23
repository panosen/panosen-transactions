@echo off

dotnet restore

dotnet build --no-restore -c Release

move /Y Panosen.Transactions\bin\Release\Panosen.Transactions.*.nupkg D:\LocalSavoryNuget\

pause