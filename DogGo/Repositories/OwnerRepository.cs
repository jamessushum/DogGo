using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        public OwnerRepository(IConfiguration config)
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

        // Method to get all owners from database
        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        o.Id,
	                                        o.Name,
	                                        o.Email,
	                                        o.Address,
	                                        o.Phone,
	                                        o.NeighborhoodId,
	                                        n.Id AS IdNeighborhood,
	                                        n.Name As NeighborhoodName
                                        FROM
	                                        Owner o
	                                        LEFT JOIN Neighborhood n ON o.NeighborhoodId = n.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdNeighborhood")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                            }
                        };

                        owners.Add(owner);
                    }

                    reader.Close();

                    return owners;
                }
            }
        }

        // Method to get individual owner by Id from database
        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        Id,
	                                        Email,
	                                        Name,
	                                        Address,
	                                        NeighborhoodId,
	                                        Phone
                                        FROM
	                                        Owner
                                        WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        reader.Close();
                        return owner;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        // Method to get owner by email from database
        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
	                                        Id,
	                                        Email,
	                                        Name,
	                                        Address,
	                                        NeighborhoodId,
	                                        Phone
                                        FROM
	                                        Owner
                                        WHERE
	                                        Email = @Email";

                    cmd.Parameters.AddWithValue("@Email", email);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        reader.Close();
                        return owner;
                    }

                    reader.Close();
                    return null;
                }
            }
        }

        // Method to add new owner to database and assign new id
        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Owner
	                                        (Name, Email, Phone, Address, NeighborhoodId)
                                        OUTPUT INSERTED.ID
                                        VALUES
	                                        (@Name, @Email, @Phone, @Address, @NeighborhoodId);";

                    cmd.Parameters.AddWithValue("@Name", owner.Name);
                    cmd.Parameters.AddWithValue("@Email", owner.Email);
                    cmd.Parameters.AddWithValue("@Phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@Address", owner.Address);
                    cmd.Parameters.AddWithValue("@NeighborhoodId", owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.Id = id;
                }
            }
        }

        // Method to update existing owner in database
        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Owner
                                        SET
	                                        Name = @Name,
	                                        Email = @Email,
	                                        Address = @Address,
	                                        Phone = @Phone,
	                                        NeighborhoodId = @NeighborhoodId
                                        WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Name", owner.Name);
                    cmd.Parameters.AddWithValue("@Email", owner.Email);
                    cmd.Parameters.AddWithValue("@Phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@Address", owner.Address);
                    cmd.Parameters.AddWithValue("@NeighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@Id", owner.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Method to delete existing owner from database
        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Owner
                                        WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
