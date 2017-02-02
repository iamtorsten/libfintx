# libfintx

[![Build Status](https://travis-ci.org/mrklintscher/libfintx.svg?branch=master)](https://travis-ci.org/mrklintscher/libfintx)
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

# Sample

e.g. read balance

```cs
public static string Balance(int Account, int BLZ, string IBAN, 
                             string BIC, string URL, int HBCIVersion, int UserID, string PIN)

libfintx.Main.Balance ( ... );
```

# Features

* Get Balance (**HKSAL**)
* Request Transactions (**HKKAZ**)
* Transfer money (**HKCCS**)
* Transfer money at a certain time (**HKCCS**)
* Collective transfer money (**HKCCM**)
* Collective transfer money terminated (**HKCME**)
* Collect money (**HKDSE**)
* Collective collect money (**HKDME**)
* Load mobile phone prepaid card (**HKPPD**)
* Submit banker's order (**HKCDE**)
* Get banker's orders (**HKCSB**)

# Documentation

```xml
<?xml version="1.0"?>
<doc>
    <assembly>
        <name>libfintx</name>
    </assembly>
    <members>
        <member name="M:libfintx.Main.Synchronization(System.Int32,System.String,System.Int32,System.Int32,System.String)">
            <summary>
            Synchronize bank connection
            </summary>
            <param name="BLZ"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <returns>
            Success or failure
            </returns>
        </member>
        <member name="M:libfintx.Main.Balance(System.Int32,System.Int32,System.String,System.String,System.String,System.Int32,System.Int32,System.String)">
            <summary>
            Account balance
            </summary>
            <param name="Account"></param>
            <param name="BLZ"></param>
            <param name="IBAN"></param>
            <param name="BIC"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <returns>
            Balance
            </returns>
        </member>
        <member name="M:libfintx.Main.Transactions(System.Int32,System.Int32,System.String,System.String,System.String,System.Int32,System.Int32,System.String)">
            <summary>
            Account transactions
            </summary>
            <param name="Account"></param>
            <param name="BLZ"></param>
            <param name="IBAN"></param>
            <param name="BIC"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <returns>
            Transactions
            </returns>
        </member>
        <member name="M:libfintx.Main.Transfer(System.Int32,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Transfer money
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="Receiver"></param>
            <param name="ReceiverIBAN"></param>
            <param name="ReceiverBIC"></param>
            <param name="Amount"></param>
            <param name="Purpose"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.Transfer_Terminated(System.Int32,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Transfer money at a certain time
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="Receiver"></param>
            <param name="ReceiverIBAN"></param>
            <param name="ReceiverBIC"></param>
            <param name="Amount"></param>
            <param name="Purpose"></param>
            <param name="ExecutionDay"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.CollectiveTransfer(System.Int32,System.String,System.String,System.String,System.Collections.Generic.List{libfintx.pain00100203_ct_data},System.String,System.Decimal,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Collective transfer money
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="PainData"></param>
            <param name="NumberofTransactions"></param>
            <param name="TotalAmount"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.CollectiveTransfer_Terminated(System.Int32,System.String,System.String,System.String,System.Collections.Generic.List{libfintx.pain00100203_ct_data},System.String,System.Decimal,System.String,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Collective transfer money terminated
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="PainData"></param>
            <param name="NumberofTransactions"></param>
            <param name="TotalAmount"></param>
            <param name="ExecutionDay"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.Collect(System.Int32,System.String,System.String,System.String,System.String,System.String,System.String,System.Decimal,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Collect money from another account
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="Payer"></param>
            <param name="PayerIBAN"></param>
            <param name="PayerBIC"></param>
            <param name="Amount"></param>
            <param name="Purpose"></param>
            <param name="SettlementDate"></param>
            <param name="MandateNumber"></param>
            <param name="MandateDate"></param>
            <param name="CeditorIDNumber"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.CollectiveCollect(System.Int32,System.String,System.String,System.String,System.String,System.Collections.Generic.List{libfintx.pain00800202_cc_data},System.String,System.Decimal,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Collective collect money from other accounts
            </summary>
            <param name="BLZ"></param>
            <param name="AccountHolder"></param>
            <param name="AccountHolderIBAN"></param>
            <param name="AccountHolderBIC"></param>
            <param name="SettlementDate"></param>
            <param name="PainData"></param>
            <param name="NumberofTransactions"></param>
            <param name="TotalAmount"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.Prepaid(System.Int32,System.String,System.String,System.Int32,System.String,System.Int32,System.String,System.Int32,System.Int32,System.String,System.String,System.Windows.Forms.PictureBox)">
            <summary>
            Load mobile phone prepaid card
            </summary>
            <param name="BLZ"></param>
            <param name="IBAN"></param>
            <param name="BIC"></param>
            <param name="MobileServiceProvider"></param>
            <param name="PhoneNumber"></param>
            <param name="Amount"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="HIRMS"></param>
            <param name="pictureBox"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.BankersOrders(System.Int32,System.String,System.String,System.String,System.Int32,System.Int32,System.String)">
            <summary>
            Get banker's orders
            </summary>
            <param name="BLZ"></param>
            <param name="IBAN"></param>
            <param name="BIC"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <returns>
            Banker's orders
            </returns>
        </member>
        <member name="M:libfintx.Main.TAN(System.String,System.String,System.Int32,System.Int32,System.Int32,System.String)">
            <summary>
            Confirm order with TAN
            </summary>
            <param name="TAN"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="BLZ"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.TAN4(System.String,System.String,System.Int32,System.Int32,System.Int32,System.String,System.String)">
            <summary>
            Confirm order with TAN
            </summary>
            <param name="TAN"></param>
            <param name="URL"></param>
            <param name="HBCIVersion"></param>
            <param name="BLZ"></param>
            <param name="UserID"></param>
            <param name="PIN"></param>
            <param name="MediumName"></param>
            <returns>
            Bank return codes
            </returns>
        </member>
        <member name="M:libfintx.Main.TAN_Scheme">
            <summary>
            TAN scheme
            </summary>
            <returns>
            TAN mechanism
            </returns>
        </member>
        <member name="M:libfintx.Main.Assembly(System.String,System.String)">
            <summary>
            Set assembly informations
            </summary>
            <param name="Buildname"></param>
            <param name="Version"></param>
        </member>
        <member name="M:libfintx.Main.Buildname">
            <summary>
            Get assembly buildname
            </summary>
            <returns>
            Buildname
            </returns>
        </member>
        <member name="M:libfintx.Main.Version">
            <summary>
            Get assembly version
            </summary>
            <returns>
            Version
            </returns>
        </member>
        <member name="M:libfintx.Main.Transaction_Output">
            <summary>
            Transactions output console
            </summary>
            <returns>
            Bank return codes
            </returns>
        </member>
    </members>
</doc>
```

# Specification

For exact information please refer to the [german version of the specification](http://www.hbci-zka.de/spec/spezifikation.htm). There is
also an [unauthorized english translation](http://www.hbci-zka.de/english/specification/engl_2_2.htm).

# SSL verification

The verification process is done by using the default [**WebRequest**](https://msdn.microsoft.com/de-de/library/system.net.webrequest(v=vs.110).aspx) class.

# Copyright & License

Copyright (c) 2016 - 2017 [Torsten Klinger](http://www.mrklintscher.com)

Licensed under GNU Lesser General Public License. Please read the LICENSE file.
