using Discord;
using Discord.Commands;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PhotoBossman.Modules
{
    public class Other : ModuleBase<SocketCommandContext>
    {
        //Owner commands

        [Command("clear")]
        [RequireOwner]
        public async Task ClearImgsAsync()
        {
            await Context.Channel.SendMessageAsync("Clearing images...");

            Process.Start(new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + "/PhotoBossman",
                UseShellExecute = true
            });

            var processes = Process.GetProcessesByName("PhotoBossman");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        [Command("cfu")]
        [RequireOwner]
        public async Task UpdateCheckAsync()
        {
            await Context.Channel.SendMessageAsync("Checking for updates...");
            await Context.Channel.SendMessageAsync("Results have been DM'd to you " + Context.Message.Author.Mention);
            await Context.User.SendMessageAsync("Not added in current version. Check Github for a new release which may have this added.");
        }

        [Command("sd")]
        [Alias("shut-down", "kill", "end")]
        [RequireOwner]
        public async Task EndTaskAsync()
        {
            await Context.Channel.SendMessageAsync("Ending process...");

            var processes = Process.GetProcessesByName("PhotoBossman");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        [Command("complete-request")]
        [Alias("cr")]
        [RequireOwner]
        public async Task AddedSuggestAsync([Remainder] string s)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Added!")
                .WithDescription(s)
                .WithCurrentTimestamp()
                .WithColor(new Color(68, 255, 0));

            var embed = builder.Build();

            await Context.Message.Channel.SendMessageAsync(embed: embed);

            await Context.Message.DeleteAsync();
        }

        //Start of non owner commands
        [Command("help")]
        [Alias("h", "info", "howto", "use")]
        public async Task HelpAsync()
        {
            await Context.Channel.SendMessageAsync("To use Photo Bossman, use w/ {AMOUNT} to stretch the width or h/ {AMOUNT} to stretch the height.\n" +
                "- Make sure to attach an image for the bot to stretch!\n\n" +
                "Use g/ to get the link to a Discord photo.\n" +
                "- If you are using this image as a link, add 'https://' to the begining of the link!\n\n" +
                "Use i/ to invert an attached image.\n\n" +
                "Use gs/ to make an attached image greyscale.\n\n" +
                "FAQ:\n" +
                "1) I can't use !sd, !cfu or !cr?\n- Those are server owner commands. If you are not the server owner, you cannot use them.\n" +
                "2) The bot won't reply with an image!\n- Use !restart or !reset then try again. If still not, use a different amount.\n" +
                "3) Can I download the source code?\n Soon, just going to finish the 1st release, that is also why !cfu is incomplete.");
        }

        [Command("restart")]
        [Alias("reset")]
        public async Task RestartTaskAsync()
        {
            await Context.Message.Channel.SendMessageAsync("Restarting...");

            Process.Start(new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + "/PhotoBossman.exe",
                UseShellExecute = true
            });

            var processes = Process.GetProcessesByName("PhotoBossman");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        [Command("suggest")]
        [Alias("request")]
        public async Task SuggestAsync([Remainder] string s)
        {
            var builder = new EmbedBuilder()
                .WithAuthor(Context.Message.Author)
                .WithTitle("Suggestion!")
                .WithDescription(s)
                .WithCurrentTimestamp()
                .WithFooter("Suggestion for PhotoBossman")
                .WithColor(new Color(122, 180, 192));

            var embed = builder.Build();

            var sent = await Context.Message.Channel.SendMessageAsync(embed: embed);
            await sent.AddReactionAsync(new Emoji("😍"));
            await sent.AddReactionAsync(new Emoji("💩"));

            await Context.Message.DeleteAsync();
        }

        [Command("ver")]
        public async Task VersionAsync()
        {
            await Context.User.SendMessageAsync("Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            await Context.Channel.SendMessageAsync("I have DM'd you " + Context.User.Mention);
        }
    }
}
