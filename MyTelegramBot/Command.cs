namespace MyTelegramBot
{
    public class Command
    {
        public string Text { get; set; }
        public Action CommandExecutor { get; set; }

        public void Execute()
        {
            CommandExecutor.Invoke();
        }

        public Command (string text, Action commandExecutor)
        {
            this.Text = text;
            this.CommandExecutor = commandExecutor;
        }
    }
}
