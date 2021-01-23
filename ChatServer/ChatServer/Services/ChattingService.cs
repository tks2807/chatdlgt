using ChatServer.Context;
using ChatServer.Models;
using ChatServer.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatProject.Services
{
    public class ChattingService : ChatService.ChatServiceBase
    {
        private readonly ChatRoom _chatRoomService;
        private readonly DataContext _context;

        public ChattingService(ChatRoom chatRoomService, DataContext context)
        {
            _chatRoomService = chatRoomService;
            _context = context;
        }

        public override async Task Join(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;

            do {
                _chatRoomService.Join(requestStream.Current.User, responseStream);
                await _chatRoomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatRoomService.Remove(context.Peer);
        }
        public override Task<Messages> GetAll(Empty requestData, ServerCallContext context)
        {
            Messages data = new Messages();
            var query = from hst in _context.Histories
                        select new Message()
                        {
                            ChatId = hst.Id,
                            User = hst.Username,
                            Text = hst.Text,
                        };
            data.Items.AddRange(query.ToArray());
            return Task.FromResult(data);
        }

        public async override Task<Empty> Insert(Message requestData, ServerCallContext context)
        {
            await _context.Histories.AddAsync(new History()
            {
                Id = requestData.ChatId,
                Username = requestData.User,
                Text = requestData.Text
            });
            _context.SaveChanges();
            return await Task.FromResult(new Empty());
        }
    }
}
