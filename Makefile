# Makefile for nativefiledialog#
# Written by Ethan "flibitijibibo" Lee

build: clean
	mkdir bin
	mcs /unsafe -debug -out:bin/nativefiledialog-CS.dll -target:library nativefiledialog.cs

clean:
	rm -rf bin
