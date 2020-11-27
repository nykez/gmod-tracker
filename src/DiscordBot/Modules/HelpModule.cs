using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace DiscordBot.Modules
{
    [Name("Help Commands")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;


        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("Prints a list of commands to use")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description =  "These are the commands you can use"
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);

                    if (result.IsSuccess)
                    {
                        // add any parameters if it has some
                        if (cmd.Parameters.Any())
                        {
                            description += $"~{string.Join(", ", cmd.Aliases.ToArray())}";
                            description += $" (params: {string.Join(", ", cmd.Parameters)})\n";
                        }
                        else
                        // no params -> just print alias and move on
                            description += $"~{string.Join(", ", cmd.Aliases.ToArray())}\n";
                        
                    }
                    
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());

        }

        [Command("help")]
        [Summary("Prints out a description of the given command (if valid)")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }

    }

}