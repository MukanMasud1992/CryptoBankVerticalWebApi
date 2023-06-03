namespace CryptoBankVerticalWebApi.Features.Users.Options;

public class PasswordHashingOptions
{
    public int MemorySize { get; set; }
    public int Iterations { get; set; }
    public int Parallelism { get; set; }
}

