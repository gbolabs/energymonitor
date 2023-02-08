# Introduction

This page describes the configuration of the serial port to read from the LandiGyr 230+.

## Original Configuration Command

`stty -F /dev/ttyUSB0 1:0:9a7:0:3:1c:7f:15:4:5:1:0:11:13:1a:0:12:f:17:16:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0`

## Boot configuration from Linux

```
speed 9600 baud; rows 0; columns 0; line = 0;
intr = ^C; quit = ^\; erase = ^?; kill = ^U; eof = ^D; eol = <undef>; eol2 = <undef>; swtch = <undef>; start = ^Q; stop = ^S; susp = ^Z; rprnt = ^R; werase = ^W; lnext = ^V; discard = ^O; min = 1; time = 0;
-parenb -parodd -cmspar cs8 hupcl -cstopb cread clocal -crtscts
-ignbrk -brkint -ignpar -parmrk -inpck -istrip -inlcr -igncr icrnl ixon -ixoff -iuclc -ixany -imaxbel -iutf8
opost -olcuc -ocrnl onlcr -onocr -onlret -ofill -ofdel nl0 cr0 tab0 bs0 vt0 ff0
isig icanon iexten echo echoe echok -echonl -noflsh -xcase -tostop -echoprt echoctl echoke -flusho -extproc
```

## Configuration after executing the above command

```
speed 300 baud; rows 0; columns 0; line = 0;
intr = ^C; quit = ^\; erase = ^?; kill = ^U; eof = ^D; eol = <undef>; eol2 = <undef>; swtch = <undef>; start = ^Q; stop = ^S; susp = ^Z; rprnt = ^R; werase = ^W; lnext = ^V; discard = ^O; min = 1; time = 5;
parenb -parodd -cmspar cs7 -hupcl -cstopb cread clocal -crtscts
ignbrk -brkint -ignpar -parmrk -inpck -istrip -inlcr -igncr -icrnl -ixon -ixoff -iuclc -ixany -imaxbel -iutf8
-opost -olcuc -ocrnl -onlcr -onocr -onlret -ofill -ofdel nl0 cr0 tab0 bs0 vt0 ff0
-isig -icanon -iexten -echo -echoe -echok -echonl -noflsh -xcase -tostop -echoprt -echoctl -echoke -flusho -extproc
```

## Changed parameters and their meaning

|Paramter|Original|Set|Description|Remark|
|:-:|:-|:-|:-|:-|
|speed|9600|300|Link speed in bauds|
|time|0|5|when comabined with `-icanon` set the read timeout in 1/10 seconds.
|cs|cs8|cs7|Define the character size to 7-bits|
|parenb|-parenb|parenb|Generate a parity bit in the output and expects a parity bit in input|Weird I though their was no parity bit involved|
|hupcl|hupcl|-hupcl|`-` negate sending an hang-up when the last process closes the tty|
|ignbrk|-ignbrk|ignbrk|Ignore break character|
|icrnl|icrnl|-icrnl|Do not translate carriage-return to new-line|
|ixon|ixon|-ixon|Disable XON/XOFF flow-control|
|opost|opost|-opost|Do not postprocess the output|Whatever it means...|
|onlcr|onlcr|-onlcr|Do not translate new-line to carriage-return|
|isig|isig|-isig|Disable interupt, quit and suspend special characters|
|icanon|icanon|-icanon|Disable erase, kill, werase and rprnt special characters|
|iexten|iexten|-iexten|Disable non-POSIX special characters|
|echo|echo|-echo|Do not echo input characters|
|echoe|echoe|-echoe|Same as `-crterase` - do not echo characters as backspace-space-backspace|whatever...
|echok|echok|-echok|Do not echo a new-line after a kill character|
|echoctl|echoctl|-echoctl|Do not echo control character in hat-notation (`^C`)
|echoke|echoke|-echoke|Do not kill a line by obeying the `echoctl` and `echok` settings

_Source: <https://www.man7.org/linux/man-pages/man1/stty.1.html>_

### What we most likely need

- `300` bauds Speed
- `7` data-bit
- `1` stop-bit
- Parity-bit `even`
- no `rts` `cts`
- no `xon` `xoff`
- Disable all special and control characters