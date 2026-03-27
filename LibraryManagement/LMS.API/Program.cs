using System.Text;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
var jwt = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!))
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
})
    .AddOData(opt => opt
        .AddRouteComponents("odata", GetEdmModel())
        .Select().Filter().OrderBy().Count().SetMaxTop(50));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Bearer token below.\nExample: 'eyJhbGciOiJIUzI...'"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.CustomSchemaIds(type => type.ToString());
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IReaderRepository, ReaderRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBorrowSlipRepository, BorrowSlipRepository>();
builder.Services.AddScoped<IBorrowSlipDetailRepository, BorrowSlipDetailRepository>();
builder.Services.AddScoped<IFineSlipRepository, FineSlipRepository>();
builder.Services.AddScoped<IBorrowRequestRepository, BorrowRequestRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReaderService, ReaderService>();
builder.Services.AddScoped<IBorrowSlipService, BorrowSlipService>();
builder.Services.AddScoped<IFineSlipService, FineSlipService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBorrowRequestService, BorrowRequestService>();

var app = builder.Build();
app.UseExceptionHandler(err => err.Run(async ctx => {
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    ctx.Response.StatusCode = (ex is ArgumentException || ex is InvalidOperationException) ? 400 : 500;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { 
        error = ex?.Message,
        detail = ex?.StackTrace
    });
}));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static Microsoft.OData.Edm.IEdmModel GetEdmModel()
{
    var b = new Microsoft.OData.ModelBuilder.ODataConventionModelBuilder();
    b.EntitySet<BookDto>("Books");
    b.EntitySet<CategoryDto>("Categories");
    b.EntitySet<ReaderDto>("Readers");
    b.EntitySet<AccountDto>("Accounts");
    b.EntitySet<BorrowSlipDto>("BorrowSlips");
    b.EntitySet<FineSlipDto>("FineSlips");
    b.EntitySet<PublisherDto>("Publishers");
    b.EntitySet<BorrowRequestDto>("BorrowRequests");
    return b.GetEdmModel();
}
