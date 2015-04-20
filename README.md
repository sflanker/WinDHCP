# WinDHCP
A Windows DHCP Server Written in C#

## Overview

WinDHCP is windows service written in C#. It provides the basic DHCP functionality necessary to assign IP addresses on your LAN w/ subnet, gateway, and dns information. Currently it only processes DHCP Discover and Request messages, all others are ignored. WinDHCP was written using Visual Studio 2008 Express and has only been compiled and tested for .Net 3.5 on Windows Vista, although it should be possible to compile for .Net 2.0 and 3.0 as well, and should run in any environment that supports the .Net framework. For more information on what DHCP is see http://en.wikipedia.org/wiki/DHCP.

## Motivation

Many of the cheap commodity routers currently available have very poor DHCP implementations. The freely available DHCP offerings on the internet primarily target Linux/Unix environments (ISC DHCP, for example, will not compile on Windows). WinDHCP makes it possible and easy to set up any Windows machine as a DHCP server.
