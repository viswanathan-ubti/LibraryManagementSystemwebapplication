namespace LibraryManagementSystem.Models
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Message);

        public ErrorViewModel(string message = null)
        {
            Message = message;
        }
    }
}