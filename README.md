SimpleImpersonation  [![NuGet Version](https://img.shields.io/nuget/v/SimpleImpersonation.svg?style=flat)](https://www.nuget.org/packages/SimpleImpersonation/) 
===================

Simple Impersonation Library for .Net

This library allows you to run code as another Windows user, as long as you have their credentials.

It achives this using the [LogonUser](http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx) Windows API, and thus can only provide the functionality provided by that API.

## Installation

```powershell
PM> Install-Package SimpleImpersonation
```

As of version 4.0.0, this library targets .NET Standard 2.0 only.  It should work well with all of the following:
  - .NET 5 or greater
  - .NET Core 2.0 or greater
  - .NET Framework 4.6.1 and greater

Note that .NET Framework versions less than 4.6.1 are no longer supported.

## Platform Support

Since this library relies on Windows APIs, it is supported on Windows only.

## Usage

As of version 4.0.0, the prefered approach is to get a `SafeAccessTokenHandle` for the credentials by calling `LogonUser` from a `UserCredentials` instance.

You'll first want to import these namespaces:
```csharp
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
```

Then you can get a handle for the user using this library.
```csharp
UserCredentials credentials = new UserCredentials(domain, username, password);
using SafeAccessTokenHandle userHandle = credentials.LogonUser(LogonType.Interactive);  // or another LogonType
```

You can then use that handle with built-in .NET functions such
as [`WindowsIdentity.RunImpersonated`](https://docs.microsoft.com/dotnet/api/system.security.principal.windowsidentity.runimpersonated) or [`WindowsIdentity.RunImpersonatedAsync`](https://docs.microsoft.com/dotnet/api/system.security.principal.windowsidentity.runimpersonatedasync). 

```csharp
WindowsIdentity.RunImpersonated(userHandle, () => {
    // do whatever you want as this user.
});
```
or

```csharp
var someResult = WindowsIdentity.RunImpersonated(userHandle, () => {
    // do whatever you want as this user.
    return someResult;
});
```
or

```csharp
await WindowsIdentity.RunImpersonatedAsync (userHandle, async () => {
    // do whatever you want as this user.
});
```
or

```csharp
var someResult = await WindowsIdentity.RunImpersonatedAsync(userHandle, async () => {
    // do whatever you want as this user.
    return someResult;
});
```

## Usage Notes

- The previous `Impersonation.RunAsUser` method has been deprecated.  Instead, please obtain a user handle and use the built-in functions as shown above.

- The `domain` parameter can optionally be omitted, in which case the `username` can contain the domain in either `domain\user` or `user@domain` format.

- For local computer users, you can either pass the computer's machine name or `.` to the `domain` parameter, or omit the `domain` parameter and just pass the `username` by itself.

- The `password` parameter can be specified as a `SecureString` or a regular `string`.  `SecureString` is recommended when the password is being typed in by a user, but is not appropriate if you already have the password as a regular `string`.

- Be sure to specify a logon type that makes sense for what you are doing.  For example:

  - If you are interactively working as a particular user from a desktop application, use `LogonType.Interactive`.

  - If you are trying to connect to a SQL server with trusted authentication using specific credentials, use `LogonType.NewCredentials`.
    - But be aware that impersonation is not taken into account in connection pooling.
    - You will also need to vary your connection string.
    - Read more [here](http://stackoverflow.com/q/18198291/634824)

  See the [MSDN documentation](http://msdn.microsoft.com/library/windows/desktop/aa378184.aspx) for additional logon types.


- If impersonation fails, it will throw a custom `ImpersonationException`, which has the following properties:
  - `Message` : The string message describing the error.  
  - `NativeErrorCode` : The native Windows error code, as described [here](https://msdn.microsoft.com/en-us/library/windows/desktop/ms681381.aspx).
  - `ErrorCode` : The `HResult` of the error.
  - `InnerException` : A `Win32Exception` used to derive the other properties.

Testing
-------

In order to verify that this library can impersonate a user, the unit tests will create a temporary user account on the local computer,
and then delete the account when the test run is complete.  To achieve this, the tests must be run as an elevated "administrator" account.

You can "run as administrator" on a command prompt window and run `dotnet test` on the test project, or you can launch Visual Studio as an administrator and execute the tests from there.
