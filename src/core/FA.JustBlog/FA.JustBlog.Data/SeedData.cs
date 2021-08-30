﻿using FA.JustBlog.Models.Common;
using FA.JustBlog.Models.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FA.JustBlog.Data
{
    public class SeedData
    {
        public static void Seed(
            JustBlogDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            var task = Task.Run(async () => await InitializeIdentity(userManager, roleManager));

            context.Database.EnsureCreated();

            if (!context.Categories.Any())
            {
                #region Add Categories

                var categories = new Category[]
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Travel",
                        UrlSlug =   "travel",
                        Description ="Travel Blog",
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Recipe",
                        UrlSlug =   "recipe",
                        Description ="Recipe Blog",
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tips",
                        UrlSlug =   "tips",
                        Description ="Tips Blog",
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Life Style",
                        UrlSlug =   "life-style",
                        Description ="Life Style Blog",
                        IsDeleted = false
                    }
                };
                context.Categories.AddRange(categories);

                #endregion

                #region Add Posts

                var posts = new List<Post>
                {
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 01",
                        UrlSlug = "post-01",
                        ShortDescription = "This is Post 01",
                        ImageUrl = "blog-1.jpg",
                        PostContent = "Content post 01",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[0].Name),
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 02",
                        UrlSlug = "post-02",
                        ShortDescription = "This is Post 02",
                        ImageUrl = "blog-2.jpg",
                        PostContent = "Content post 02",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[3].Name),
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 03",
                        UrlSlug = "post-03",
                        ShortDescription = "This is Post 03",
                        ImageUrl = "blog-3.jpg",
                        PostContent = "Content post 03",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[1].Name),
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 04",
                        UrlSlug = "post-04",
                        ShortDescription = "This is Post 04",
                        ImageUrl = "blog-4.jpg",
                        PostContent = "Content post 04",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[2].Name),
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 05",
                        UrlSlug = "post-05",
                        ShortDescription = "This is Post 05",
                        ImageUrl = "blog-5.jpg",
                        PostContent = "Content post 05",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[1].Name),
                    },
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        Title = "Post 06",
                        UrlSlug = "post-06",
                        ShortDescription = "This is Post 06",
                        ImageUrl = "blog-6.jpg",
                        PostContent = "Content post 06",
                        PublishedDate = DateTime.Now,
                        IsDeleted = false,
                        Published = true,
                        Category = categories.Single(x => x.Name == categories[0].Name),
                    }
                };
                context.Posts.AddRange(posts);

                #endregion

                var tags = new List<Tag>
                {
                    new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = "travel",
                        UrlSlug = "travel",
                        Description = "Travel Tag",
                        IsDeleted = false
                    },
                    new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = "food",
                        UrlSlug = "food",
                        Description = "food Tag",
                        IsDeleted = false
                    },
                    new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = "recipe",
                        UrlSlug = "recipe",
                        Description = "recipe Tag",
                        IsDeleted = false
                    },
                    new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = "tips",
                        UrlSlug = "tips",
                        Description = "tips Tag",
                        IsDeleted = false
                    },
                    new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = "study",
                        UrlSlug = "study",
                        Description = "study Tag",
                        IsDeleted = false
                    }
                };
                context.Tags.AddRange(tags);

                var postTags = new List<PostTag>
                {
                    new PostTag
                    {
                        Post = posts[0],
                        Tag = tags[0],
                    },
                    new PostTag
                    {
                        Post = posts[0],
                        Tag = tags[1],
                    },
                    new PostTag
                    {
                        Post = posts[0],
                        Tag = tags[2],
                    },
                    new PostTag
                    {
                        Post = posts[1],
                        Tag = tags[0],
                    },
                    new PostTag
                    {
                        Post = posts[1],
                        Tag = tags[2],
                    },
                    new PostTag
                    {
                        Post = posts[2],
                        Tag = tags[3],
                    }
                };
                context.PostTags.AddRange(postTags);

                context.SaveChanges();
            }
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static async Task InitializeIdentity(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            const string name = "admin@example.com";
            const string rawPassword = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole<Guid>()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                };
                var roleResult = await roleManager.CreateAsync(role);
            }

            var user = await userManager.FindByNameAsync(name);

            if (user == null)
            {

                user = new User()
                {
                    Id = Guid.NewGuid(),
                    UserName = name,
                    Email = name
                };

                var passwordHasher = new PasswordHasher<User>();

                var password = passwordHasher.HashPassword(user, rawPassword);
                var result = await userManager.CreateAsync(user);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = await userManager.GetRolesAsync(user);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = await userManager.AddToRoleAsync(user, role.Name);
            }
        }
    }
}