public class LoginAttempt
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Username { get; set; }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public DateTime AttemptTime { get; set; }
}