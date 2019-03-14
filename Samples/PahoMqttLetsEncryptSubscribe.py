# https://github.com/jhalbrecht/XamarinFormsMqttSample/tree/master/Samples 
# Adapted from https://medium.com/@erinus/mosquitto-paho-mqtt-python-29cadb6f8f5c
# This is setup for my mosquitto broker secured by my https letsencrypt certificate
# Note; during testing I am not using usename and password credentials

import ssl
import sys

import paho.mqtt.client
host = 'redacted.org'
topic = 'xamtest'

def on_connect(client, userdata, flags, rc):
        print('connected (%s)' % client._client_id)
        client.subscribe(topic=topic, qos=2)

def on_message(client, userdata, message):
        print('------------------------------')
        print('topic: %s' % message.topic)
        print('payload: %s' % message.payload)
        print('qos: %d' % message.qos)

def main():
        client = paho.mqtt.client.Client(client_id='[clientid]', clean_session=False)
        #client.username_pw_set('[username]', '[password]')
        client.on_connect = on_connect
        client.on_message = on_message
        # debian stretch
        client.tls_set('/etc/ssl/certs/DST_Root_CA_X3.pem', tls_version=ssl.PROTOCOL_TLSv1_2)
        # CentOS 7
        # client.tls_set('/etc/ssl/certs/ca-bundle.trust.crt', tls_version=ssl.PROTOCOL_TLSv1_2)
        client.connect(host=host, port=8883)
        client.loop_forever()

if __name__ == '__main__':
        main()
        sys.exit(0)
