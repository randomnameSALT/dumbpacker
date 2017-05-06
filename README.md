# dumbpacker
A file compressor / self-extractor for PE files.

**USAGE:**

\>dumbpacker.exe -{pmbh} <PATH_TO_PE_FILE1> <PATH_TO_PE_FILE2> ...

**OPTIONS:**

-p(ack): Compresses target PE file, and tacks the depacking stub on top.

-m(ask): XORs compressed bytes.

-b(ackup): Always make backups :)

-h(alp): Print usage && options

**ABOUT:**

I wanted to get more familiar with C#'s filestream libs, and thought it'd be a good time to write a gimpy little packer to do that! Program uses zlib to compress target PE files, and then glues a depacker on top of the resulting bytes. The packer and depacker are fantastically stupid, so I imagine the TODO on this thing is pretty monstrous if I ever want to take it out of toy code status!
