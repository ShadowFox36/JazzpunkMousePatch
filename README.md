JazzPunkMouse Patch

This is just a simple patch I made to fix the laggy feeling of mouse input in Jazzpunk.

The reason for the mouse input feeling so laggy in Jazzpunk is that the mouse input uses FixedUpdate in the Unity engine, which by default is only at a rate of 50 fps. I changed this to use Update, which is updated every frame, allowing for way less mouse latency.
