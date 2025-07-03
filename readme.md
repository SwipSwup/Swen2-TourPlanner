    # 📝 Logging in TourPlanner (.NET + log4net)

---

## 🧱 Directory

Logs are stored in:
```
/UI/Logs/TourPlanner.log
```

---

## 🧪 Usage in Code

1. **Import log4net**
```csharp
using log4net;
```

2. **Define a static logger per class**
```csharp
private static readonly ILog _log = LogManager.GetLogger(typeof(TourService));
```

3. **Log messages**
```csharp
_log.Info("Tour created successfully.");
_log.Warn("Estimated time is unusually high.");
_log.Error("Failed to retrieve route data", ex);
```

---

## 🎯 Best Practices

| Level     | Purpose                             |
|-----------|-------------------------------------|
| `DEBUG`   | Internal flow, data transformations |
| `INFO`    | Normal application events           |
| `WARN`    | Something unexpected but recoverable|
| `ERROR`   | Exception caught, user impacted     |
| `FATAL`   | Application can't continue          |

---

## ✅ Where Logging Is Used

| Layer | Examples of Logging                     |
|-------|------------------------------------------|
| UI    | App start, user actions (optional)       |
| BL    | Tour creation, API calls, business logic |
| DAL   | Database operations (e.g., `AddTour`)     |
| REST  | Errors during OpenRouteService calls     |

# 📘 Using TourService in the UI Layer (WPF, MVVM with Dependency Injection)

This project uses dependency injection to manage services like `TourService` and `MainViewModel`. Here's how to correctly consume `TourService` in your WPF UI when using Microsoft.Extensions.Hosting and `AppHost.Services`.

---

## 🧩 Dependency Registration

In your DI configuration (usually in `App.xaml.cs` or `Program.cs`):

```csharp
services.AddScoped<IRouteService, RouteService>();
services.AddScoped<ITourRepository, TourRepository>();
services.AddScoped<TourService>();
services.AddScoped<MainViewModel>();
```

---

## 🧱 Retrieving Services in WPF

In your `MainWindow.xaml.cs` or wherever you're setting the `DataContext`:

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = App.AppHost.Services.GetRequiredService<MainViewModel>();
        DataContext = vm;

        vm.LoadToursCommand.Execute(null);
    }
}
```

---

## 🧪 Using TourService in MainViewModel

### Load Tours

```csharp
public async Task LoadToursAsync()
{
    var tours = await _tourService.GetAllToursAsync();
    Tours = new ObservableCollection<TourDto>(tours);
}
```

### Create a Tour

```csharp
public async Task AddTourAsync(TourDto newTour)
{
    var errors = newTour.Validate();
    if (errors.Count > 0)
    {
        MessageBox.Show(string.Join("\n", errors));
        return;
    }

    await _tourService.CreateTourAsync(newTour);
    await LoadToursAsync(); // refresh
}
```

### Add a Tour Log

```csharp
public async Task AddLogAsync(int tourId, TourLogDto log)
{
    await _tourService.AddTourLogAsync(tourId, log);
    SelectedTour.TourLogs.Add(log);
}
```

### Update or Delete a Log

```csharp
await _tourService.UpdateTourLogAsync(log);
await _tourService.DeleteTourLogAsync(logId);
```

### Get Logs for Tour

```csharp
var logs = await _tourService.GetLogsForTourAsync(tourId);
```

---

✅ This pattern ensures a fully testable, loosely-coupled architecture using `Microsoft.Extensions.DependencyInjection`.