#!/bin/bash


# Drop the database
echo "Dropping the database..."
dotnet ef database drop --force

# Remove all migrations (optional: back up the migrations folder if needed)
# echo "Removing all migrations..."
# rm -rf ./Migrations
#
# # Add a new migration
# echo "Creating new migration..."
# dotnet ef migrations add InitialMigration

# Update the database with the new migration
echo "Applying migration to the database..."
dotnet ef database update

echo "Database has been redone with the new migration."