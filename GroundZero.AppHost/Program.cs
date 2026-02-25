using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// var pg = builder.AddPostgres("db")
//     .WithPgAdmin(o => { o.WithHostPort(8080); })
//     .AddDatabase("groundzero");

builder.AddProject<GroundZero_Web>("api");
    // .WithReference(pg);

builder.Build().Run();
