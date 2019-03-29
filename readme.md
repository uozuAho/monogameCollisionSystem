# Particle collision simulator

Copied from https://algs4.cs.princeton.edu/61event/ and adapted to C#.

To run:

    dotnet build
    dotnet run


# todo

- improve performance and memory usage
	- when not limited the event queue grows very quickly, consuming a lot of memory
	- when limited, less memory is consumed, but collisions are missed. Is there an
	  appropriate limit that will not miss collisions, while limiting memory usage?
	- even when pre-filling all object arrays and limiting memory usage, performance
	  is very jittery. Why?