﻿using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        public DogRepository(IConfiguration config)
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

        // Method to get all dogs from database
        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        Id,
	                                        Name,
	                                        Breed,
	                                        Notes,
	                                        ImageUrl,
	                                        OwnerId
                                        FROM
	                                        Dog";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }

        // Method to get single dog by id from database
        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        Id,
	                                        Name,
	                                        Breed,
	                                        Notes,
	                                        ImageUrl,
	                                        OwnerId
                                        FROM
	                                        Dog
                                        WHERE
	                                        Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        };

                        reader.Close();
                        return dog;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        // Method to add new dog to database
        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Dog
	                                        (Name, OwnerId, Breed, Notes, ImageUrl)
                                        OUTPUT INSERTED.Id
                                        VALUES
	                                        (@Name, @OwnerId, @Breed, @Notes, @ImageUrl)";

                    cmd.Parameters.AddWithValue("@Name", dog.Name);
                    cmd.Parameters.AddWithValue("@OwnerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@Breed", dog.Breed);
                   
                    // Inserts NULL value in database if Notes and ImageUrl values are null
                    if (dog.Notes == null)
                    {
                        cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Notes", dog.Notes);
                    }

                    if (dog.ImageUrl == null)
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", dog.ImageUrl);
                    }

                    int id = (int)cmd.ExecuteScalar();

                    dog.Id = id;
                }
            }
        }

        // Method updates existing dog in database
        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Dog
                                        SET
	                                        Name = @Name,
	                                        OwnerId = @OwnerId,
	                                        Breed = @Breed,
	                                        Notes = @Notes,
	                                        ImageUrl = @ImageUrl
                                        WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Name", dog.Name);
                    cmd.Parameters.AddWithValue("@OwnerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@Breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@Id", dog.Id);

                    if (dog.Notes == null)
                    {
                        cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Notes", dog.Notes);
                    }

                    if (dog.ImageUrl == null)
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", dog.ImageUrl);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method to delete existing dog from database
        public void DeleteDog(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Dog
                                        WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method to get list of dogs by owner id from database
        public List<Dog> GetDogsByOwnerId(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        Id,
	                                        Name,
	                                        OwnerId,
	                                        Breed,
	                                        Notes,
	                                        ImageUrl
                                        FROM 
	                                        Dog
                                        WHERE
	                                        OwnerId = @OwnerId";

                    cmd.Parameters.AddWithValue("@OwnerId", ownerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl"))
                        };

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }
    }
}
