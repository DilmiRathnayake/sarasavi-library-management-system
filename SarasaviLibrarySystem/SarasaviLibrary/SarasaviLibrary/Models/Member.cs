namespace SarasaviLibrary.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{MemberID} | {FullName} | {Email} | {Phone} | Active: {IsActive}";
        }
    }
}
