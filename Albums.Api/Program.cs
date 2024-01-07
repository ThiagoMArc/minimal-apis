using Albums.Api.Configuration;
using Albums.Api.Data;
using Albums.Api.Models;
using Albums.Api.Utils;
using Albums.Api.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.ResolveDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("v1/albums/{pageSize}/{pageIndex}", (IAlbumRepository repo, int pageSize, int pageIndex) =>
{
    IQueryable<Album> albums = repo.GetAll();
    PagedList<Album> albumsPaged = PagedList<Album>.ToPagedList(albums, pageIndex, pageSize);

    Result result = new(true,
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

app.MapPost("v1/album/info", async (IAlbumRepository repo, AlbumFilterViewModel albumFilter) =>
{
    Func<Album, bool>? predicate = null;

    if (albumFilter != null)
    {
        if (!string.IsNullOrEmpty(albumFilter.Artist))
            predicate = a => a.Artist.ToLowerInvariant() == albumFilter.Artist.ToLowerInvariant();
        

        if (!string.IsNullOrEmpty(albumFilter.Title))
            predicate = predicate + (a => a.Title.ToLowerInvariant() == albumFilter.Title.ToLowerInvariant());
        

        if (albumFilter.Year != null)
            predicate = predicate + (a => a.Year == albumFilter.Year);
        
    }

    IEnumerable<Album>? result = predicate != null ? repo.Where(predicate) :
                                     repo.GetAll().AsEnumerable();

    return Results.Ok(new Result(true, "Busca realizada com sucesso", result));
})
.WithName("FilterAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Filter Albums by their infos"
})
.Produces<Album>(StatusCodes.Status200OK);

app.MapPost("v1/album", async (IAlbumRepository repo, AlbumViewModel albumViewModel) =>
{
    if (!albumViewModel.IsValid)
        return Results.BadRequest(albumViewModel.Notifications);

    Album album = albumViewModel.ToEntity();

    repo.Add(album);
    await repo.SaveChangesAsync();

    return Results.Created($"v1/album/{album.Id}", new Result(true, "Registro criado com sucesso", album));
})
.WithName("CreateAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Register an Album"
})
.Produces<Album>(StatusCodes.Status201Created);

app.MapPut("v1/album/{id}",
  async (IAlbumRepository repo, AlbumUpdateViewModel albumViewModel, Guid id) =>
  {
      Album? album = await repo.GetByIdAsync(id);

      if (album is null)
          return Results.NotFound();

      if(!string.IsNullOrWhiteSpace(albumViewModel.Title))
        album.Title = albumViewModel.Title;
      
      if(!string.IsNullOrWhiteSpace(albumViewModel.Artist))
        album.Artist = albumViewModel?.Artist;
      
      if(albumViewModel?.Year is not null and not 0)
        album.Year = albumViewModel.Year.Value;

      if(albumViewModel?.TrackList is not null && albumViewModel.TrackList.Count != 0)
        album.Tracklist = albumViewModel.TrackList;

      repo.Update(album);

      await repo.SaveChangesAsync();

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

app.MapDelete("v1/album/{id}", async (IAlbumRepository repo, Guid id) =>
{
    Album? album = await repo.GetByIdAsync(id);

    if (album is null)
        return Results.NotFound();

    repo.Delete(album);
    await repo.SaveChangesAsync();

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
