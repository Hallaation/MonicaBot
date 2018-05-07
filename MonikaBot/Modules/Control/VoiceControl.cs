using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonikaBot.Modules.Control
{

    public class VoiceControl : BaseModule
    {
        //Define constants
        private Dictionary<ulong, SocketVoiceChannel> userLockedChannels = new Dictionary<ulong, SocketVoiceChannel>();

        public VoiceControl(DiscordSocketClient client)
            : base(client)
        {
        }
        public async Task ExecuteAsync()
        {
            Console.WriteLine("Executed Async");
            await Task.CompletedTask;
        }
        //wait. lets not remake the RNG duel.
        //Lets make an ACTUAL duel with inventory systems etc. hmm
        //how do I go about this
        //[Command("Duel", RunMode = RunMode.Async)]
        public async Task DuelAsync([Remainder] SocketUser duelTarget)
        {
            await ReplyAsync("Dueling " + duelTarget.Mention);
            await ReplyAsync(_client.CurrentUser.Mention);

            await Task.CompletedTask;
        }

        //Move member command to move members from one voice channel to another
        //Lets hope the socketvoicechannel can take an ID as a source and target channel
        [Command("MoveMembers", RunMode = RunMode.Async)]
        public async Task MoveMembers([Remainder] IVoiceChannel targetChannel)
        {
            //Get the user the command caller is in
            IReadOnlyCollection<SocketGuildUser> users = Context.Guild.GetUser(Context.User.Id).VoiceChannel.Users;
            foreach (var user in users)
            {
                
            }
        }

        //Should test if overloaded functions with commands work. but this should work theoretically
        [Command("MoveMembers", RunMode = RunMode.AsyncS)]
        public async Task MoveMembers(IVoiceChannel sourceChannel, IVoiceChannel targetChannel)
        {


        }


        /*
        //disable this command to be sure.
        //[Command("RequestVoice", RunMode = RunMode.Async)]
        public async Task RequestAsync()
        {
            try
            {
                _client.MessageReceived -= RequestReplyAsync;
                _client.MessageReceived += RequestReplyAsync;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (!RequestUsers.Contains(Context.User))
            {
                RequestUsers.Add(Context.User);
            }
            RestVoiceChannel channel = Context.Guild.CreateVoiceChannelAsync("helpme").Result;
            Console.WriteLine("---------");
            try
            {
                OverwritePermissions testingtestig = new OverwritePermissions();
                testingtestig = testingtestig.Modify(connect: PermValue.Deny);
                await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, testingtestig);
                Console.WriteLine(channel.PermissionOverwrites);
                //overwritepermissions.
                //channel.AddPermissionOverwriteAsync(Context.User);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("---------");
            await ReplyAsync("Added " + Context.Guild.GetUser(Context.User.Id) + " to request list");
        }
        

        public async Task RequestReplyAsync(SocketMessage arg)
        {
            if (!Context.User.IsBot) //if the user in context isn't a bot
            {
                if (RequestUsers.Contains(Context.User))
                {

                }
            }
            await Task.CompletedTask;
        }
        */

        [Command("Lock", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild)]
        public async Task LockVoiceAsync()
        {
            if (!userLockedChannels.ContainsKey(Context.User.Id))
            {
                SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);
                SocketVoiceChannel channel = user.VoiceChannel;

                userLockedChannels.Add(Context.User.Id, channel); //add them to the list
                Console.WriteLine("---------");

                try
                {
                    //Setup 2 types of permissions, 1 to allow the current users, the other to block everyone else
                    OverwritePermissions allowedUser = new OverwritePermissions();
                    allowedUser = allowedUser.Modify(connect: PermValue.Allow);

                    OverwritePermissions deniedUsers = new OverwritePermissions();
                    deniedUsers = deniedUsers.Modify(connect: PermValue.Deny);

                    //Block everyone else
                    await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, deniedUsers);

                    //find every connected voice user and add them to the list
                    foreach (var channelUsers in channel.Users)
                    {
                        await channel.AddPermissionOverwriteAsync(channelUsers, allowedUser);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine("---------");

            }

            await Task.CompletedTask;
        }

        //removes any permissions for the current context voice channel
        [Command("Unlock", RunMode = RunMode.Async)]
        public async Task UnlockAsync()
        {
            if (userLockedChannels.ContainsKey(Context.User.Id))
            {
                IVoiceChannel channel = userLockedChannels[Context.User.Id];
                foreach (var item in channel.PermissionOverwrites)
                {
                    Console.WriteLine(item.TargetId);
                    await channel.RemovePermissionOverwriteAsync(Context.Guild.GetUser(item.TargetId));
                }
                OverwritePermissions resetPermission = new OverwritePermissions();
                resetPermission.Modify(connect: PermValue.Inherit);

                await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, resetPermission);
                userLockedChannels.Remove(Context.User.Id);
            }
            else
            {
                Console.WriteLine("User doesn't have any locked channels");
            }
            await Task.CompletedTask;
        }

        //TODO find a way to handle multiple users efficiently
        [Command("AllowUser", RunMode = RunMode.Async)]
        public async Task AllowUserAsync(params SocketGuildUser[] targetUsers)
        {
            await ReplyAsync(userLockedChannels.Count.ToString());
            //Check if the user has locked any channels
            //if (userLockedChannels.ContainsKey(Context.User.Id))
            //{
            Console.WriteLine("Allowing User");
            SocketVoiceChannel channel = userLockedChannels[Context.User.Id];

            //Prepare the permissions
            OverwritePermissions perms = new OverwritePermissions();
            perms = perms.Modify(connect: PermValue.Allow);
            try
            {
                foreach (var user in targetUsers)
                {
                    await channel.AddPermissionOverwriteAsync(user, perms);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //}
            await Task.CompletedTask;
        }

        [Command("TestStrings")]
        public async Task StringTest(string test, [Remainder] string stringArray)
        {
            await ReplyAsync(test);
            await ReplyAsync(stringArray);
        }
    }


}
