using LPREventReadBulkInserter.Models;
using System;
using System.Collections.Generic;

namespace LPREventReadBulkInserter.Logic
{
    public class Utilities
    {
        public static List<Person> GeneratePeople(int toGenerate)
        {
            List<Person> toReturn = new List<Person>();

            for (int i = 0; i < toGenerate; i++)
            {
                DateTime? birthDate = DateTime.Now;
                string favoriteMovie = $"Favorite Movie {i}";
                int? favoriteNumber = i;

                if (i % 10 == 0)
                {
                    birthDate = null;
                    favoriteMovie = null;
                    favoriteNumber = null;
                }

                toReturn.Add
                (
                    new Person
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Person {i}",
                        BirthDate = birthDate,
                        FavoriteMovie = favoriteMovie,
                        FavoriteNumber = favoriteNumber,
                    }
                );
            }

            return toReturn;
        }
    }
}
