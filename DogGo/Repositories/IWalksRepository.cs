using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalksRepository
    {
        List<Walks> GetWalksByWalkerId(int walkerId);
    }
}