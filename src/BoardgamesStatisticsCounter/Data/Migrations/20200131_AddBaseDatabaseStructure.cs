using FluentMigrator;

namespace BoardgamesStatisticsCounter.Data.Migrations
{
    [Migration(20200131184900)]
    public class AddBaseDatabaseStructure : Migration
    {
        public override void Up()
        {
            Create.Table("user_chat")
                .WithColumn("UserId").AsInt64().PrimaryKey().Identity()
                .WithColumn("ChatId").AsString();

            Create.Table("Games")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("GameName").AsString();

            Create.Table("UserGames")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("GameId").AsString()
                .WithColumn("UserId").AsInt64()
                .WithColumn("GameDateUtc").AsDateTime()
                .WithColumn("GamePlayers").AsAnsiString()
                .WithColumn("GameScore").AsString()
                .WithColumn("Winner").AsString();
        }

        public override void Down()
        {
            Delete.Table("Log");
        }
    }
}
