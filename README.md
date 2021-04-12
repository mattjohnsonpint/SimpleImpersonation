SimpleImpersonation  [![NuGet Version](https://img.shields.io/nuget/v/SimpleImpersonation.svg?style=flat)](https://www.nuget.org/packages/SimpleImpersonation/) 
===================

Simple Impersonation Library for .Net

This library allows you to run code as another Windows user, as long as you have their credentials.

It achives this using the [LogonUser](http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx) Windows API, and thus can only provide the functionality provided by that API.

## Installation

```powershell
PM> Install-Package SimpleImpersonation
```

This library is multi-targeted, and should work well with all of the following:
  - .NET Framework 2.0, 3.5, 4.0, 4.5, 4.6, and greater
  - Any implementation of .NET Standard 2.0, including .NET Core 2.0 or greater

## Platform Support

Since this library relies on Windows APIs, it is Windows only.

(Linux and Mac support would be gladly accepted as pull requests from an ambitious contributor!)

## Usage

*Note this API is new for version 3.0.0 and varies significantly from previous versions.*

```csharp
var credentials = new UserCredentials(domain, username, password);
Impersonation.RunAsUser(credentials, logonType, () =>
{
    // do whatever you want as this user.
}); 
```

or

```csharp
var credentials = new UserCredentials(domain, username, password);
var result = Impersonation.RunAsUser(credentials, logonType, () =>
{
    // do whatever you want as this user.
    return something;
}); 
```

A few notes:

- **Don't use impersonation with asynchronous code.  See [#32](https://github.com/mj1856/SimpleImpersonation/issues/32) for details about why.**

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

- If you need access to the handle of the user being impersonated, you can gain access to it as an argument to the action or function delegate.  Ex:  `(tokenHandle) => { ... }`

Testing
-------

In order to verify that this library can impersonate a user, the unit tests will create a temporary user account on the local computer,
and then delete the account when the test run is complete.  To achieve this, the tests must be run as an elevated "administrator" account.

You can "run as administrator" on a command prompt window and run `dotnet test` on the test project, or you can launch Visual Studio as an administrator and execute the tests from there.

Changelog
---------

1.0.0

 - Initial Version

1.0.1

 - Issue #2 - Fixes possible "SafeHandle cannot be null"

1.1.0

 - Issue #9 - Adds support for passing the password as a `SecureString`

2.0.0

- Issue #14 - Throws a more useful exception.  (This is a breaking change if you were previously parsing the error code out of the message string).

2.0.1

- Issue #17 - Adds the `NativeError` code to the exception.

3.0.0

- Major changes to the API.  The `LogonUser` method and `IDisposable` pattern are deprecated, in favor of `RunAsUser` that takes an action or function delegate.
- Uses the built-in `WindowsIdentity.RunImpersonated` and `SafeAccessTokenHandle` APIs in .NET Framework 4.6+ and where available.
- .NET Standard 2.0 support

3.1.0
- Add 'LowLevelImpersonation' for more fine-grained control over 'SafeAccessTokenHandle'.
