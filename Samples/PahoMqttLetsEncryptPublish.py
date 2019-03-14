# https://github.com/jhalbrecht/XamarinFormsMqttSample/tree/master/Samples
# Adapted from https://medium.com/@erinus/mosquitto-paho-mqtt-python-29cadb6f8f5c
# This .py is setup for my mosquitto broker secured by my https letsencrypt certificate
# Note; during testing I am not using usename and password credentials

import ssl
import sys

import paho.mqtt.client
import paho.mqtt.publish

host = 'redacted.org'
topic = 'xamtest'

def on_connect(client, userdata, flags, rc):
        print('connected')

def main():
        paho.mqtt.publish.single(
                topic=topic,
                payload='Testing an mqtt publish from python to a mosquitto broker secured by a letsencrypt certificate.',
                qos=2,
                hostname=host,
                port=8883,
                client_id='[clientid]',
                #auth={
                #       'username': '[username]',
                #       'password': '[password]'
                #},
                tls={
                        'ca_certs': '/etc/ssl/certs/DST_Root_CA_X3.pem',
                        'tls_version': ssl.PROTOCOL_TLSv1_2
                }
        )

if __name__ == '__main__':
        main()
        sys.exit(0)
