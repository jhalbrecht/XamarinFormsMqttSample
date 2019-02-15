
'''
Revisions February 12, 2019
Created on Dec 3, 2015

@author: redacted

Python 2.7.13 (default, Sep 26 2018, 18:42:22)

pip freeze | grep mqtt
paho-mqtt==1.4.0

Given this mosquitto version 1.4.10 configuration

log_type all
listener 1883

listener 8883
tls_version tlsv1.2
cafile /etc/mosquitto/ca_certificates/ca.crt
certfile /etc/mosquitto/certs/redacted.redacted.org.crt
keyfile /etc/mosquitto/certs/redacted.redacted.org.key
require_certificate true
use_identity_as_username true

This works;

mosquitto_sub -t \$SYS/broker/bytes/\# -v 
    --cafile ./ca.crt 
    --cert ./consoleclient.crt 
    --key ./consoleclient.key 
    -p 8883

    mosquitto_pub --cafile ./ca.crt  -h redacted.org -t "xamtest" -m "`date` A message sent on port 8883" -p 8883 -d  --cert ./consoleclient.crt --key ./consoleclient.key -d --insecure


'''

import paho.mqtt.client as mqtt
import os
import ssl

publishHost = "redacted.org"
mqttPort = 8883

def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))
    client.subscribe("#")
    
def on_message(client, userdata, msg):
    print(msg.payload)

# dataDir = './'     # more convenient When I'm on linux... 
dataDir     = 'C:\TLS\redacted\selfsigned' # windows; Duh! ;-)
caCrt       = os.path.join(dataDir, 'ca.crt')
clientCrt   = os.path.join(dataDir, 'consoleclient.crt')
clientKey   = os.path.join(dataDir, 'consoleclient.key')

print("tls connection testing")
print(dataDir)
                                                     
client = mqtt.Client(
    client_id="mywikitest",
    protocol=mqtt.MQTTv311)

client.tls_set(ca_certs=caCrt,
               certfile=clientCrt,
               keyfile=clientKey,
               tls_version=ssl.PROTOCOL_TLSv1_2)

client.on_connect = on_connect
client.on_message = on_message

client.connect(publishHost, mqttPort, 60)
client.loop_forever()