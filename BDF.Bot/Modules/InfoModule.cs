using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BDF.Bot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Alias("about")]
        public async Task InfoAsync()
        {
            var app = await Context.Client.GetApplicationInfoAsync();

            await ReplyAsync(
                $"{Format.Bold("Info")}\n" +
                $"- Author: {app.Owner} ({app.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture} " +
                $"({RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture})\n" +
                $"- Uptime: {GetUptime()}\n\n" +

                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {GetHeapSize()}MiB\n" +
                $"- Guilds: {Context.Client.Guilds.Count}\n" +
                $"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}\n" +
                $"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}\n");
        }

        [Command("help")]
        [Alias("commands")]
        public async Task HelpAsync()
        {

            await ReplyAsync(
                $"{Format.Bold("Available Commands:")}\n" +
                $"- !info: Displays bot info.\n" +
                $"- !help: Lists available commands.\n" +
                $"- !cat: Returns cat picture.\n" +
                $"- !echo {Format.Italics("<msg>")}: Makes the bot say something.\n" +
                $"- !ping: Test latency.\n" +
                $"- !clean {Format.Italics("<amount>")}: Deletes the specified amount of messages.");

        }

        private static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
