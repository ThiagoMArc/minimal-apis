using Albums.Api.Data;
using Albums.Api.ViewModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("v1/albums", (AppDbContext dbContext) =>
{
    List<Album> albums = dbContext.Albums.ToList();
    return Results.Ok(albums);
})
.WithName("GetAlbums")
.WithOpenApi(operation => new(operation)
{
    Description = "Get All Albums"
})
.Produces<List<Album>>(StatusCodes.Status200OK);

app.MapGet("v1/album/{id}", async (AppDbContext dbContext, Guid id) =>
{
    return await dbContext.Albums.FindAsync(id) is Album album ?
        Results.Ok(album) : Results.NotFound();
})
.WithName("GetAlbumById")
.WithOpenApi(operation => new(operation)
{
    Description = "Get An Album By Id"
})
.Produces<Album>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);


app.MapPost("v1/album", async (AppDbContext dbContext, AlbumViewModel albumViewModel) =>
{
    if (!albumViewModel.IsValid)
        return Results.BadRequest(albumViewModel.Notifications);

    Album album = albumViewModel.ToEntity();

    dbContext.Albums.Add(album);
    await dbContext.SaveChangesAsync();

    return Results.Created($"v1/album/{album.Id}", album);
})
.WithName("CreateAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Register an Album"
})
.Produces<Album>(StatusCodes.Status201Created); ;

app.MapPut("v1/album/{id}",
  async (AppDbContext dbContext, AlbumViewModel albumViewModel, Guid id) =>
  {
     albumViewModel.Validate();

     if(!albumViewModel.IsValid)
        return Results.BadRequest();

      Album? album = await dbContext.Albums.FindAsync(id);

      if (album is null)
          return Results.NotFound();

      album.Title = albumViewModel.Title;
      album.Artist = albumViewModel.Artist;
      album.Year = albumViewModel.Year;

      dbContext.Albums.Update(album);

      await dbContext.SaveChangesAsync();

      return Results.Ok(album);
  })
.WithName("UpdateAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Updates an Album"
})
.Produces<Album>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("v1/album/{id}", async (AppDbContext dbContext, Guid id) =>
{
    Album? album = await dbContext.Albums.FindAsync(id);

    if (album is null)
        return Results.NotFound();

    dbContext.Albums.Remove(album);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
})
.WithName("DeleteAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Deletes an Album"
})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.Run();