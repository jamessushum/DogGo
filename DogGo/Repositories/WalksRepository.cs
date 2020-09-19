using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class WalksRepository : IWalksRepository
    {
        private readonly IConfiguration _config;

        public WalksRepository(IConfiguration config)
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

        // Method to get all walks by walker id from database
        public List<Walks> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        w.Id,
	                                        w.Date,
	                                        w.Duration,
	                                        w.WalkerId,
	                                        w.DogId,
	                                        wr.Id AS IdWalker,
	                                        wr.Name AS WalkerName,
	                                        wr.ImageUrl AS WalkerImageUrl,
	                                        wr.NeighborhoodId AS WalkerNeighborhoodId,
	                                        d.Id AS IdDog,
	                                        d.Name AS DogName,
	                                        d.OwnerId,
	                                        d.Breed,
	                                        d.Notes,
	                                        d.ImageUrl AS DogImageUrl,
	                                        o.Id AS IdOwner,
	                                        o.Email,
	                                        o.Name AS OwnerName,
	                                        o.Address,
	                                        o.NeighborhoodId AS OwnerNeighborhoodId,
	                                        o.Phone,
	                                        n.Id AS IdWalkerNeighborhood,
	                                        n.Name AS WalkerNeighborhoodName
                                        FROM Walks w
	                                        LEFT JOIN Dog d ON w.DogId = d.Id
	                                        LEFT JOIN Walker wr ON w.WalkerId = wr.Id
	                                        LEFT JOIN Owner o ON d.OwnerId = o.Id
	                                        LEFT JOIN Neighborhood n ON wr.NeighborhoodId = n.Id
                                        WHERE
	                                        w.WalkerId = @Id";

                    cmd.Parameters.AddWithValue("@Id", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walks> walks = new List<Walks>();

                    while (reader.Read())
                    {
                        Walks walk = new Walks
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Walker = new Walker()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdWalker")),
                                Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("WalkerNeighborhoodId")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("WalkerImageUrl")),
                                Neighborhood = new Neighborhood()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("IdWalkerNeighborhood")),
                                    Name = reader.GetString(reader.GetOrdinal("WalkerNeighborhoodName"))
                                }
                            },
                            Dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdDog")),
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("DogImageUrl")) ? null : reader.GetString(reader.GetOrdinal("DogImageUrl")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Owner = new Owner()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("IdOwner")),
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    NeighborhoodId = reader.GetInt32(reader.GetOrdinal("OwnerNeighborhoodId"))
                                }
                            }
                        };

                        walks.Add(walk);
                    }

                    reader.Close();

                    return walks;
                }
            }
        }
    }
}
