This is mean to be a simple http server lib.
It will be working in multithreading, with a parallel architecture.

# Hot to use

Create an instance of the HttpServer class.

```csharp
var app = new HttpServer(3000);
```

Where `3000` is the port to listen to.

Then you can add different endpoint/route to it. Think of it almost like express js

```csharp
app.AddRoute(HttpMethod.GET, "/", request => return HttpResponse.Ok("Hello World"));
app.AddRoute(HttpMethod.POST, "/", request => {
  var body = request.Body;
  // make actions ...
return HttpResponse.Ok();
})
```

And then finally you can run the server with :

```csharp
app.Start();
```

_Note :_ You can actually change the max amount of threads running simultaneous to handle request.

```csharp
app.SetMaxThreadAmount(8);
```

It mainly depends on the CPU running the app.
