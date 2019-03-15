# https://github.com/jhalbrecht/XamarinFormsMqttSample/tree/master/Samples
# Adapted from https://medium.com/@erinus/mosquitto-paho-mqtt-python-29cadb6f8f5c
# This is setup for a mosquitto broker secured by CA Signed Server certificate. Perhaps letsencrypt 
# Note; during testing I am not using usename and password credentials

import ssl
import sys

import paho.mqtt.client
import paho.mqtt.publish

host = 'iot.eclipse.org'
topic = 'xamtest'

def on_connect(client, userdata, flags, rc):
        print('connected')

def main():
        paho.mqtt.publish.single(
                topic=topic,
                payload='Testing an mqtt publish from python to a mosquitto broker secured by a CA signed certificate.',
                qos=2,
                hostname=host,
                port=8883,
                client_id='[clientid]',
                #auth={
                #       'username': '[username]',
                #       'password': '[password]'
                #},
                tls={
                        # debian
                        #'ca_certs': '/etc/ssl/certs/DST_Root_CA_X3.pem',
                        # CentOS 7
                        'ca_certs': '/etc/ssl/certs/ca-bundle.trust.crt',
                        'tls_version': ssl.PROTOCOL_TLSv1_2
                }
        )

if __name__ == '__main__':
        main()
        sys.exit(0)
