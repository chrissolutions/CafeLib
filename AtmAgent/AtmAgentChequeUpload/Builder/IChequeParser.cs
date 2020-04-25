namespace AtmAgentChequeUpload.Builder
{
    public interface IChequeParser
    {
        ChequeFileInfo Parse(string fileName);
    }
}
