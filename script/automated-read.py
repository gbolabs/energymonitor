import time
import serial
#Serial takes two parameters: serial device and baudrate
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

if (not ser.isOpen()):
    ser.open()
    print("Serial port open")

if (ser.isOpen()):
    print("Serial port open, writing...")
    command = b"\x2F\x3F\x21\x0D\x0A" # /?!<CRL><LF>
    print("/?!<CRL><LF> : "+command)
    ser.write(command)
    time.sleep(1)
    command = b"\x06\x30\x30\x30\x0D\x0A" # <ACK>000<CR><LF>
    print("<ACK>000<CR><LF> : "+command)
    ser.write(command)

    print("Wait to 500ms in order to let the portal completes the write")

    print(time.time)

    time.sleep(1)
    
    print(time.time)

    print("Read 350 block from serial")
    data = ser.read(350)
    
    print(time.time)
    print(data)

    ser.close()
    print("Serial port closed")