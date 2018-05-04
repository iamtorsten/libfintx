# libfintx

[![Build Status](https://travis-ci.org/mrklintscher/libfintx.svg?branch=master)](https://travis-ci.org/mrklintscher/libfintx)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/928e912657d44a6090d329343aa13346)](https://www.codacy.com/app/torsten-klinger/libfintx?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=mrklintscher/libfintx&amp;utm_campaign=badger)
[![Issue Count](https://codeclimate.com/github/mrklintscher/libfintx/badges/issue_count.svg)](https://codeclimate.com/github/mrklintscher/libfintx)

An C# based client library for **HBCI 2.2** and **FinTS 3.0**.

In 1995 the ZKA announced a common online banking standard called *Homebanking Computer Interface* (HBCI). In 2003 they published the next generation of this protocol standard and named it *Financial Transaction Services* (FinTS).

Today most of all german banks support this online banking standards.

This client library supports both APIs, HBCI 2.2 and FinTS 3.0.

It can be used to read the balance of a bank account, receive an account statement, and make a SEPA payment using PIN/TAN.

# Usage

There are many reasons why you need to use a banking library which can exchange data from your application with the bank. One reason for example is to found a [Fintech](https://de.wikipedia.org/wiki/Finanztechnologie).

# Target platforms

* .NET Framework 4.5 and higher
* MONO (Windows, MacOS, Linux)
* Windows Phone 8
* WinRT
* Xamarin.Android
* Xamarin.iOS
* ASP.NET (Web)

# Sample

e.g. read balance

```cs
libfintx.Main.Assembly("Your build name", "Your build version");

public static AccountBalance Balance(ConnectionDetails connectionDetails, bool anonymous)

libfintx.Main.Balance ( ... );
```

# Features

* Get Balance (**HKSAL**)
* Request Transactions (**HKKAZ**)
* Transfer money (**HKCCS**)
* Transfer money at a certain time (**HKCCS**)
* Collective transfer money (**HKCCM**)
* Collective transfer money terminated (**HKCME**)
* Rebook money from one to another account (**HKCUM**)
* Collect money (**HKDSE**)
* Collective collect money (**HKDME**)
* Load mobile phone prepaid card (**HKPPD**)
* Submit banker's order (**HKCDE**)
* Get banker's orders (**HKCSB**)

# Specification

For exact information please refer to the [german version of the specification](http://www.hbci-zka.de/spec/spezifikation.htm). There is
also an [unauthorized english translation](http://www.hbci-zka.de/english/specification/engl_2_2.htm).

# SSL verification

The verification process is done by using the default [**WebRequest**](https://msdn.microsoft.com/de-de/library/system.net.webrequest(v=vs.110).aspx) class.

# Copyright & License

Copyright (c) 2016 - 2018 **Torsten Klinger**

Licensed under GNU Lesser General Public License. Please read the LICENSE file.
