using Discord;
using Discord.WebSocket;
using System.Diagnostics;
namespace AlphaTTS
{
    public sealed class Program
    {
        DiscordSocketClient _client;
        //Sealed means class can't be inherited
        public static async Task Main()
        {
            //Static: Inheritor will not create an instance, It will do Program.Main()
            var program = new Program();
            await program.StartAsync();
        }
        private async Task StartAsync()
        {
            // _client = new DiscordSocketClient(new DiscordSocketConfig()
            _client = new(new()
            {
                LogLevel = LogSeverity.Info
            });
            _client.Log += Log;
            _client.Ready += ReadyAsync;
            _client.SlashCommandExecuted += SlashCommandExecutedAsync;
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.StartAsync();
            await Task.Delay(-1); // Wait forever 
        }
        private Task Log(LogMessage msg)
        {
            // we'll quickly go on it
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
        private async Task SlashCommandExecutedAsync(SocketSlashCommand cmd)
        {
            var cmdName = cmd.CommandName.ToUpperInvariant();
            if (cmdName == "PING")
            {
                await cmd.RespondAsync("Pong!");
            }
        }
        private Task ReadyAsync()
        {
            // _ means variable is not gonna be used, so compiler stops whining
            _ = Task.Run(async () => {
                var builder = new[]
                {
                    new SlashCommandBuilder()
                    {
                        Name = "ping",
                        Description = "Pings Alpha"
                    }
                }.Select(x => x.Build()).ToArray();
                
                foreach (var command in builder)
                {
                    if (Debugger.IsAttached)
                    {
                        await _client.GetGuild(1237296137774567474).CreateApplicationCommandAsync(command);
                    }
                    else
                    {
                        await _client.CreateGlobalApplicationCommandAsync(command);
                    }
                }
                if (Debugger.IsAttached)
                {
                    await _client.GetGuild(1237296137774567474).BulkOverwriteApplicationCommandAsync(builder);
                }
                else
                {
                    await _client.GetGuild(1237296137774567474).DeleteApplicationCommandsAsync();
                    await _client.BulkOverwriteGlobalApplicationCommandsAsync(builder);
                }
            });
            return Task.CompletedTask;
        }
    }
}