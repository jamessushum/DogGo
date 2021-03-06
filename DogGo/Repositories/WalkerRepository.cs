﻿using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        w.Id,
	                                        w.Name,
	                                        w.ImageUrl,
	                                        w.NeighborhoodId,
	                                        n.Id AS IdNeighborhood,
	                                        n.Name AS NeighborhoodName
                                        FROM
	                                        Walker w
	                                        LEFT JOIN Neighborhood n ON w.NeighborhoodId = n.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdNeighborhood")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
	                                        w.Id, 
	                                        w.[Name], 
	                                        w.ImageUrl, 
	                                        w.NeighborhoodId,
	                                        n.Id AS IdNeighborhood,
	                                        n.Name AS NeighborhoodName
                                        FROM Walker w
	                                        LEFT JOIN Neighborhood n ON w.NeighborhoodId = n.Id
                                        WHERE w.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdNeighborhood")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        // Method to get list of walkers by neighborhood id from database
        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        w.Id,
	                                        w.Name,
	                                        w.ImageUrl,
	                                        w.NeighborhoodId,
	                                        n.Id AS IdNeighborhood,
	                                        n.Name AS NeighborhoodName
                                        FROM
	                                        Walker w
	                                        LEFT JOIN Neighborhood n ON w.NeighborhoodId = n.Id
                                        WHERE
	                                        w.NeighborhoodId = @NeighborhoodId";

                    cmd.Parameters.AddWithValue("@NeighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();

                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdNeighborhood")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        // Method to add a new walker to database
        public void AddWalker(Walker walker)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Walker
	                                        (Name, ImageUrl, NeighborhoodId)
                                        OUTPUT INSERTED.Id
                                        VALUES
	                                        (@Name, @ImageUrl, @NeighborhoodId)";

                    cmd.Parameters.AddWithValue("@Name", walker.Name);
                    cmd.Parameters.AddWithValue("@ImageUrl", walker.ImageUrl);
                    cmd.Parameters.AddWithValue("@NeighborhoodId", walker.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    walker.Id = id;
                }
            }
        }
    }
}