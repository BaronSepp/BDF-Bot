using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bot.Modules;

public class ChatModule : ModuleBase<SocketCommandContext>
{

	[Command("ping")]
	public async Task PingAsync()
	{
		await ReplyAsync("pong!");
	}


	[Command("echo")]
	public async Task EchoAsync([Remainder] string text)
	{
		await ReplyAsync('\u200B' + text);
	}

	[Command("clean", RunMode = RunMode.Async)]
	[Alias("del", "rm")]
	[RequireUserPermission(GuildPermission.ManageMessages)]
	[RequireBotPermission(ChannelPermission.ManageMessages)]
	public async Task CleanChatAsync(int amount)
	{
		var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
		await ((SocketTextChannel)Context.Channel)?.DeleteMessagesAsync(messages);
		var m = await ReplyAsync($"Deleted {amount} messages.");
		await Task.Delay(2000);
		await m.DeleteAsync();
	}
}