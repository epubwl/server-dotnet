dotnet ef migrations add UserInitialMigration --context UserDbContext
dotnet ef migrations add EpubMatadataInitialMigration --context EpubMetadataDbContext
dotnet ef migrations add EpubFileInitialMigration --context EpubFileDbContext
dotnet ef migrations add EpubCoverInitialMigration --context EpubCoverDbContext

dotnet ef database update --context UserDbContext
dotnet ef database update --context EpubMetadataDbContext
dotnet ef database update --context EpubFileDbContext
dotnet ef database update --context EpubCoverDbContext