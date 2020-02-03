using System;
using System.Net.NetworkInformation;
using FluentMigrator;

namespace BoardgamesStatisticsCounter.Data.Migrations
{
    [Migration(20200131184900)]
    public class AddBaseDatabaseStructure : Migration
    {
        public override void Up()
        {
            Create.Table("user_chats")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("chat_it").AsString().Indexed().Unique();

            Create.Table("games")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString().Indexed().Unique();

            Create.Table("user_games")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("game_id").AsString()
                .WithColumn("user_id").AsInt64()
                .WithColumn("game_datetime").AsDateTime()
                .WithColumn("players").AsAnsiString().Nullable()
                .WithColumn("score").AsString().Nullable()
                .WithColumn("winner").AsString().Nullable();
            
            Create.ForeignKey()
                .FromTable("user_games").ForeignColumn("user_id")
                .ToTable("user_chats").PrimaryColumn("id");
            
            Create.ForeignKey()
                .FromTable("user_games").ForeignColumn("game_id")
                .ToTable("games").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.Table("user_chats");
            Delete.Table("games");
            Delete.Table("user_games");
        }
    }
}
