# This script triggers a read and display the result on screen
echo -n -e '\x2F\x3F\x21\x0D\x0A' > /dev/ttyUSB0
dd if=/dev/ttyUSB0 bs=1 of=response.txt