var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FilmsDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped<IFilmRepository, FilmRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepository>(new UserRepository());

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/login", [AllowAnonymous] async (HttpContext context, ITokenService tokenService, IUserRepository userRepository) =>
{
    UserModel model = new()
    {
        UserName = context.Request.Query["username"],
        Password = context.Request.Query["password"]
    };
    var userDto = userRepository.GetUser(model);
    if (userDto == null) return Results.Unauthorized();
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"], userDto);
    return Results.Ok(token);
});

app.MapGet("/films", [Authorize] async (IFilmRepository repository) =>
    await repository.GetFilmsAsync()).
    Produces<List<Film>>(StatusCodes.Status200OK).
    WithName("GetAllFilms").
    WithTags("Getters");

app.MapGet("/films/{id}", [Authorize] async (int id, IFilmRepository repository) =>
    await repository.GetFilmAsync(id) is Film film
    ? Results.Ok(film)
    : Results.NotFound()).
    Produces<Film>(StatusCodes.Status200OK).
    WithName("GetFilm").
    WithTags("Getters"); ;

app.MapPost("/films", [Authorize] async (Film film, IFilmRepository repository) =>
    {
        await repository.InsertFilmAsync(film);
        await repository.SaveAsync();
        return Results.Created($"/films/{film.Id}", film);
    }).
    Accepts<Film>("application/json").
    Produces<Film>(StatusCodes.Status201Created).
    WithName("CreateFilm").
    WithTags("Creators");

app.MapPut("/films", [Authorize] async (Film film, IFilmRepository repository) =>
    {
    try
    {
        await repository.UpdateFilmAsync(film);
        await repository.SaveAsync();
        return Results.Ok();
    }
    catch
    {
        return Results.NotFound();
    }}).
    Accepts<Film>("application/json").
    WithName("UpdateFilm").
    WithTags("Updaters");

app.MapDelete("/films/{id}", [Authorize] async (int id, IFilmRepository repository) =>
{
    try
    {
        await repository.DeleteFilmAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    }
    catch
    {
        return Results.NotFound();
    }}).
    WithName("DeleteFilm").
    WithTags("Deleters");

app.MapGet("/films/search/{title}", [Authorize] async (string title, IFilmRepository repository) => await repository.FindFilmAsync(title)
    is IEnumerable<Film> film
    ? Results.Ok(film)
    : Results.NotFound(Array.Empty<Film>)).
    Produces<List<Film>>(StatusCodes.Status200OK).
    Produces<List<Film>>(StatusCodes.Status404NotFound).
    WithName("SearchFilms").
    WithTags("Getters").
    ExcludeFromDescription();

app.UseHttpsRedirection();

app.Run();
