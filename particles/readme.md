# Particle collision simulator, from https://algs4.cs.princeton.edu/61event/

To run:

    dotnet build
    dotnet run p10.txt

## original simulation start + loop

This is how original `StdDraw` loop worked:

- generate particles with initial velocity
- predict next collision for all particles, push collisions to event queue
- push redraw event to queue
- while event queue is not empty
    - pop from queue
    - if event is not valid (particles have collided since pushed to queue), ignore
    - update all particles to event time
    - recalculate next collisions for event popped off queue, push to queue
    - if redraw event
        - draw all particles
        - sleep for 20ms
        - push redraw event to queue


## plan for monogame simulation loop

- generate particles with initial velocity
- predict next collision for all particles, push collisions to event queue
- no redraw event required
- while true
    - update (time)
        - peek at queue
        - if next item time > now, break
        - else same as original loop:
            - if event is not valid (particles have collided since pushed to queue), ignore
            - update all particles to event time (should this only happen on draw?)
            - recalculate next collisions for event popped off queue, push to queue
    - draw (time)
        - update
        - draw all particles