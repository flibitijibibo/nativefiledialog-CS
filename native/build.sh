#!/bin/bash

set -ex

cd "`dirname "$0"`"

rm -f libnfd.so libnfd_gtk.so libnfd_zenity.so
rm -f libnfd.dylib
rm -f nfd.dll

# Linux (includes GTK, Zenity, and a library to support both)
cc -O3 -fpic -fPIC -shared -o libnfd_gtk.so nfd_common.c nfd_gtk.c `pkg-config --cflags gtk+-3.0` -lgtk-3 -lgobject-2.0 -lglib-2.0 -Wl,--no-undefined
cc -O3 -fpic -fPIC -shared -o libnfd_zenity.so nfd_common.c nfd_zenity.c -Wl,--no-undefined
cc -O3 -fpic -fPIC -shared -o libnfd.so nfd_linux.c `sdl2-config --cflags --libs` -Wl,--no-undefined

# Windows
x86_64-w64-mingw32-gcc -O3 -fpic -fPIC -shared -o nfd.dll nfd_win.c nfd_common.c -lole32

# macOS
x86_64-apple-darwin18-cc -mmacosx-version-min=10.9 -O3 -fpic -fPIC -shared -o libnfd.dylib nfd_common.c nfd_cocoa.m -lobjc -framework Cocoa -install_name @rpath/libnfd.dylib
