using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var sampleProject = builder.AddProject<Sample>("sample")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
