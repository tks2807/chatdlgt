using ChatClient.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using ChatClient;

namespace ChatClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Please enter your name: ");
            var username = Console.ReadLine();
            static void IMessageGot(object sender, ChatEventArgs cea)
            {
                Console.WriteLine($"{cea.Username} at {cea.Date}");
            }
            ChatEventHandler ceh = new ChatEventHandler();
            ceh.MessageGot += IMessageGot;
            

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ChatService.ChatServiceClient(channel);


            using (var chat = client.Join())
            {
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext())
                    {
                        var response = chat.ResponseStream.Current;
                        Console.WriteLine($"{response.User} : {response.Text}");
                        ceh.ChatEvent(chat);
                    }
                });

                var history = client.GetAll(new Empty());
                using (var cht = client.GetAll(new Empty()))
                {
                    _ = Task.Run(async () =>
                    {
                        while (await cht.ResponseStream.MoveNext())
                        {
                            var response = cht.ResponseStream.Current;
                            Console.WriteLine($"{response.User}: {response.Text}");
                        }
                    });

                    await chat.RequestStream.WriteAsync(new Message { User = username, Text = $"{username} has joined the chat!" });

                    string line;

                    while ((line = Console.ReadLine()) != null)
                    {
                        if (line.ToUpper() == "EXIT")
                        {
                            break;
                        }
                        await chat.RequestStream.WriteAsync(new Message { User = username, Text = line });
                        await client.InsertAsync(new Message { User = username, Text = line });
                    }

                    await chat.RequestStream.CompleteAsync();
                }

                Console.WriteLine("Disconnection started!");
                await channel.ShutdownAsync();
            }
        }
    }
}
