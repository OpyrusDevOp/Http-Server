# HttpServer

A simple, lightweight HTTP server implementation in C#. This project provides a customizable framework for handling HTTP requests with routing and static file support.

---

## Features
- **Routing**: Easily map HTTP methods and paths to handlers.
- **Thread Pooling**: Efficient request handling with configurable thread limits.
- **Dynamic Parameters**: Support for extracting route and query parameters.
- **Static File Serving**: Simple static file hosting with content type detection.
- **Customizable Endpoints**: Add custom routes and handlers programmatically.
- **Modular Design**: Extendable with routers and subroutes.

---

## Usage

### Setting Up the Server

Create a new instance of `HttpServer` and specify the port:

```csharp
var server = new HttpServer(8080);
```

### Adding Routes

Add routes using `AddEndpoints`:
```csharp
server.AddEnpoint(HttpMethods.GET, "/hello", req => HttpResponses.Ok("Hello, World!"));
```

### Serving Static Files

The `HttpResponses.View` method serves files from the `webroot` directory:
```csharp
server.AddEndpoint(
    HttpMethods.GET,
    req =>
    {
        return HttpResponses.View("index.html");
    }
);
server.AddEnpoint(
  HttpMethods.GET,
  req =>{
      if (req.Params.TryGetValue("filename", out var css))
          return HttpResponses.View($"css/{css}");
      return HttpResponses.NotFound();
  },
  "/css/{filename}"
);
```

Ensure files are placed in the `webroot` folder of your application.

### Using Routers for Modular Design

Routers group related routes and can be nested:
```csharp
var userRouter = new Router("/users");
userRouter.AddEndpoint(HttpMethods.GET, req => HttpResponses.Ok("User List"), "/list");

server.AddRouter(userRouter);
```

### Starting the Server

Start the server by calling `Start`:
```csharp
server.Start();
```

---

## Configuration

### Setting Max Threads

You can configure the thread pool size to handle concurrent requests:
```csharp
server.SetMaxThreadAmount(8);
```

---

## Request Handling

The `HttpRequest` class automatically parses incoming requests. Extract parameters from the route or query string via `req.Params`:
```csharp
server.AddEnpoint(
  HttpMethods.GET,
  req =>{
      if (req.Params.TryGetValue("id", out var productId))
          return HttpResponses.Ok($"Product ID: {productId}");
      return HttpResponses.NotFound();
  },
  "/product/{id}"
);
```

---

## Response Helpers

Use the `HttpResponses` static class for common HTTP responses:
- `HttpResponses.Ok(body)`
- `HttpResponses.NotFound()`
- `HttpResponses.BadRequest()`
- `HttpResponses.InternalError()`

Or even for static files (views):
- `HttpResponses.View(viewName)`

Example:
```csharp
server.AddEnpoint(
  HttpMethods.GET,
  req => HttpResponses.Ok("Server is running!"),
  "/status"
);
```

---

## Example

```csharp
using Http_Server;

// Create the HTTP server instance
var app = new HttpServer(3000);

// Create the main API router
var apiRouter = new Router("api");

// Add endpoints to the API router


apiRouter.AddEndpoint(
    HttpMethods.GET,
    req =>
    {
        var id = req.Params["id"];
        Console.WriteLine(req.Params["id"]);

        return HttpResponses.Ok($"Request data of id {id}");
    },
    "/{id}"
);

apiRouter.AddEndpoint(
    HttpMethods.GET,
    req =>
    {
        return HttpResponses.View("index.html");
    }
);

apiRouter.AddEndpoint(
    HttpMethods.POST,
    req =>
    {
        var body = req.Body;
        return HttpResponses.Ok($"Data received: {body}");
    },
    "/submit"
);

// Create a sub-router for authentication-related routes
var authRouter = new Router("/auth");

authRouter.AddEndpoint(
    HttpMethods.GET,
    req =>
    {
        return HttpResponses.Ok("Login Page");
    },
    "/login"
);

authRouter.AddEndpoint(
    HttpMethods.POST,
    req =>
    {
        var body = req.Body;
        return HttpResponses.Ok($"Authenticating user with data: {body}");
    },
    "/login"
);

// Attach the sub-router to the main API router
apiRouter.AddSubRoute(authRouter);

// Create another sub-router for user-related routes
var userRouter = new Router("/user");

userRouter.AddEndpoint(
    HttpMethods.GET,
    req =>
    {
        return HttpResponses.Ok("User Profile");
    },
    "/profile"
);
userRouter.AddEndpoint(
    HttpMethods.PUT,
    req =>
    {
        var body = req.Body;
        return HttpResponses.Ok($"Updating user profile with data: {body}");
    },
    "/profile"
);

// Attach the user router to the main API router
apiRouter.AddSubRoute(userRouter);

// Add the main router to the HTTP server
app.AddRouter(apiRouter);

//Console.WriteLine(AppContext.BaseDirectory);

// Start the server
app.Start();
```

---

## Future Improvements
- [ ] Support for HTTPS.
- [ ] Middleware for request preprocessing.
- [ ] Improved error handling and logging.
- [ ] File upload support.

---
