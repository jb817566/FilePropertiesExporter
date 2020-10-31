using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public partial class FlacFile
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Size")]
        public string Size { get; set; }

        [JsonProperty("Item type")]
        public string ItemType { get; set; }

        [JsonProperty("Date modified")]
        public string DateModified { get; set; }

        [JsonProperty("Date created")]
        public string DateCreated { get; set; }

        [JsonProperty("Date accessed")]
        public string DateAccessed { get; set; }

        [JsonProperty("Attributes")]
        public string Attributes { get; set; }

        [JsonProperty("Perceived type")]
        public string PerceivedType { get; set; }

        [JsonProperty("Owner")]
        public string Owner { get; set; }

        [JsonProperty("Kind")]
        public string Kind { get; set; }

        [JsonProperty("Contributing artists")]
        public string ContributingArtists { get; set; }

        [JsonProperty("Album")]
        public string Album { get; set; }

        [JsonProperty("Rating")]
        public string Rating { get; set; }

        [JsonProperty("Authors")]
        public string Authors { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Copyright")]
        public string Copyright { get; set; }

        [JsonProperty("#")]
        public long Empty { get; set; }

        [JsonProperty("Length")]
        public TimeSpan Length { get; set; }

        [JsonProperty("Bit rate")]
        public string BitRate { get; set; }

        [JsonProperty("Total size")]
        public string TotalSize { get; set; }

        [JsonProperty("Computer")]
        public string Computer { get; set; }

        [JsonProperty("File extension")]
        public string FileExtension { get; set; }

        [JsonProperty("Filename")]
        public string Filename { get; set; }

        [JsonProperty("Space free")]
        public string SpaceFree { get; set; }

        [JsonProperty("Shared")]
        public string Shared { get; set; }

        [JsonProperty("Folder name")]
        public string FolderName { get; set; }

        [JsonProperty("Folder path")]
        public string FolderPath { get; set; }

        [JsonProperty("Folder")]
        public string Folder { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Link status")]
        public string LinkStatus { get; set; }

        [JsonProperty("Date released")]
        public DateTimeOffset DateReleased { get; set; }

        [JsonProperty("Album artist")]
        public string AlbumArtist { get; set; }

        [JsonProperty("Composers")]
        public string Composers { get; set; }

        [JsonProperty("Part of set")]
        public long PartOfSet { get; set; }

        [JsonProperty("Space used")]
        public string SpaceUsed { get; set; }

        [JsonProperty("Shared with")]
        public string SharedWith { get; set; }

        [JsonProperty("Sharing status")]
        public string SharingStatus { get; set; }
    }
}