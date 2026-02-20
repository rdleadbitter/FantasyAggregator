using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task<int?> CreatePlayerAsync(object player)
    {
        var json = JsonSerializer.Serialize(player);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await client.PostAsync("api/players", content);

        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Create failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
            var body = await resp.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body)) Console.WriteLine(body);
            return null;
        }

        if (resp.Headers.Location != null)
        {
            var loc = resp.Headers.Location.ToString(); // e.g. http://localhost:5196/api/players/123
            var parts = loc.TrimEnd('/').Split('/');
            if (int.TryParse(parts[^1], out var idFromLoc))
            {
                Console.WriteLine($"Created player. ID = {idFromLoc}");
                return idFromLoc;
            }
        }

        var bodyJson = await resp.Content.ReadAsStringAsync();
        try
        {
            using var doc = JsonDocument.Parse(bodyJson);
            if (doc.RootElement.TryGetProperty("playerId", out var pidProp) ||
                doc.RootElement.TryGetProperty("PlayerId", out pidProp))
            {
                if (pidProp.TryGetInt32(out var pid))
                {
                    Console.WriteLine($"Created player. ID = {pid}");
                    return pid;
                }
            }
        }
        catch { /* ignore parsing errors */ }

        // fallback: show body & location
        Console.WriteLine("Created player but could not determine ID.");
        Console.WriteLine("Location: " + resp.Headers.Location);
        Console.WriteLine("Body: " + bodyJson);
        return null;
    }

    static async Task<JsonElement?> GetPlayerAsync(int id)
    {
        var resp = await client.GetAsync($"api/players/{id}");
        if (resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement.Clone();
                return root;
            }
            catch
            {
                Console.WriteLine("GET returned invalid JSON.");
                return null;
            }
        }
        else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null; // indicate NotFound
        }
        else
        {
            Console.WriteLine($"GET failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
            return null;
        }
    }

    static async Task<bool> UpdatePlayerActiveAsync(int id, bool active)
    {
        // GET current object so we preserve fields (or reconstruct if API requires full object)
        var current = await GetPlayerAsync(id);
        if (current == null)
        {
            Console.WriteLine($"Update aborted: player {id} not found.");
            return false;
        }

        // Build update object with required fields. Adjust property names if your API uses different casing.
        var updateObj = new
        {
            PlayerId = id,
            FullName = current.Value.GetProperty("fullName").GetString() ?? current.Value.GetProperty("FullName").GetString(),
            Position = current.Value.TryGetProperty("position", out var p1) ? p1.GetString() :
                       (current.Value.TryGetProperty("Position", out var p2) ? p2.GetString() : null),
            TeamAbbrev = current.Value.TryGetProperty("teamAbbrev", out var t1) ? t1.GetString() :
                         (current.Value.TryGetProperty("TeamAbbrev", out var t2) ? t2.GetString() : null),
            Active = active
        };

        var json = JsonSerializer.Serialize(updateObj);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await client.PutAsync($"api/players/{id}", content);
        if (resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Updated player {id}: Active = {active}");
            return true;
        }
        else
        {
            Console.WriteLine($"Update failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
            var body = await resp.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body)) Console.WriteLine(body);
            return false;
        }
    }

    static async Task<bool> DeletePlayerAsync(int id)
    {
        var resp = await client.DeleteAsync($"api/players/{id}");
        if (resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Deleted player {id}");
            return true;
        }
        else
        {
            Console.WriteLine($"Delete failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
            var body = await resp.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body)) Console.WriteLine(body);
            return false;
        }
    }

    static string GetActiveFromJson(JsonElement el)
    {
        if (el.TryGetProperty("active", out var a) && a.ValueKind == JsonValueKind.True) return "True";
        if (el.TryGetProperty("active", out var a2) && a2.ValueKind == JsonValueKind.False) return "False";
        if (el.TryGetProperty("Active", out var a3) && a3.ValueKind == JsonValueKind.True) return "True";
        if (el.TryGetProperty("Active", out var a4) && a4.ValueKind == JsonValueKind.False) return "False";
        return "(unknown)";
    }

    static async Task<int> Main()
    {
        // set base address to the API's HTTP url printed by dotnet run
        client.BaseAddress = new Uri("http://localhost:5196/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Console.WriteLine("Demo: Create -> GET -> Update(active=false) -> GET -> Delete -> GET\n");

        var tom = new
        {
            FullName = "Tom Brady",
            Position = "QB",
            TeamAbbrev = "TB",
            Active = true
        };

        // CREATE
        var id = await CreatePlayerAsync(tom);
        if (id == null)
        {
            Console.WriteLine("Aborting demo due to create failure.");
            return 1;
        }

        // GET after create
        var afterCreate = await GetPlayerAsync(id.Value);
        if (afterCreate == null)
        {
            Console.WriteLine("After create: NotFound");
        }
        else
        {
            Console.WriteLine($"After create: Active = {GetActiveFromJson(afterCreate.Value)}");
        }

        // UPDATE -> Active = false
        var updated = await UpdatePlayerActiveAsync(id.Value, false);
        if (!updated)
        {
            Console.WriteLine("Aborting demo due to update failure.");
            return 2;
        }

        // GET after update
        var afterUpdate = await GetPlayerAsync(id.Value);
        if (afterUpdate == null)
        {
            Console.WriteLine("After update: NotFound");
        }
        else
        {
            Console.WriteLine($"After update: Active = {GetActiveFromJson(afterUpdate.Value)}");
        }

        // DELETE
        var deleted = await DeletePlayerAsync(id.Value);
        if (!deleted)
        {
            Console.WriteLine("Demo finished with errors.");
            return 3;
        }

        // GET after delete to confirm removal
        var afterDelete = await GetPlayerAsync(id.Value);
        if (afterDelete == null)
        {
            Console.WriteLine("After delete: NotFound");
        }
        else
        {
            Console.WriteLine("After delete: still exists (unexpected)");
        }

        Console.WriteLine("\nDemo complete.");
        return 0;
    }
}
