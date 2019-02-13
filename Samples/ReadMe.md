# That's crazy! Samples in a Sample

![](/images/SmallMoves.png)

I got the self signed certificates to work on mosquitto sub/put and a python example. .net you're next.

## Resources

### Self Signed certificates

* [SSL/TLS Client Certs to Secure MQTT](http://rockingdlabs.dunmire.org/exercises-experiments/ssl-client-certs-to-secure-mqtt) from rockingdlabs
* [generate-CA.sh](https://github.com/owntracks/tools/blob/master/TLS/generate-CA.sh) from @jpmens

## mosquitto pub sub

`mosquitto_sub -h redacted.org  -t \$SYS/broker/bytes/\# -v --cafile ./ca.crt --cert ./consoleclient.crt --key ./consoleclient.key  -p 8883`

`mosquitto_pub -h redacted.org --cafile ./ca.crt   -t "xamtest" -m "A message sent on port 8883" -p 8883 -d  --cert ./consoleclient.crt --key ./consoleclient.key -d --insecure`
## python

[PahoMqttSelfSigned.py](https://github.com/jhalbrecht/XamarinFormsMqttSample/blob/master/PahoMqttSelfSigned.py)



## .net core

Coming RSN.