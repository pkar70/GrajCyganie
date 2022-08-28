using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreSWebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ¿e z .Net5.0 startup ma byæ tutaj?
builder.Services.AddDbContext<NetCoreSWebApp.Models.LocalSQLContext>();
//builder.Services.AddScoped<IVideoParamRepository,VideoParamRepository>();
//builder.Services.AddScoped<IActorNameRepository, ActorNameRepository >();
//builder.Services.AddScoped<IActorFilmRepository, ActorFilmRepository >();
//builder.Services.AddScoped<IStoreFileRepository, StoreFileRepository >();
// Singleton: dla wszystkich requests ten sam; Scooped: nowy dla ka¿dego Request; Transient: nowy dla ka¿dego new(dep inject)
builder.Services.AddSingleton<IVideoParamRepository, VideoParamRepository>();
builder.Services.AddSingleton<IActorNameRepository, ActorNameRepository>();
builder.Services.AddSingleton<IActorFilmRepository, ActorFilmRepository>();
builder.Services.AddSingleton<IStoreFileRepository, StoreFileRepository>();

// jak kilka dla interface, to wybiera ostatni Add
// ale mozna w ctor(IEnumerable<Iinterface> cos) i miec dostep do wszystkich - w ten sposob mozna zrobic dwie bazy, tylko kazda musialaby miec Name="", albo inny rozrozniacz

builder.Services.AddControllersWithViews();

// zrobiæ w³asne, z api/v2/ , oraz wlasne - jako zestawienie wszystkich
// https://github.com/dotnet/aspnet-api-versioning/wiki/API-Version-Reader
// https://github.com/dotnet/aspnet-api-versioning/wiki/Versioning-via-the-URL-Path
// https://github.com/dotnet/aspnet-api-versioning/blob/ms/src/Common/Versioning/QueryStringApiVersionReader.cs
// https://github.com/dotnet/aspnet-api-versioning/blob/ms/src/Common/Versioning/IApiVersionReader.cs
builder.Services.AddApiVersioning(opt =>
{
    opt.ReportApiVersions = true;
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader(),
        new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader(),
        new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader("ver"),
        new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader()
        // wtedy trzeba [Route("api/v{version:apiVersion}/[Controller")]
        );
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

//// pkar ze szkolenia 2
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//// pkar ze szkolenia 2
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id:int?}");
//});


// <a asp-controller="Pie" asp-action="List">View pie list</a>

//app.UseAuthorization();

app.MapRazorPages();

app.Run();



// var books = _db.Query<Book>(sqL:"GetAllBooks", commandType: CommandType.StoredProcedure).ToList();
// httpclient.PostAsJsonAsync(url, structClass)

// https://www.googleapis.com/books/v1/volumes?q=isbn:NUMER
// 978-83-66084-73-5
// https://www.googleapis.com/books/v1/volumes?q=isbn:9788366084735
// zwraca dane o ksia¿ce

// DI ILogger, albo ILoggerFactory, i _logger=loggerfactory.CreateLogger("CATEGORY")