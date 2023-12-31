using System.Text.RegularExpressions;
using Albums.Api.Configuration;
using Albums.Api.Data;
using Albums.Api.Models;
using Albums.Api.Utils;
using Albums.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.ResolveDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("v1/albums/{pageSize}/{pageIndex}", async ([FromServices] IAlbumRepository repo, int pageSize, int pageIndex) =>
{
    IQueryable<Album>? albums = (await repo.GetAll()).AsQueryable();
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

app.MapGet("v1/album/{id}", async ([FromServices]IAlbumRepository repo, string id) =>
{
    return await repo.GetById(id) is Album album ?
        Results.Ok(new Result(true, "Registro encontrado com sucesso", album)) : 
        Results.NotFound();
})
.WithName("GetAlbumById")
.WithOpenApi(operation => new(operation)
{
    Description = "Get An Album By Id"
})
.Produces<Album>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("v1/album/filter", async ([FromServices]IAlbumRepository repo, [FromBody]AlbumFilterViewModel albumFilter) =>
{
    FilterDefinitionBuilder<Album> builder = Builders<Album>.Filter;
    FilterDefinition<Album> filter = FilterDefinition<Album>.Empty; 

    if (albumFilter != null)
    {
        if (!string.IsNullOrEmpty(albumFilter.Artist))
        {
            filter = builder.Regex("Artist", new Regex($"^{albumFilter.Artist}$", RegexOptions.IgnoreCase));
        }

        if (!string.IsNullOrEmpty(albumFilter.Title))
        {
            var titleFilter = builder.Regex("Title", new Regex($"^{albumFilter.Title}$", RegexOptions.IgnoreCase));
            filter &= titleFilter;
        }

        if (albumFilter.Year != null)
        {
            var yearFilter = builder.Eq(a => a.Year, albumFilter.Year);
            filter &= yearFilter;
        }
    }

    IEnumerable<Album>? result = filter != FilterDefinition<Album>.Empty ? 
                                     await repo.Where(filter) :
                                     await repo.GetAll();

    return Results.Ok(new Result(true, "Busca realizada com sucesso", result));
})
.WithName("FilterAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Filter an Album"
})
.Produces<Album>(StatusCodes.Status200OK);

app.MapPost("v1/album", async ([FromServices] IAlbumRepository repo, [FromBody] AlbumViewModel albumViewModel) =>
{
    albumViewModel.Validate();

    if (!albumViewModel.IsValid)
        return Results.BadRequest(albumViewModel.Notifications);

    Album album = albumViewModel.ToEntity();

    await repo.Create(album);
   
    return Results.Created($"v1/album/{album.Id}", new Result(true, "Registro criado com sucesso", album));
})
.WithName("CreateAlbum")
.WithOpenApi(operation => new(operation)
{
    Description = "Register an Album"
})
.Produces<Album>(StatusCodes.Status201Created);

app.MapPut("v1/album/{id}",
  async ([FromServices]IAlbumRepository repo, [FromBody] AlbumViewModel albumViewModel, string id) =>
  {
      albumViewModel.Validate();

      if (!albumViewModel.IsValid)
          return Results.BadRequest();

      Album? album = await repo.GetById(id);

      if (album is null)
          return Results.NotFound();

      album.Title = albumViewModel.Title;
      album.Artist = albumViewModel.Artist;
      album.Year = albumViewModel.Year;

      await repo.Update(id, album); 
      
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

app.MapDelete("v1/album/{id}", async ([FromServices]IAlbumRepository repo, string id) =>
{
    Album? album = await repo.GetById(id);

    if (album is null)
        return Results.NotFound();

    await repo.Delete(id);

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
