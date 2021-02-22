using Domain.Enums;


namespace Domain.DomainModel
{
    public class UsedDaysStatistic
    {
        public int Id { get; set; }
        public VacationType Type { get; set; }
        public int NumberDaysUsed { get; set; }
        public string Year { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }
    }
}
