using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using MinAPI.Dtos;
using MinAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConBuilder = new SqlConnectionStringBuilder();

sqlConBuilder.ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection");
sqlConBuilder.UserID = builder.Configuration["UserId"];
sqlConBuilder.Password = builder.Configuration["Password"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlConBuilder.ConnectionString));

builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/v1/commands", async (ICommandRepo repo, IMapper mapper)=>
{
    var commands = await repo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
    
});

app.MapGet("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id)=>
{
    var command = await repo.GetCommandById(id);
    if(command == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(mapper.Map<CommandReadDto>(command));
});

app.MapPost("api/v1/commands", async (ICommandRepo repo, IMapper mapper, CommandCreateDto commandCreateDto)=>
{
    var commandModel = mapper.Map<Command>(commandCreateDto);
    await repo.CreateCommand(commandModel);
    await repo.SaveChangesAsync();

    var commandReadDto = mapper.Map<CommandReadDto>(commandModel);

    return Results.Created($"api/v1/commands/{commandReadDto.Id}", commandReadDto);
});

app.MapPut("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDto commandUpdateDto)=>

{
    var command = await repo.GetCommandById(id);
    if(command == null)
    {
        return Results.NotFound();
    }
    
    mapper.Map(commandUpdateDto, command);

    await repo.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/v1/commands/{id}", async (ICommandRepo repo, int id)=>
{
    var command = await repo.GetCommandById(id);
    if(command == null)
    {
        return Results.NotFound();
    }

    repo.DeleteCommand(command);
    await repo.SaveChangesAsync();

    return Results.NoContent();
}
);



app.Run();

