namespace EncorePosApiFaker.Dto;

public record ApiResponseDto
{
    public bool Success { get; set; }
    public long Id { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string>? FaultyParameters { get; set; }
    public object? Datas { get; set; }
}