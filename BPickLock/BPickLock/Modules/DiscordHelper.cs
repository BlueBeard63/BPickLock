using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BPickLock.Modules
{
    public static class DiscordHelpers
    {
        public static string DateTimeToISO(DateTime dateTime)
        {
            return DateTimeToISO(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        public static string DateTimeToISO(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Local)
                .ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }

    public class WebhookAuthor
    {
        public string name;
        public string url;
        public string icon_url;
    }

    public class WebhookEmbed
    {
        [JsonIgnore]
        private WebhookMessage parent;

        internal WebhookEmbed(WebhookMessage parent)
        {
            this.parent = parent;
        }

        public WebhookEmbed()
        {
        }

        public WebhookMessage Finalize()
        {
            if (parent == null)
            {
                parent = new WebhookMessage() { embeds = new List<WebhookEmbed>() { this } };
            }
            return parent;
        }

        public int color;

        public WebhookAuthor author;

        public string title;

        public string url;

        public string description;

        public List<WebhookField> fields = new List<WebhookField>();

        public WebhookImage image;

        public WebhookImage thumbnail;

        public WebhookFooter footer;

        public string timestamp;

        public WebhookEmbed WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public WebhookEmbed WithURL(string value)
        {
            this.url = value;
            return this;
        }

        public WebhookEmbed WithDescription(string value)
        {
            this.description = value;
            return this;
        }

        public WebhookEmbed WithTimestamp(DateTime value)
        {
            this.timestamp = DiscordHelpers.DateTimeToISO(value);
            return this;
        }

        public WebhookEmbed WithField(string name, string value, bool inline = true)
        {
            this.fields.Add(new WebhookField() { value = value, inline = inline, name = name });
            return this;
        }

        public WebhookEmbed WithImage(string value)
        {
            this.image = new WebhookImage() { url = value };
            return this;
        }

        public WebhookEmbed WithThumbnail(string value)
        {
            this.thumbnail = new WebhookImage() { url = value };
            return this;
        }

        public WebhookEmbed WithAuthor(string name, string url = null, string icon = null)
        {
            this.author = new WebhookAuthor() { name = name, icon_url = icon, url = url };
            return this;
        }

        public WebhookEmbed WithColor(Color color)
        {
            byte r = Clamp(color.r);
            byte g = Clamp(color.g);
            byte b = Clamp(color.b);

            int numeric = BitConverter.ToInt32(new byte[4] { b, g, r, 0 }, 0);
            this.color = numeric;
            return this;
        }

        private byte Clamp(float a)
        {
            return (byte)(Math.Round(a * 255, 0));
        }
    }

    public class WebhookField
    {
        public string name;

        public string value;

        public bool inline;
    }

    public class WebhookFooter
    {
        public string text;

        public string icon_url;
    }

    public class WebhookImage
    {
        public string url = "";
    }

    public class WebhookMessage
    {
        public string username;

        public string avatar_url;

        public string content = "";

        public List<WebhookEmbed> embeds = new List<WebhookEmbed>();

        public bool tts { get; set; }

        public WebhookMessage WithEmbed(WebhookEmbed embed)
        {
            embeds.Add(embed);
            return this;
        }

        public WebhookEmbed PassEmbed()
        {
            WebhookEmbed embed = new WebhookEmbed(this);
            embeds.Add(embed);
            return embed;
        }

        public WebhookMessage WithUsername(string un)
        {
            username = un;
            return this;
        }

        public WebhookMessage WithAvatar(string avatar)
        {
            avatar_url = avatar;
            return this;
        }

        public WebhookMessage WithContent(string c)
        {
            content = c;
            return this;
        }

        public WebhookMessage WithTTS()
        {
            tts = true;
            return this;
        }
    }
}
