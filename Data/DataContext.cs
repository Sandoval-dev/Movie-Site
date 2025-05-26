using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieSite.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<MovieList> MovieLists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new List<Category>
            {
                new Category { Id = 1, Name = "Action" },
                new Category { Id = 2, Name = "Drama" },
                new Category { Id = 3, Name = "Comedy" },
                new Category { Id = 4, Name = "Horror" },
                new Category { Id = 5, Name = "Sci-Fi" },
            }
        );

        modelBuilder.Entity<Movie>().HasData(
            new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "Inception",
                    Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.",
                    ImageUrl = "inception.png",
                    Rating=4.8f,
                    ReleaseDate = new DateTime(2010, 7, 16),
                    CategoryId = 5
                },
                new Movie
                {
                    Id = 2,
                    Title = "The Dark Knight",
                    Description = "When the menace known as the Joker emerges from his mysterious past, he wreaks havoc and chaos on the people of Gotham.",
                    ImageUrl = "dark.jpg",
                    Rating=4.7f,
                    ReleaseDate = new DateTime(2008, 7, 18),
                    CategoryId = 1
                },
                new Movie
                {
                    Id = 3,
                    Title = "The Shawshank Redemption",
                    Description = "A banker convicted of uxoricide forms a friendship over a quarter century with a hardened convict, while maintaining his innocence and trying to remain hopeful through simple compassion.",
                    ImageUrl = "Esaretin-bedeli.jpg",
                    Rating=4.9f,
                    ReleaseDate = new DateTime(1994, 9, 23),
                    CategoryId = 2
                },
                new Movie
                {
                    Id = 4,
                    Title = "The Godfather",
                    Description = "An organized crime dynasty's aging patriarch transfers control of his clandestine empire to his reluctant son.",
                    ImageUrl = "godfather.jpg",
                    Rating=4.8f,
                    ReleaseDate = new DateTime(1972, 3, 24),
                    CategoryId = 2
                },
                new Movie
                {
                    Id = 5,
                    Title = "The Matrix",
                    Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                    ImageUrl = "matrix.jpeg",
                    Rating=4.7f,
                    ReleaseDate = new DateTime(1999, 3, 31),
                    CategoryId = 5
                },
                new Movie{
                    Id = 6,
                    Title="Justice League",
                    Description="Fueled by his restored faith in humanity and inspired by Superman's selfless act, Bruce Wayne enlists the help of his newfound ally, Diana Prince, to face an even greater enemy.",
                    ImageUrl="justiceleague.jpg",
                    Rating=4.0f,
                    ReleaseDate=new DateTime(2017, 11, 17),
                    CategoryId=1
                },
                new Movie{
                    Id = 7,
                    Title="The Hangover",
                    Description="Three buddies wake up from a bachelor party in Las Vegas, with no memory of the previous night and the bachelor missing.",
                    ImageUrl="hangover.jpg",
                    Rating=4.7f,
                    ReleaseDate=new DateTime(2009, 6, 5),
                    CategoryId=3
                },
                new Movie{
                    Id = 8,
                    Title="The Conjuring",
                    Description="Paranormal investigators Ed and Lorraine Warren work to help a family terrorized by a dark presence in their farmhouse.",
                    ImageUrl="conjuring.jpg",
                    Rating=4.2f,
                    ReleaseDate=new DateTime(2013, 7, 19),
                    CategoryId=4
                },
                new Movie{
                    Id = 9,
                    Title="The Social Network",
                    Description="As Harvard students create the social networking site that would become known as Facebook, they are forced to deal with both personal and legal issues.",
                    ImageUrl="socialnetwork.jpg",
                    Rating=3.8f,
                    ReleaseDate=new DateTime(2010, 10, 1),
                    CategoryId=2
                },
                new Movie{
                    Id = 10,
                    Title="The Avengers",
                    Description="Earth's mightiest heroes must come together and learn to fight as a team if they are going to stop the mischievous Loki and his alien army from enslaving humanity.",
                    ImageUrl="avengers.jpg",
                    Rating=4.5f,
                    ReleaseDate=new DateTime(2012, 5, 4),
                    CategoryId=1
                }
            }
        );
    }


    


}