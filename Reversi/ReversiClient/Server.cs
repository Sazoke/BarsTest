using System;
using System.Net.Http;
using System.Text.Json;
using ReversiClient.Models;

namespace ReversiClient
{
    public class Server
    {
        private HttpClient _client;
        private string _url;
        private int _id;

        public Server()
        {
            _id = -1;
            _client = new HttpClient();
            _url  = "https://localhost:5001/Game";
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_url+$"?userId={_id}"));
            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            var message = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var gameDto = JsonSerializer.Deserialize<GameDto>(message);
            _id = gameDto.playerId;
        }

        public GameDto GetUpdate()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_url+$"?userId={_id}"));
            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            var message = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var dto = JsonSerializer.Deserialize<GameDto>(message);
            return dto;
        }

        public void SetUpdate(GameChange gameChange)
        {
            var json = JsonSerializer.Serialize(gameChange);
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_url+$"?json={json}"));
            _client.SendAsync(request).GetAwaiter().GetResult();
        }
    }
}