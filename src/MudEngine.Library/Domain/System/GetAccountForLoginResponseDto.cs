﻿namespace MudEngine.Library.Domain.System;

public class GetAccountForLoginResponseDto
{
    public Guid AccountId { get; set; }
    public string? HashedPassword { get; set; }
}