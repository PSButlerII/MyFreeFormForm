public class FormStatisticsViewModel
{
    public List<FormStatistic> Statistics { get; set; } = new List<FormStatistic>();

    public class FormStatistic
    {
        public string FormName { get; set; }
        public int Count { get; set; }
    }
}
