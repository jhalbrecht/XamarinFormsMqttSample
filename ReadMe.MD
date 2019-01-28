# Mqtt Sample

## Why?
I'm using this repository to learn how to secure my Xamarin.Forms applications with for example a Let's Encrypt cerfificate.

## Goal
Learn best practices / real world way(s) to install and use certificates in mobile apps supported by Xamarin.Forms

## Resources

### Mohaqeq M2Mqtt Fork
The Mqtt client. Thanks Mohaqeq!
The Eclipse.org paho M2Mqtt hasn't been worked on in a while..
From mohaqeq on GitHub and converted to .NET Standard 2.0 library
https://github.com/mohaqeq/paho.mqtt.m2mqtt

## Testing

### Mosquitto_[pub|sub]

#### port 1883
``mosquitto_pub -h 192.168.1.222  -t test -m "`date` Howdy It works!"``
#### port 8883 TLS
``mosquitto_pub -h redacted.org -t test -m "`date` 0hello again" -p 8883 --capath /etc/ssl/certs/``

## Gotchas
### Unlikley 
- If you configure DNS nameserver to return a hostname on a NATed ip address this code will bomb.
