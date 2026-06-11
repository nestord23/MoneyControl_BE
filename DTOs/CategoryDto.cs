namespace MoneyControl.DTOs;

public record CreateCategoryRequest(string Name, string? Description, string Type);

public record UpdateCategoryRequest(string Name, string? Description, string Type);

public record CategoryResponse(int Id, string Name, string? Description, string Type, DateTime CreatedAt);
