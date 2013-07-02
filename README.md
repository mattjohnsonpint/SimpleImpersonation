SimpleImpersonation
===================

Simple Impersonation Library for .Net

This library provides a managed wrapper for the [LogonUser](http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx) function of the Win32 API.  Basically, it allows you to impersonate any user, as long as you have their credentials.

It is dual compiled for both .Net 2.0 and .Net 4.0 runtimes.  It should work well under .Net 2.0, 3.0, 3.5, 4.0, 4.5, and the client profiles.  It has no dependencies.

Installation
------------

Use NuGet package [SimpleImpersonation](https://nuget.org/packages/SimpleImpersonation/).

    PM> Install-Package SimpleImpersonation


Usage
-----

    using (Impersonation.LogonUser(domain, username, password, logonType))
    {
        // do whatever you want as this user.
    }

Be sure to specify a logon type that makes sense for what you are doing.  For example:

- If you are trying to connect to a SQL server with trusted authentication using specific credentials, use `LogonType.NewCredentials`.

- If you are interactively working as a particular user from a desktop application, use `LogonType.Interactive`.

See the [MSDN documentation](http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx) for additional logon types.
