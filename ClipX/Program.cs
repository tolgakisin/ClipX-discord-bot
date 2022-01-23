using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClipX.Enums;
using ClipX.Infrastructure.Repositories;
using ClipX.Models;
using ClipX.Utils;

namespace ClipX
{
    class Program
    {
        private static HttpClient _httpClient;
        public static DiscordClient _discordClient;
        private static IRedisRepository _redisRepository;
        private static SettingsModel AppSettings;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AppSettings = config.Get<SettingsModel>();

            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions { EndPoints = { AppSettings.GeneralSettings.RedisEndpoint } });
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<HttpClient>()
                .AddSingleton<IConnectionMultiplexer>(multiplexer)
                .AddScoped<IRedisRepository, RedisRepository>()
                .BuildServiceProvider();

            _httpClient = provider.GetService<HttpClient>();
            _redisRepository = provider.GetService<IRedisRepository>();
            _discordClient = new DiscordClient(new DiscordConfiguration() { Token = AppSettings.DiscordSettings.SecretToken, TokenType = TokenType.Bot });

            try
            {
                MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async static Task MainAsync()
        {
            string period = ClipPeriod.Week.Value;
            string getClipsRequestUri = $"{Constants.TwitchConstants.API_URL}/kraken/clips/top?channel={AppSettings.TwitchSettings.ChannelName}&period={period}&trending=true";

            var auth = await AuthenticateAsync();
            var user = await GetUserAsync(auth);

            await _discordClient.ConnectAsync();
            var channel = await _discordClient.GetChannelAsync(AppSettings.DiscordSettings.ChannelId);
            while (true)
            {
                try
                {
                    var clips = await GetClipsAsync(auth, user.Id);
                    var clipsToShare = clips.Where(x => !_redisRepository.AnyAsync($"{channel.Id}{x.Id}").GetAwaiter().GetResult()).ToList();

                    if (clipsToShare.Count > 0)
                    {
                        foreach (var item in clipsToShare)
                        {
                            var redisKey = $"{channel.Id}{item.Id}";
                            await _discordClient.SendMessageAsync(channel, $"{item.Title} {item.Url}");
                            await _redisRepository.AddAsync(redisKey, item.Url);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    await Task.Delay(AppSettings.GeneralSettings.DelayTimeAsMs);
                }
            }
        }

        async static Task<Auth> AuthenticateAsync()
        {
            string authUri = Constants.TwitchConstants.AUTH_URL + $"/oauth2/token?client_id={AppSettings.TwitchSettings.ClientId}&client_secret={AppSettings.TwitchSettings.ClientSecret}&grant_type=client_credentials";
            var result = await _httpClient.PostAsync(authUri, null);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<Auth>(jsonResult);

            return auth;
        }

        async static Task<User> GetUserAsync(Auth auth)
        {
            string userUri = Constants.TwitchConstants.API_URL + $"/users?login={AppSettings.TwitchSettings.ChannelName}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, userUri);
            httpRequestMessage.Headers.Add("Authorization", "Bearer " + auth.AccessToken);
            httpRequestMessage.Headers.Add("Client-Id", AppSettings.TwitchSettings.ClientId);
            var result = await _httpClient.SendAsync(httpRequestMessage);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserWrapper>(jsonResult);

            return user.Data.FirstOrDefault();
        }

        async static Task<List<Clip>> GetClipsAsync(Auth auth, string userId)
        {
            var startedAt = DateTime.UtcNow.AddDays(-5).Date.ToString("O");
            var endedAt = DateTime.UtcNow.AddDays(1).Date.ToString("O");
            string clipUri = Constants.TwitchConstants.API_URL + $"/clips?broadcaster_id={userId}&started_at={startedAt}&ended_at={endedAt}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, clipUri);
            httpRequestMessage.Headers.Add("Authorization", "Bearer " + auth.AccessToken);
            httpRequestMessage.Headers.Add("Client-Id", AppSettings.TwitchSettings.ClientId);
            var result = await _httpClient.SendAsync(httpRequestMessage);
            var jsonResult = await result.Content.ReadAsStringAsync();
            var clips = JsonConvert.DeserializeObject<ClipWrapper>(jsonResult);

            return clips.Data;
        }
    }
}
