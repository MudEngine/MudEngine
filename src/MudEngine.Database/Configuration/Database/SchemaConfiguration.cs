using System.Data;
using System.Reflection;
using System.Text;
using Dapper;
using Serilog;
namespace MudEngine.Database.Configuration.Database;

public static class SchemaConfiguration
{
    public static void Seed(this IHost host, string folderName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceFiles = assembly.GetManifestResourceNames().ToList();
        var prefix = $"{assembly.GetName().Name}.Seeding.{folderName}";
        var sqlFiles = resourceFiles
            .Where(x => x.StartsWith(prefix)
                        && x.EndsWith(".sql"))
            .OrderBy(x => x).ToList();
        var connectionFactory = host.Services.GetRequiredService<Func<IDbConnection>>();
        foreach (var sqlFile in sqlFiles)
        {
            var sqlFileName = "[" + sqlFile.Substring(prefix.Length + 1, sqlFile.Length - prefix.Length - 5)
                .Replace(".", "].[") + "]";
            try
            {
                Log.Warning("Seeding: {sqlFileName}", sqlFileName);
                using var stream = assembly.GetManifestResourceStream(sqlFile)!;
                using var reader = new StreamReader(stream);
                var sqlText = reader.ReadToEnd();
                using var connection = connectionFactory.Invoke();
                connection.Open();
                _ = connection.Execute(sqlText);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "SeedSqlFile: {sqlFileName}", sqlFileName);
            }
        }
    }
    public static void SeedEnum(this IHost host, Type enumType)
    {
        var enumEntries = Enum.GetNames(enumType).ToDictionary(name => (int) Enum.Parse(enumType, name));
        if (enumEntries.Count == 0 || string.IsNullOrWhiteSpace(enumType.FullName))
        {
            return;
        }
        var enumName =
            $"[{string.Join("].[", enumType.FullName.Split('.', StringSplitOptions.RemoveEmptyEntries).TakeLast(2))}]"
                .Replace("[Enums]", "[Enum]");
        try
        {
            Log.Warning("Seeding Enum: {enumName}", enumName);
            var sqlTextBuilder = new StringBuilder($"SET IDENTITY_INSERT {enumName} ON\r\n");
            foreach (var enumEntry in enumEntries)
            {
                sqlTextBuilder.AppendLine(
                    $"IF EXISTS (SELECT * FROM {enumName} WHERE {enumType.Name}Id = {enumEntry.Key}) "
                    + $"UPDATE {enumName} SET [Name] = '{enumEntry.Value}' WHERE {enumType.Name}Id = {enumEntry.Key} "
                    + $"ELSE INSERT INTO {enumName} ({enumType.Name}Id, [Name]) VALUES ({enumEntry.Key}, '{enumEntry.Value}')");
            }
            sqlTextBuilder.AppendLine(
                $"DELETE FROM {enumName} WHERE {enumType.Name}Id > {enumEntries.Max(e => e.Key)}");
            sqlTextBuilder.AppendLine($"SET IDENTITY_INSERT {enumName} OFF");
            var sqlText = sqlTextBuilder.ToString();
            var connectionFactory = host.Services.GetRequiredService<Func<IDbConnection>>();
            using var connection = connectionFactory.Invoke();
            connection.Open();
            _ = connection.Execute(sqlText);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "SeedEnum: {enumName}", enumName);
        }
    }
    public static void SeedSchema(this IHost host, string schema)
    {
        try
        {
            Log.Warning("Seeding Schema: {schema}", "[" + schema + "]");
            var connectionFactory = host.Services.GetRequiredService<Func<IDbConnection>>();
            using var connection = connectionFactory.Invoke();
            connection.Open();
            _ = connection.Execute(
                $"IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE [name] = N'{schema}') EXEC('CREATE SCHEMA [{schema}] AUTHORIZATION [dbo]')");
        }
        catch (Exception exception)
        {
            Log.Warning(exception, "SeedSchema: {schema}", "[" + schema + "]");
        }
    }
}