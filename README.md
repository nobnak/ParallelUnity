# Parallel.For for Unity

[Unity Package](Parallel.unitypackage)

 - For Loop Execution in Parallel

## Usage
Declare Parallel Task
```c#
System.Action<int> task = (i) => {
	var p = particles[i];
	var speed = p.velocity.magnitude;
	p.time += speed * dt;
	p.position += c.velocity * dt;
	particles[i] = p;
};
```
Call Parallel.For()
```c#
Parallel.For(0, particles.Length, task);
```

## References
1. [ParallelForEach](https://gist.github.com/0x53A/30f9c3005932db2c74b9)
