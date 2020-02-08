# BitStream
Stream wrapper to read/write bits and other data types

## Introduction
Bitstreams unlike streams using `BinaryReader` and `BinaryWriter` use the stream at bit level, allowing read/write bits

This library project attempts to add bit manipulation to a stream while using known `Stream`, `BinaryReader` and `BinaryWriter` class methods

## Usage
Initialize a **BitStream** using a Stream or `byte[]` int the constructor
```
//Using Stream stream;
BitStream bitstream = new BitStream(stream);
//Using byte[] bytes;
BitStream bitstream = new BitStream(bytes);
```
You can set the **BitStream** to use most-significant bit or less-significant bit as bit 0, by default LSB is used
```
//Using Stream stream and LSB;
BitStream bitstream = new BitStream(bytes);
//Using Stream stream and MSB;
BitStream bitstream = new BitStream(bytes,true);
```
After reading/writing the stream use **GetStream()** to get the stream or **GetStreamData()** to get a `byte[]` of the data in the stream

## Features

### Seeking, advancing and returning bits
Seeking in a BitStream uses *Seek(long offset, int bit)* to specify the stream position

Using bit >= 8 will increase the offset automatically

Using *AdvanceBit()* and *ReturnBit()* allows moving the bit offset forward or backwards by one

You can also use indexer *[long offset, int bit]* to Seek the specified offset and get a `bool` that specifies if the seeked offset is inside the stream

```
if (bitstream[0xE8, 0])
{
	bitstream.ReadBytes(128);
}
```

### Reading/Writing bits
Reading a bit is easy using *ReadBit()* method, it returns a **Bit** which can be assigned to a byte, int or bool
```
Bit bit = bitstream.ReadBit();
byte b = bitstream.ReadBit();
int i = bitstream.ReadBit();
bool boolean = bitstream.ReadBit();
```
Writing a bit using *WriteBit(Bit bit)* is also possible with `byte`, `int` and `bool`
```
bitstream.WriteBit(1);
bitstream.WriteBit(b);
bitstream.WriteBit(i);
bitstream.WriteBit(true);
```

**BitStream** can also read/write arrays of Bits with *ReadBits(int length)* and *WriteBits(Bit[] bits)* methods

### Reading/Writing data types
Just like `BinaryReader` and `BinaryWriter` can read/write data types like int, bool and string, **BitStream** can read/write this data types, currently these are supported:
* byte (Can specify number of bits)
* sbyte (Can specify number of bits)
* byte[]
* bool (Reading/Writing byte)
* short
* ushort
* int
* uint
* long
* ulong
* 24bit int/uint
* 48bit long/ulong
* char
* string

### Character Encoding
**BitStream** allows setting a character encoding on constructor *BitStream([Stream stream OR byte[] buffer], Encoding encoding, [bool MSB = false])* or using the method *SetEncoding(Encoding encoding)* to read/write characters

### Shifts
**BitStream** can do bitwise and circular shifts on current position byte using *bitwiseShift(int bits, bool leftShift)* and *circularShift(int bits, bool leftShift)* or using the current bit position create a byte and use it using *bitwiseShiftOnBit(int bits, bool leftShift)* and *circularShiftOnBit(int bits, bool leftShift)* methods

### Bitwise Operators
**BitStream** can do bitwise operations `AND`,`OR`,`XOR` and `NOT` at byte and bit level

## NuGet
NuGet package is available here https://www.nuget.org/packages/rubendal.BitStream
