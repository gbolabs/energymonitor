#!/bin/bash
#
PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
# Set the tty interface speed to 300 Baud and 7N1 - i did not try higher values
stty -F /dev/ttyUSB0 1:0:9a7:0:3:1c:7f:15:4:5:1:0:11:13:1a:0:12:f:17:16:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0

# This is an OBIS Init comend, and it needs a slight delay durng this phase, i added 1s
echo -n -e '\x2F\x3F\x21\x0D\x0A' > /dev/ttyUSB0 # /?!<CRL><LF>
sleep 1s
echo -n -e '\x06\x30\x30\x30\x0D\x0A' > /dev/ttyUSB0 # <ACK>000<CR><LF>

# Get the Unix system time in MS (i just rounded up to the second)
TIME=$(date "+%s000")

# As the tty does not return an end of file, or end of stream byte, i will just read 64 bytes, so i'll cut the stream just after the T1 KWH value
VALUE=$(dd if=/dev/ttyUSB0 bs=1 count=128 2> /dev/null )

echo $VALUE

# Now i will extract the last line, and the numeric value of the 1.8.2 OBIS Tag, ex:
# 1.8.1(036884.085*kWh)
COUNTER=$(echo "$VALUE" | tail -1 | strings  | cut -d'(' -f2 | cut -d'*' -f1)
# 1.8.2(036884.085*kWh)
COUNTER=$(echo "$VALUE" | tail -1 | strings  | cut -d'(' -f2 | cut -d'*' -f1)

# In some rare cases, the reader returns garbage, so i check if i received exactly 11 bytes...
LEN=$(echo $COUNTER | wc -m)

# For debugging ....
echo "USAGE $POWER $TIME"
echo "COUNTER $COUNTER $TIME"
echo "STR LENGTH $LEN"

if [[ $LEN -eq 11 ]]
then
        # Now i have to send the result to the volkszaehler middleware, i have to get the UUID from the middleware, here set to 6f965c00-986d-11e6-a157-123456789012
#       wget -O - "http://volkszaehler.yourserverhere/middleware.php/data/6f965c00-986d-11e6-a157-123456789012.json?operation=add&ts=$TIME&value=$COUNTER"
fi
