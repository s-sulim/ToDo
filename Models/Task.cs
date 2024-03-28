namespace ToDo.Models
{
    public class Task
    {
        public enum TaskPriority
        {
            High = 0,
            Medium = 1,
            Low = 2
        }
        public enum TaskState
        {
            ReadyToStart  = 0,
            Doing = 1,
            Done = 2,
            Backlog = 3
        }
        public enum TaskType
        {
            Housework = 0,
            Business = 1,
            SelfImprovement = 2,
            Health = 3
        }
        public class Substep
        {
            public int Id { get; set; } 
            public bool IsDone { get; set; }
            public string? Text { get; set; }
            public int TaskId { get; set; } 
            public bool MarkedForDeletion { get; set; }
        }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public List<Substep>? Substeps { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskState State { get; set; }
        public TaskType Type { get; set; }
        public string? CreatedUserName { get; set; }
        public string? AssignedUserName { get; set; }
        public string? FinishedUserName { get; set; }
    }
}