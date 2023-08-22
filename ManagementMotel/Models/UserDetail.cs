namespace ManagementMotel.Models
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CCCD { get; set; }
        public string Address { get; set; }
        public string MobilePhone { get; set; }
        public string ImagePreviousCCCD { get; set; }
        public string ImageAfterCCCD { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
