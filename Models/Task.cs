namespace MyApp.Namespace;
public class Tasks
{
    public int c_TaskId { get; set; }
    public int c_EmpId { get; set; } // Ensure this is int if it's an integer in the database
    public string c_Title { get; set; }
    public string c_Description { get; set; }
    public int c_EstimateDays { get; set; }
    public DateTime c_startDate { get; set; }
    public DateTime c_endDate { get; set; }
    public string c_Status { get; set; }
}