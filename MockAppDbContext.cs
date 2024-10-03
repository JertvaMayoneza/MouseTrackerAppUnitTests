using Microsoft.EntityFrameworkCore;
using MouseTrakerApp.Data;

public static class MockAppDbContext
{
    public static AppDbContext Create()
    {
      
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Ќазвание базы данных дл€ тестировани€
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
