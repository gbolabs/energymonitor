import time
import serial
from processdata import parse_powermeter_data
from azureclient import post_to_azure_read_info
from storerawdata import writeTofile

READ_BYTES = 375
READ_TIMEOUT_SEC = 15

# Serial takes two parameters: serial device and baudrate
ser = serial.Serial(
    port='/dev/ttyUSB0',
    baudrate=300,
    parity=serial.PARITY_EVEN,
    stopbits=serial.STOPBITS_ONE,
    bytesize=serial.SEVENBITS,
    timeout=READ_TIMEOUT_SEC,
    xonxoff=False,
    rtscts=False,
)

if (not ser.isOpen()):
    ser.open()
    print("Serial port open")

if (ser.isOpen()):
    print("Serial port open, writing...")
    command = b"\x2F\x3F\x21\x0D\x0A"  # /?!<CRL><LF>
    print("/?!<CRL><LF> : "+command)
    ser.write(command)
    time.sleep(1)
    command = b"\x06\x30\x30\x30\x0D\x0A"  # <ACK>000<CR><LF>
    print("<ACK>000<CR><LF> : "+command)
    ser.write(command)

    print("Wait to 1s in order to let the serialport completes the write")

    time.sleep(1)

    print("Read {} block from serial".format(READ_BYTES))
    start = time.time()
    data = ser.read(READ_BYTES)
    delta=time.time()-start
    print("Reading "+str(data.__len__())+" took "+str(delta))

    ser.close()
    print("Serial port closed")

# Stores raw data for troubleshooting
writeTofile(data)

paresdData = parse_powermeter_data(data)

post_to_azure_read_info(paresdData)