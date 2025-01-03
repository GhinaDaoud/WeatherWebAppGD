var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

//builder.Configuration.AddJsonFile("appsettings.json");

//builder.Configuration.AddEnvironmentVariables();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  
              .AllowAnyMethod() 
              .AllowAnyHeader(); 
    });
});

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.UseCors("AllowAll");

app.Run();
