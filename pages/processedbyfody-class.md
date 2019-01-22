# ProcessedByFody Class

Fody adds a new class to 

`class ProcessedByFody`

The purpose of this type is to flag that the assembly has been processed by Fody. This allows Fody to skip processing when Visual Studio has no changed the assembly.

Now I am the first to admit that this is a hack. Unfortunately it was the only one I could think of to reliably mark and assembly as being processed. The other options were ruled out for various reasons.

