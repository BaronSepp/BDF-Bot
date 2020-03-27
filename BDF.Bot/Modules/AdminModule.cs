using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BDF.Bot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("clean", RunMode = RunMode.Async)]
        [Summary("Deletes the specified amount of messages.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CleanChatAsync(int amount)
        {
            var messages = await this.Context.Channel.GetMessagesAsync(amount +1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            var m = await ReplyAsync($"Deleted {amount} messages.");
            await Task.Delay(2000);
            await m.DeleteAsync();
        }
    }

}
