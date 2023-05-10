namespace backend.Services.Interfaces
{
    public interface IHashService
    {
        public string HashString(string text);
        public bool VerifyString(string? text, string? hashedText);
    }
}