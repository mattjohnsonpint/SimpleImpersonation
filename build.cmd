msbuild SimpleImpersonation.sln /t:Build /p:Configuration=Release
nuget pack .\SimpleImpersonation\SimpleImpersonation.csproj -Prop Configuration=Release -Symbols