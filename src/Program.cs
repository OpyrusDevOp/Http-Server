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
