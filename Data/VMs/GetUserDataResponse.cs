namespace Data.VMs
{
    public class GetUserDataResponse
    {

        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }

        public string? Token { get; set; }

    }
}
