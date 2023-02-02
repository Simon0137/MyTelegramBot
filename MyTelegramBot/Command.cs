namespace MyTelegramBot
{
    public class Command
    {
        public string Text { get; set; }
        public Func<Task> CommandExecutor { get; set; }

        public void Execute()
        {
            CommandExecutor.Invoke();
        }

        public Command (string text, Func<Task> commandExecutor)
        {
            this.Text = text;
            this.CommandExecutor = commandExecutor;
        }
    }
}
