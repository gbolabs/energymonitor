# Introduction

This page describes the process to read, using an application, periodcally the value of the Electric Power Meter.

## Principle

1. Configure the serial port
2. Submit the magic communication initialization message
   ```echo -n -e '\x06\x30\x30\x30\x0D\x0A' > /dev/ttyUSB0 # <ACK>000<CR><LF>```
2. Read the first-`350` block from the `dev/ttyUSB0` interface.
3. Process the read content to extract the consumed `kWh` for all the three phases.
4. Process the read content to extract the current consumed `A` for each phases.
5. Post the values or exposes the value to/for _Jeedom_.

## Implementation

### Python

Source: _<https://riptutorial.com/python/example/20311/read-from-serial-port>_

#### Setting-up the serial-port

[Configuration of serial port using bash](serial-port-config.md)

We need:
- 300bauds
- 7N1, `7` Data bit, `N` no parity, `1` stop bit

```python
import serial

# configure the serial connections (the parameters differs on the device you are connecting to)
ser = serial.Serial(
    port='/dev/ttyUSB0',
    baudrate=300,
    parity=serial.PARITY_EVEN,
    stopbits=serial.STOPBITS_ONE,
    bytesize=serial.SEVENBITS,
    timeout=30,
    xonxoff=False,
    rtscts=False,
)
```

#### Write the initialisation sequence

```bash
# This is an OBIS Init comend, and it needs a slight delay durng this phase, i added 1s
echo -n -e '\x2F\x3F\x21\x0D\x0A' > /dev/ttyUSB0 # /?!<CRL><LF>
sleep 1s
echo -n -e '\x06\x30\x30\x30\x0D\x0A' > /dev/ttyUSB0 # <ACK>000<CR><LF>
```

##### Parameters

_Source: <https://www.man7.org/linux/man-pages/man1/echo.1.html>_

- `-n`: Do not output a trailing new-line
- `-e`: Enables interpretation of `\`-characters as escape<br/>
 implies content as `\x2F` will be writen as an hexadecimal value.

In python:

```python
if ser.isOpen():
    try:
        ser.flushInput() #flush input buffer, discarding all its contents
        ser.flushOutput()#flush output buffer, aborting current output 
                 #and discard all that is in buffer

        #write data
        command = b"\x2F\x3F\x21\x0D\x0A" # /?!<CRL><LF>
        ser.write(command)
        time.sleep(1)
        command = b"\x06\x30\x30\x30\x0D\x0A" # <ACK>000<CR><LF>
        ser.write(command)
```


#### Exception handling

```python
try: 
    ser.open()
except Exception, e:
    print "error open serial port: " + str(e)
    exit()
```