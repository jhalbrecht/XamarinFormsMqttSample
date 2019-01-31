# Mqtt Sample for Xamarin.forms

## App running
![](images/XamFormsMqttUwpDemo.JPG)


## Why?
I'm using this repository to learn how to secure my Xamarin.Forms applications with for example a Let's Encrypt cerfificate.

## Goal
Learn best practices / real world way(s) to install and use certificates in mobile apps supported by Xamarin.Forms

## Resources
### Reading
Help me find some great and current reading material. Maybe some videos? Again; **Looking for real world examples** of how to handle installing certificartes in 
the mobile apps UWP, Android and IOS. And then how to access them. **_Properly._**

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
``mosquitto_pub -h redacted.org -t test -m "`date` hello again" -p 8883 --capath /etc/ssl/certs/``

#### Public mqtt broker
##### test.mosquitto.org at [mosquitto.org](https://mosquitto.org/)

    [root@hosta ~]# mosquitto_sub -h test.mosquitto.org -t "xamtest"
    Mon Jan 28 06:49:50 MST 2019 Hello World of Xamarin Forms
    ^C
    [root@hosta ~]#

    root@hostb:~# mosquitto_pub -h test.mosquitto.org -t xamtest -m "`date` Hello World of Xamarin Forms"
    root@hostb:~#
##### TLS at [mosquitto.org](https://mosquitto.org/)

Demo coming **_real soon now_**

## Gotchas
### Unlikley 
- If you configure DNS nameserver to return a hostname on a NATed ip address this code will bomb.
