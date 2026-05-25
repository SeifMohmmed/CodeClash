namespace CodeClash.Domain.Premitives.Responses;
public sealed class AcceptedResponse : BaseSubmissionResponse
{
    public decimal ExecutionTime { get; set; }

    public AcceptedResponse()
    {

    }

    public AcceptedResponse(BaseSubmissionResponse baseResponse)
    {
        SubmissionDate = baseResponse.SubmissionDate;
        SubmissionResult = baseResponse.SubmissionResult;
        TotalTestcases = baseResponse.TotalTestcases;
        NumberOfPassedTestCases = baseResponse.NumberOfPassedTestCases;
    }
}
