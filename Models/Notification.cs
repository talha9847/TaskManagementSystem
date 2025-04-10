namespace MyApp.Namespace;
public class Notification
{
    public string Id { get; set; }
    public string Action { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }

    public static Notification FromString(string str)
        {
            var parts = str.Split('|');
            return new Notification
            {
                Id = parts[0],
                Action = parts[1],
                Message = parts[2],
                Timestamp = DateTime.Parse(parts[3])
            };
        }

        public override string ToString()
        {
            return $"{Id}|{Action}|{Message}|{Timestamp}";
        }

}