#!/bin/sh

dotnet ef database update 0 --context UserDbContext
dotnet ef database update 0 --context EpubMetadataDbContext
dotnet ef database update 0 --context EpubFileDbContext
dotnet ef database update 0 --context EpubCoverDbContext

dotnet ef database update --context UserDbContext
dotnet ef database update --context EpubMetadataDbContext
dotnet ef database update --context EpubFileDbContext
dotnet ef database update --context EpubCoverDbContext