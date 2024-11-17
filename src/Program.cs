var app = new HttpServer(3000);

app.AddRoute(
    HttpMethod.GET,
    "/",
    req =>
    {
        return HttpResponses.Ok("Hello World");
    }
);

app.Start();
