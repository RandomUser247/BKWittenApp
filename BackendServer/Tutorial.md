# ASP.NET Core Projekt: Überblick und Funktion der einzelnen Teile

## Projektstruktur
/BackendServer \
├── Program.cs \
├── Data \
│ └── Migrations.cs (ContentDBContext und Models) \
├── Pages \
│ └── Razor Pages \
├── Services \
│ ├── Interfaces \
│ │ └── IUserService.cs (Schnittstellen) \ 
│ └── UserService.cs (Service-Implementierungen) \
├── wwwroot \
│ └── Static Files (CSS, JS, uploads, txt files) \
├── appsettings.json \
└── BackendServer.csproj\


## 1. **Program.cs**

**Funktion:**  
- **DbContext**: `ContentDBContext` wird registriert und verwendet SQLite als Datenbank.
- **Authentication**: Cookie-basierte Authentifizierung wird implementiert.
- **Authorization**: Rollenbasierte Autorisierungsrichtlinien ("Teacher", "Admin") werden hinzugefügt.
- **Razor Pages**: UI-Rendering und serverseitige Verarbeitung.

```csharp
builder.Services.AddDbContext<ContentDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContentDB")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireTeacherRole", policy => policy.RequireClaim("isTeacher", "True"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("isAdmin", "True"));
});

builder.Services.AddRazorPages();
```


## 2. Data/Migrations.cs ##

**Funktion:**

- Definiert die Datenbankmodelle wie User, Post, Media und Event.
- Der DbContext (ContentDBContext) stellt den Zugriff auf die Datenbank bereit.
- SeedDataAsync: Initiale Daten werden zur Datenbank hinzugefügt, falls diese leer ist.

```csharp
public class User
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsTeacher { get; set; }
    public bool IsAdmin { get; set; }
    public ICollection<Post> Posts { get; set; }
}

public class ContentDBContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    public async Task SeedDataAsync()
    {
        // Seed initial users, posts, and media
    }
}

```

## 3. Services/UserService.cs ##

**Funktion:**

- Der UserService implementiert die Logik zum Verwalten von Benutzern (z.B. Erstellung, Abruf).
- DI-Registrierung in Program.cs macht den Service in der gesamten Anwendung verfügbar.

```csharp
public class UserService : IUserService
{
    private readonly ContentDBContext _context;

    public UserService(ContentDBContext context)
    {
        _context = context;
    }

    public User GetUserById(int id) => _context.Users.Find(id);
}
```

## 4. appsettings.json ##

 **Funktion:**

- Speichert Konfigurationsdaten wie die Datenbankverbindung.

```csharp
{
  "ConnectionStrings": {
    "ContentDB": "Data Source=content.db"
  }
}
```

## 5. PasswordHelper (Hilfsklasse) ##

**Funktion:**

- Bietet Methoden zur sicheren Passwort-Hashing und Verifizierung mithilfe von bcrypt.

```csharp
public static class PasswordHelper
{
    public static string HashPassword(string password, string salt) =>
        BCrypt.Net.BCrypt.HashPassword(password, salt);

    public static bool VerifyPassword(string password, string storedHash) =>
        BCrypt.Net.BCrypt.Verify(password, storedHash);
}
```

## 6. Razor Pages ##

**Funktion:**

- Ermöglicht serverseitiges Rendern der UI-Komponenten.
- Zentrale Steuerung des User Interface und Interaktion mit dem Backend über Razor Pages.
```
@page
<h2>Welcome to the ASP.NET Core Application!</h2>
```

## 7. Zusammenarbeit der Komponenten ##

- Datenzugriff: ContentDBContext stellt sicher, dass alle Entitäten (User, Post, Media, Event) mit der SQLite-Datenbank synchronisiert sind.
- Services: Der UserService kümmert sich um die Geschäftslogik der Datenbankverwaltung. Diese Dienste sind in den Razor Pages und anderen Teilen der Anwendung verfügbar.
    - Bisher ist dies nicht in die Logik eingebunden aber schon als Service regristriert
- Authentication/Authorization: Cookie-Authentifizierung schützt Seiten vor unberechtigtem Zugriff. Benutzerrollen wie "Admin" und "Teacher" bestimmen den Zugriff auf spezielle Seiten.
- Password Management: Sichere Passwortverarbeitung über PasswordHelper.

## 8. Ablauf eines Requests ##

- Der Nutzer sendet eine Anfrage an die App.
- Die Middleware in Program.cs leitet Anfragen an die entsprechenden Razor Pages oder Controller weiter.
- Falls erforderlich, werden Daten aus der Datenbank über ContentDBContext abgerufen und vom jeweiligen Service verarbeitet.
- Authentifizierung und Autorisierung werden vor dem Zugriff auf geschützte Seiten überprüft.