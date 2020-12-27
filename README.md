# UnitySerialTest_PianoWithPiezobuzzer
Unity-Arduino Serial Communication Test (Piano play with piezo buzzer And RTTTL parsing)

## License
MIT License

Copyright (c) 2020 wjoh0315

     

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

     

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

     

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Setting
<img src="https://ifh.cc/g/WlWT3R.jpg" width="450" height="500">

Change the serial port name to the serial port name used by the Arduino board.

**NOTE: It would be better not touch another component setting (Excluding RTTTL file setting)**

## Circuit
<img src="https://ifh.cc/g/r2sQVL.jpg" width="800" height="500">
<img src="https://ifh.cc/g/7w52F5.png" width="600" height="500">

## Execution
* **Arduino IDE**

Upload UnitySerialTest.ino sketch to Arduino board

* **Unity Editor**

Select RTTTL file (Text Asset) in RTTTL folder or create new RTTTL file, apply it to the component setting "RTTTL File" in "PianoWithSerial".
And, click execution button in unity editor.

Then, It will be working like this.

<img src="https://ifh.cc/g/YPOukF.jpg" width="820" height="500">

Enjoy!

## Other
### What is the RTTTL?
RTTTL is Ring Tone Transfer Language, [Wikipedia](https://en.wikipedia.org/wiki/Ring_Tone_Transfer_Language)

### Communication Protocol in my project

* **Syntax**
     - **Data transfer:** digital
     - **Encoding:** 1 byte (8 bits) ASCII characters (string)

* **Sematics**
     - **\<Sender>**: identify sender, start reading
     - **\</Sender>**: Re-identify sender, end reading
     - **dataName**: data identification
     - **(Type)**: identify data type
     - **\<data>**: data to read
     - **':'**: Start reading data of that name
     - **';'**: End of reading data of the name

* **Timing:**
     - **baudrate:** 115200 bps
     
* **Example**
```
<Unity>
     name(System.String):<noteName>;
     frequency(System.Double):<frequency>;
     time(System.Single):<time>;
     keyIndex(System.Int32):<Index>;
</Unity>
```
(I Referred to html transmission method of HTTP protocol)

### Blog URL
Blog in korea ["퍼텐셜의 개발일지"](https://blog.naver.com/wjoh0315)
