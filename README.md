ImageDiffDBCreator

input a folderpath containing all place snapshots to be included
filename is: blocknumber-UnixTimestamp.png

---
FileStructure:

128 bytes color table
32 entries of 1 byte ID + 3 bytes RGB24
02 + 00 9E AA

Ref Image ASCII name surrounded by colons
:RefImage:0-1648822500.png:

Ref Image data as bytedump of the og file

ImageData: [dump of the og file]

diff time compared to last image as 2 bytes in seconds surrounded by colons
:diff: 00 0C :

Pixel Position x (1.5 bytes) + y (1.5 bytes) + color as denoted in color table (1 bytes)
32 C + 1 D8 + 0A

Repeat till next diff block indicating a new diferential image

---
make new Ref Image entry if block number changes
i.e. 0-1648822500.png -> 1-1648822500.png

the block number applies an offset to x,y of the image size depending on block location
0 1
2 3
