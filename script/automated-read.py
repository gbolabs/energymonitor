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
    # ser.write("\x06\x30\x30\x30\x0D\x0A")
    time.sleep(1)

    print("Read 350 block from serial")
    data = ser.read(350)
    print(data)

    ser.close()
    print("Serial port closed")