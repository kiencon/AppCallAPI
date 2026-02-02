namespace Contracts;

public enum Lang
{
    EN,
    FR
}

public sealed record SubmitLeaveRequestCommand(
    string UserId,
    DateOnly StartDate,
    DateOnly EndDate,
    Lang Language,
    string ClientVersion
);

public sealed record SubmitLeaveRequestResult(
    Guid RequestId,
    string CorrelationId
);

public sealed record PrintLeavePdfCommand(
    Guid RequestId,
    string UserId,
    string ClientVersion
);

public sealed record PrintLeavePdfResult(
    Guid RequestId,
    Lang RequestLanguage,
    Lang PdfLanguage,
    string PdfPath,
    string CorrelationId
);
