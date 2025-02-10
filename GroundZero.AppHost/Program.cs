using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pg = builder.AddPostgres("db")
    .WithPgWeb()
    .AddDatabase("groundzero");

builder.AddProject<GroundZero_Api>("api")
    .WithReference(pg);

builder.Build().Run();