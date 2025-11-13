namespace neighborhood_collab.DTOs.ComplaintDTOs
{
    public class ComplaintVoteCreateDto
    {
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public bool IsLike { get; set; } // true = like, false = dislike
    }

    // ✅ DTO for showing vote info in responses
    public class ComplaintVoteResponseDto
    {
        public int VoteId { get; set; }
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public bool IsLike { get; set; }
        public string? UserName { get; set; }  // optional (if you want to show who voted)
        public DateTime CreatedAt { get; set; }
    }
}
