# Mqtt Sample

## Why?
I'm using this repository to learn how to secure my Xamarin.Forms applications with for example a Let's Encrypt cerfificate.

## Goal
Learn best practices / real world way(s) to install and use certificates in mobile apps supported by Xamarin.Forms

## Resources

### Mohaqeq M2Mqtt Fork
The Mqtt client. Thanks Mohaqeq
From mohaqeq on GitHub and converted to .NET Standard 2.0 library
https://github.com/mohaqeq/paho.mqtt.m2mqtt

## Gotchas
### Unlikley 
- If you configure DNS nameserver to return a hostname on a NATed ip address this code will bomb.
