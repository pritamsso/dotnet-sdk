namespace JwtRevoke
{
    public class RevokedToken
    {
        public string Id { get; set; }
        public string JwtId { get; set; }
        public string Reason { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RevokedByEmail { get; set; }
    }

    public class RevokedTokenList
    {
        public List<RevokedToken> Data { get; set; }
    }

    public class RevokeRequest
    {
        public string JwtId { get; set; }
        public string Reason { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class RevokeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public RevokedToken Token { get; set; }
    }
} 