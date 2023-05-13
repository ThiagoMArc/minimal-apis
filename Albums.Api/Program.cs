using Albums.Api.Data;
using Albums.Api.Models;
using Albums.Api.Utils;
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


app.MapGet("v1/albums/{pageSize}/{pageIndex}", (AppDbContext dbContext, int pageSize, int pageIndex) =>
{
    IQueryable<Album> albums = dbContext.Albums.AsQueryable();
    PagedList<Album> albumsPaged = PagedList<Album>.ToPagedList(albums, pageIndex, pageSize);

    object metadaData = new
    {
        currentPage = albumsPaged.CurrentPage,
        totalPages = albumsPaged.TotalPages,
        pageSize = albumsPaged.PageSize,
        totalCount = albumsPaged.TotalCount,
        hasNextPage = albumsPaged.HasNext
    };

    Result result = new Result(true,
                             "Dados encontrados com sucesso",
                             new
                             {
                                 currentPage = albumsPaged.CurrentPage,
                                 totalPages = albumsPaged.TotalPages,
                                 pageSize = albumsPaged.PageSize,
                                 totalCount = albumsPaged.TotalCount,
                                 result = albumsPaged
                             });

    return Results.Ok(result);
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
        Results.Ok(new Result(true, "Registro encontrado com sucesso", album)) : Results.NotFound();
})
.WithName("GetAlbumById")
.WithOpenApi(operation => new(operation)
{
    Description = "Get An Album By Id"
})
.Produces<Album>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("v1/album/filter", async (AppDbContext dbContext, AlbumFilterViewModel albumFilter) =>
{
    Func<Album, bool>? predicate = null;

    if (albumFilter != null)
    {
        if (!string.IsNullOrEmpty(albumFilter.Artist))
        {
            predicate = a => a.Artist == albumFilter.Artist;
        }

        if (!string.IsNullOrEmpty(albumFilter.Title))
        {
            predicate = predicate + (a => a.Title == albumFilter.Title);
        }

        if (albumFilter.Year != null)
        {
            predicate = predicate + (a => a.Year == albumFilter.Year);
        }
    }

    IEnumerable<Album>? result = predicate != null ? dbContext.Albums.Where(predicate) :
                                     dbContext.Albums.ToList();

    return Results.Ok(new Result(true, "Busca realizada com sucesso", result));
})
.WithName("FilterAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Filter an Album"
})
.Produces<Album>(StatusCodes.Status200OK);

app.MapPost("v1/album", async (AppDbContext dbContext, AlbumViewModel albumViewModel) =>
{
    if (!albumViewModel.IsValid)
        return Results.BadRequest(albumViewModel.Notifications);

    Album album = albumViewModel.ToEntity();

    dbContext.Albums.Add(album);
    await dbContext.SaveChangesAsync();

    return Results.Created($"v1/album/{album.Id}", new Result(true, "Registro criado com sucesso", album));
})
.WithName("CreateAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Register an Album"
})
.Produces<Album>(StatusCodes.Status201Created);

app.MapPut("v1/album/{id}",
  async (AppDbContext dbContext, AlbumViewModel albumViewModel, Guid id) =>
  {
      albumViewModel.Validate();

      if (!albumViewModel.IsValid)
          return Results.BadRequest();

      Album? album = await dbContext.Albums.FindAsync(id);

      if (album is null)
          return Results.NotFound();

      album.Title = albumViewModel.Title;
      album.Artist = albumViewModel.Artist;
      album.Year = albumViewModel.Year;

      dbContext.Albums.Update(album);

      await dbContext.SaveChangesAsync();

      return Results.Ok(new Result(true, "Registro atualizado com sucesso", album));
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
