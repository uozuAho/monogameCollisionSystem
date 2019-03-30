# Particle collision simulator

Copied from https://algs4.cs.princeton.edu/61event/ and adapted to C#, then tweaked a bit more.

To run:

    dotnet build
    dotnet run


# todo

- Collisions are missed unless the event queue is allowed to grow to arbitrary size. This results
  in lots of GC and eventually runs out of memory. Does the queue really need to keep growing,
  or is my max size implementation incorrect? Intuitively, I would think only the top N*N
  collisions need to be kept on the queue.

- The above GC results in very stuttery performance. Pre-filling the data structures helps.
  Interestingly, when the event queue size is limited and most particles have flown off the
  screen, performance is still stuttery. Update, draw and fps metrics are all very good at
  this point, and VS indicates there is no GC happening. Why the stutter, then? Are other
  processes given priority? That would maybe explain why the metrics are good but performance
  is bad (if the metric timers are using process time...?).
