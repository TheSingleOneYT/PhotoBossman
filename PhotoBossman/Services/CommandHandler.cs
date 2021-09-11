using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoBossman.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient client;
        private readonly CommandService service;
        private readonly IConfiguration configuration;

        Bitmap bmp;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration)
        {
            this.provider = provider;
            this.service = service;
            this.client = client;
            this.configuration = configuration;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived += OnMessageReceived;
            this.service.CommandExecuted += OnCommandExecuted;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }

        private async Task OnCommandExecuted(Discord.Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (result.IsSuccess)
                return;

            if (result.ErrorReason == "Unknown command.")
            {
                return;
            }

            await commandContext.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != Discord.MessageSource.User) return;

            if (message.Content.StartsWith("h/"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(message.Author.Mention + ", preparing image...");

                await message.Channel.TriggerTypingAsync();

                var amount = Int32.Parse(message.Content.Remove(0, 3));

                var url = message.Attachments.FirstOrDefault().Url;

                var wc = new WebClient();
                wc.DownloadFile(url, Directory.GetCurrentDirectory() + "/img.png");

                Image img = Image.FromFile(Directory.GetCurrentDirectory() + "/img.png");
                int orgHeight = img.Height;

                bmp = new Bitmap(img.Width, orgHeight * amount);

                Resize(img, img.Width, orgHeight * amount);

                await message.Channel.SendFileAsync(Directory.GetCurrentDirectory() + "/output.png", message.Author.Mention);

                File.Delete(Directory.GetCurrentDirectory() + "/output.png");

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
            else if (message.Content.StartsWith("w/"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(message.Author.Mention + ", preparing image...");

                await message.Channel.TriggerTypingAsync();

                var amount = Int32.Parse(message.Content.Remove(0, 3));

                var url = message.Attachments.FirstOrDefault().Url;

                var wc = new WebClient();
                wc.DownloadFile(url, Directory.GetCurrentDirectory() + "/img.png");

                Image img = Image.FromFile(Directory.GetCurrentDirectory() + "/img.png");
                int orgHeight = img.Height;

                bmp = new Bitmap(img.Width * amount, orgHeight);

                Resize(img, img.Width * amount, orgHeight);

                await message.Channel.SendFileAsync(Directory.GetCurrentDirectory() + "/output.png", message.Author.Mention);

                File.Delete(Directory.GetCurrentDirectory() + "/output.png");

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
            else if (message.Content.StartsWith("g/"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(message.Author.Mention + ", generating link...");

                await message.Channel.TriggerTypingAsync();

                var link = message.Attachments.FirstOrDefault().Url.Remove(0, 8);

                await message.Channel.SendMessageAsync(link + "\n(Add https:// to the beginning)");
            }
            else if (message.Content.StartsWith("i/"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(message.Author.Mention + ", preparing image...");

                await message.Channel.TriggerTypingAsync();

                var url = message.Attachments.FirstOrDefault().Url;

                var wc = new WebClient();
                wc.DownloadFile(url, Directory.GetCurrentDirectory() + "/img.png");

                Image img = Image.FromFile(Directory.GetCurrentDirectory() + "/img.png");

                Invert(img);

                await message.Channel.SendFileAsync(Directory.GetCurrentDirectory() + "/output.png", message.Author.Mention);

                File.Delete(Directory.GetCurrentDirectory() + "/output.png");

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
            else if (message.Content.StartsWith("gs/"))
            {
                await message.Channel.TriggerTypingAsync();

                await message.Channel.SendMessageAsync(message.Author.Mention + ", preparing image...");

                await message.Channel.TriggerTypingAsync();

                var url = message.Attachments.FirstOrDefault().Url;

                var wc = new WebClient();
                wc.DownloadFile(url, Directory.GetCurrentDirectory() + "/img.png");

                Image img = Image.FromFile(Directory.GetCurrentDirectory() + "/img.png");

                gs(img);

                await message.Channel.SendFileAsync(Directory.GetCurrentDirectory() + "/output.png", message.Author.Mention);

                File.Delete(Directory.GetCurrentDirectory() + "/output.png");

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
            else
            {
                int argPos = 0;

                if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos)) return;

                var context = new SocketCommandContext(this.client, message);
                await this.service.ExecuteAsync(context, argPos, this.provider);
            }
        }

        Image Resize(Image image, int width, int height)
        {
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(image, 0, 0, width, height);
            graphic.Dispose();

            bmp.Save(Directory.GetCurrentDirectory() + "/output.png", ImageFormat.Png);

            return bmp;
        }

        Bitmap newBitmap;

        Image Invert(Image image) // from: https://youtu.be/AqRepVUMFs4
        {
            newBitmap = new Bitmap(image);
            for (int x = 0; x < newBitmap.Width; x++)
            {
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    Color pixel = newBitmap.GetPixel(x, y);

                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;

                    newBitmap.SetPixel(x, y, Color.FromArgb(255 - red, 255 - green, 255 - blue));
                }

            }
            newBitmap.Save(Directory.GetCurrentDirectory() + "/output.png", ImageFormat.Png);
            return newBitmap;
        }

        Image gs(Image image) // from: https://youtu.be/Ww1BAL8Yh_Y
        {
            newBitmap = new Bitmap(image);
            for (int x = 0; x < newBitmap.Width; x++)
            {
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    Color original = newBitmap.GetPixel(x, y);

                    int greyScale = (int)((original.R * .3) + (original.G * .59) + (original.B * .11));

                    Color newcolor = Color.FromArgb(greyScale, greyScale, greyScale);

                    newBitmap.SetPixel(x, y, newcolor);
                }
            }
            newBitmap.Save(Directory.GetCurrentDirectory() + "/output.png", ImageFormat.Png);
            return newBitmap;
        }
    }
}